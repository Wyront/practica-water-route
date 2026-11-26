-- WaterRoute — MySQL 8.0 triggers (отдельный файл для ясности)
-- Запуск после schema.sql: mysql -u root -p waterroute < triggers.sql
-- Триггеры только для защиты инвариантов и аудита.
-- Отчёты и бизнес-логика — в хранимых процедурах.

SET NAMES utf8mb4;
DELIMITER $$

-- ============================================================
-- 1. ЗАЩИТА ОСТАТКОВ И РЕЗЕРВА
-- ============================================================

-- Запрет отрицательного физического остатка
CREATE TRIGGER `trg_stock_no_negative`
BEFORE UPDATE ON `warehouse_stock`
FOR EACH ROW
BEGIN
    IF NEW.`quantity` < 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Остаток на складе не может быть отрицательным';
    END IF;
END$$

-- Резерв не превышает физический остаток
CREATE TRIGGER `trg_stock_reserved_not_exceed`
BEFORE UPDATE ON `warehouse_stock`
FOR EACH ROW
BEGIN
    IF NEW.`reserved_quantity` > NEW.`quantity` THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Резервированное количество не может превышать физический остаток';
    END IF;
END$$

-- ============================================================
-- 2. ПРОВЕРКА ФАКТИЧЕСКОЙ ДОСТАВКИ
-- ============================================================

-- Переданное количество не больше заказанного (INSERT)
CREATE TRIGGER `trg_delivery_qty_check_ins`
BEFORE INSERT ON `delivery_items`
FOR EACH ROW
BEGIN
    DECLARE v_ordered INT;
    SELECT `quantity` INTO v_ordered FROM `order_items` WHERE `id` = NEW.`order_item_id`;
    IF v_ordered IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Позиция заказа не найдена';
    ELSEIF NEW.`quantity_delivered` > v_ordered THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Переданное количество не может превышать заказанное';
    END IF;
END$$

-- Переданное количество не больше заказанного (UPDATE)
CREATE TRIGGER `trg_delivery_qty_check_upd`
BEFORE UPDATE ON `delivery_items`
FOR EACH ROW
BEGIN
    DECLARE v_ordered INT;
    SELECT `quantity` INTO v_ordered FROM `order_items` WHERE `id` = NEW.`order_item_id`;
    IF NEW.`quantity_delivered` > v_ordered THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Переданное количество не может превышать заказанное';
    END IF;
END$$

-- Статус "delivered" только при полном соответствии всех позиций
CREATE TRIGGER `trg_delivery_full_check`
BEFORE UPDATE ON `route_stops`
FOR EACH ROW
BEGIN
    IF NEW.`status` = 'delivered' AND OLD.`status` <> 'delivered' THEN
        DECLARE v_missing INT DEFAULT 0;
        SELECT COUNT(*) INTO v_missing
        FROM `order_items` oi
        LEFT JOIN `delivery_items` di ON di.`order_item_id` = oi.`id`
        LEFT JOIN `deliveries` d ON d.`id` = di.`delivery_id`
        WHERE oi.`order_id` = NEW.`order_id`
          AND (di.`quantity_delivered` IS NULL OR di.`quantity_delivered` < oi.`quantity`);
        IF v_missing > 0 THEN
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Не все позиции переданы полностью — нельзя установить статус delivered';
        END IF;
    END IF;
END$$

-- ============================================================
-- 3. ИСТОРИЯ СТАТУСОВ
-- ============================================================

-- Автоматическая запись при смене статуса заказа
CREATE TRIGGER `trg_status_history`
AFTER UPDATE ON `orders`
FOR EACH ROW
BEGIN
    IF NEW.`status` <> OLD.`status` THEN
        INSERT INTO `status_history` (`order_id`, `user_id`, `old_status`, `new_status`, `changed_at`, `note`)
        VALUES (NEW.`id`, NULL, OLD.`status`, NEW.`status`, NOW(), NULL);
    END IF;
END$$

-- ============================================================
-- 4. ОДИН АДРЕС ПО УМОЛЧАНИЮ НА КЛИЕНТА
-- ============================================================

CREATE TRIGGER `trg_ca_single_default_ins`
BEFORE INSERT ON `client_addresses`
FOR EACH ROW
BEGIN
    IF NEW.`is_default` = 1 THEN
        UPDATE `client_addresses` SET `is_default` = 0
        WHERE `user_id` = NEW.`user_id` AND `is_default` = 1;
    END IF;
END$$

CREATE TRIGGER `trg_ca_single_default_upd`
BEFORE UPDATE ON `client_addresses`
FOR EACH ROW
BEGIN
    IF NEW.`is_default` = 1 AND OLD.`is_default` = 0 THEN
        UPDATE `client_addresses` SET `is_default` = 0
        WHERE `user_id` = NEW.`user_id` AND `is_default` = 1 AND `id` <> NEW.`id`;
    END IF;
END$$

-- ============================================================
-- 5. АВТОМАТИЧЕСКАЯ ПРОБЛЕМА ПРИ НЕДОСТАВКЕ
-- ============================================================

CREATE TRIGGER `trg_problem_auto_on_fail`
AFTER UPDATE ON `route_stops`
FOR EACH ROW
BEGIN
    IF NEW.`status` = 'not_delivered' AND OLD.`status` <> 'not_delivered' THEN
        INSERT INTO `notes` (`author_id`, `type`, `target_type`, `target_id`, `category`, `problem_status`, `message`, `created_at`)
        SELECT
            r.`courier_id`,
            'problem',
            'route',
            NEW.`route_id`,
            'not_delivered',
            'open',
            CONCAT('Автоматическая регистрация: точка не доставлена. Заказ: ', o.`order_number`, ', комментарий курьера: ', COALESCE(NEW.`comment`, '—')),
            NOW()
        FROM `routes` r
        JOIN `orders` o ON o.`id` = NEW.`order_id`
        WHERE r.`id` = NEW.`route_id`;
    END IF;
END$$

-- ============================================================
-- 6. ПОДТВЕРЖДЕНИЕ ПРИЁМКИ ТАРЫ
-- ============================================================

CREATE TRIGGER `trg_container_confirm_check`
BEFORE UPDATE ON `container_movements`
FOR EACH ROW
BEGIN
    IF NEW.`confirmation_status` = 'confirmed' AND OLD.`confirmation_status` = 'pending' THEN
        IF NEW.`confirmed_by` IS NULL THEN
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Не указан кладовщик, подтвердивший приём тары';
        END IF;
        IF NEW.`quantity` > OLD.`quantity` THEN
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Количество при подтверждении не может превышать заявленное курьером';
        END IF;
    END IF;
END$$

-- ============================================================
-- 7. АУДИТ КРИТИЧЕСКИХ ИЗМЕНЕНИЙ (опционально, расширяемо)
-- ============================================================

-- Пример: логирование изменения роли пользователя админом
CREATE TRIGGER `trg_audit_user_role_change`
AFTER UPDATE ON `users`
FOR EACH ROW
BEGIN
    IF NEW.`role_id` <> OLD.`role_id` THEN
        INSERT INTO `notes` (`author_id`, `type`, `target_type`, `target_id`, `category`, `message`, `created_at`)
        VALUES (NEW.`id`, 'note', 'user', NEW.`id`, 'role_change',
                CONCAT('Роль изменена с ', (SELECT `code` FROM `roles` WHERE `id` = OLD.`role_id`),
                       ' на ', (SELECT `code` FROM `roles` WHERE `id` = NEW.`role_id`)),
                NOW());
    END IF;
END$$

-- Пример: логирование деактивации пользователя
CREATE TRIGGER `trg_audit_user_deactivate`
AFTER UPDATE ON `users`
FOR EACH ROW
BEGIN
    IF NEW.`active` = 0 AND OLD.`active` = 1 THEN
        INSERT INTO `notes` (`author_id`, `type`, `target_type`, `target_id`, `category`, `message`, `created_at`)
        VALUES (NEW.`id`, 'note', 'user', NEW.`id`, 'deactivation',
                'Учётная запись деактивирована', NOW());
    END IF;
END$$

DELIMITER ;