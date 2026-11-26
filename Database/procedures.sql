-- WaterRoute — MySQL 8.0 stored procedures
-- Запуск после schema.sql: mysql -u root -p waterroute < procedures.sql
-- Все обращения из C# только через CALL sp_...
-- Параметры: p_<name>, ошибки: SIGNAL SQLSTATE '45000'
-- Транзакции внутри процедур там, где требуется атомарность

SET NAMES utf8mb4;
DELIMITER $$

-- ============================================================
-- ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ
-- ============================================================

-- Проверить, свободен ли ресурс (нет активных маршрутов planned/in_progress)
CREATE FUNCTION `fn_is_courier_free`(p_courier_id INT) RETURNS TINYINT
READS SQL DATA
DETERMINISTIC
BEGIN
    DECLARE v_count INT DEFAULT 0;
    SELECT COUNT(*) INTO v_count
    FROM `routes`
    WHERE `courier_id` = p_courier_id
      AND `status` IN ('planned','in_progress');
    RETURN IF(v_count = 0, 1, 0);
END$$

CREATE FUNCTION `fn_is_vehicle_free`(p_vehicle_id INT) RETURNS TINYINT
READS SQL DATA
DETERMINISTIC
BEGIN
    DECLARE v_count INT DEFAULT 0;
    SELECT COUNT(*) INTO v_count
    FROM `routes`
    WHERE `vehicle_id` = p_vehicle_id
      AND `status` IN ('planned','in_progress');
    RETURN IF(v_count = 0, 1, 0);
END$$

CREATE FUNCTION `fn_is_dispatcher_free`(p_dispatcher_id INT) RETURNS TINYINT
READS SQL DATA
DETERMINISTIC
BEGIN
    DECLARE v_count INT DEFAULT 0;
    SELECT COUNT(*) INTO v_count
    FROM `routes`
    WHERE `dispatcher_id` = p_dispatcher_id
      AND `status` IN ('planned','in_progress');
    RETURN IF(v_count = 0, 1, 0);
END$$

-- ============================================================
-- 1. АУТЕНТИФИКАЦИЯ И ПОЛЬЗОВАТЕЛИ
-- ============================================================

-- Вход пользователя
CREATE PROCEDURE `sp_auth_login`(
    IN p_email VARCHAR(255),
    IN p_password_plain VARCHAR(255)
)
BEGIN
    DECLARE v_id INT;
    DECLARE v_hash VARCHAR(255);
    DECLARE v_role_id INT;
    DECLARE v_active TINYINT;
    DECLARE v_full_name VARCHAR(255);
    DECLARE v_role_code VARCHAR(50);

    SELECT u.`id`, u.`password_hash`, u.`role_id`, u.`active`, u.`full_name`, r.`code`
    INTO v_id, v_hash, v_role_id, v_active, v_full_name, v_role_code
    FROM `users` u
    JOIN `roles` r ON r.`id` = u.`role_id`
    WHERE u.`email` = p_email
    LIMIT 1;

    IF v_id IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Пользователь не найден';
    END IF;

    IF v_active = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Учётная запись деактивирована';
    END IF;

    -- bcrypt проверка в приложении (C#), здесь только возвращаем хеш для сверки
    SELECT v_id AS `user_id`, v_full_name AS `full_name`, v_role_id AS `role_id`,
           v_role_code AS `role_code`, v_hash AS `password_hash`;
END$$

-- Валидация сохранённой сессии (auto-login)
CREATE PROCEDURE `sp_auth_validate_session`(
    IN p_user_id INT,
    IN p_session_token VARCHAR(255)
)
BEGIN
    DECLARE v_active TINYINT;
    SELECT `active` INTO v_active FROM `users` WHERE `id` = p_user_id;
    IF v_active IS NULL OR v_active = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Сессия недействительна';
    END IF;
    -- Токен проверяется в приложении (защищённый локальный storage)
    SELECT 1 AS `ok`;
END$$

-- Регистрация клиента (public)
CREATE PROCEDURE `sp_client_register`(
    IN p_full_name VARCHAR(255),
    IN p_email VARCHAR(255),
    IN p_phone VARCHAR(50),
    IN p_password_plain VARCHAR(255),
    OUT p_user_id INT
)
BEGIN
    DECLARE v_role_client INT;
    SELECT `id` INTO v_role_client FROM `roles` WHERE `code` = 'client';

    IF EXISTS (SELECT 1 FROM `users` WHERE `email` = p_email) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'E-mail уже зарегистрирован';
    END IF;
    IF p_phone IS NOT NULL AND EXISTS (SELECT 1 FROM `users` WHERE `phone` = p_phone) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Телефон уже зарегистрирован';
    END IF;

    INSERT INTO `users` (`role_id`, `full_name`, `email`, `phone`, `password_hash`)
    VALUES (v_role_client, p_full_name, p_email, p_phone, p_password_plain); -- хеш создаётся в C#

    SET p_user_id = LAST_INSERT_ID();
END$$

-- Создание сотрудника админом
CREATE PROCEDURE `sp_user_create_employee`(
    IN p_full_name VARCHAR(255),
    IN p_email VARCHAR(255),
    IN p_phone VARCHAR(50),
    IN p_role_code VARCHAR(50),
    IN p_password_plain VARCHAR(255),
    OUT p_user_id INT
)
BEGIN
    DECLARE v_role_id INT;
    SELECT `id` INTO v_role_id FROM `roles` WHERE `code` = p_role_code;
    IF v_role_id IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Роль не найдена';
    END IF;
    IF p_role_code = 'client' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Для клиентов используйте sp_client_register';
    END IF;
    IF EXISTS (SELECT 1 FROM `users` WHERE `email` = p_email) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'E-mail уже занят';
    END IF;
    IF p_phone IS NOT NULL AND EXISTS (SELECT 1 FROM `users` WHERE `phone` = p_phone) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Телефон уже занят';
    END IF;

    INSERT INTO `users` (`role_id`, `full_name`, `email`, `phone`, `password_hash`)
    VALUES (v_role_id, p_full_name, p_email, p_phone, p_password_plain);
    SET p_user_id = LAST_INSERT_ID();
END$$

-- Создание клиента диспетчером
CREATE PROCEDURE `sp_user_create_client_by_dispatcher`(
    IN p_full_name VARCHAR(255),
    IN p_email VARCHAR(255),
    IN p_phone VARCHAR(50),
    IN p_password_plain VARCHAR(255),
    OUT p_user_id INT
)
BEGIN
    CALL `sp_client_register`(p_full_name, p_email, p_phone, p_password_plain, p_user_id);
END$$

-- Смена пароля
CREATE PROCEDURE `sp_user_change_password`(
    IN p_user_id INT,
    IN p_old_password VARCHAR(255),
    IN p_new_password_plain VARCHAR(255)
)
BEGIN
    DECLARE v_hash VARCHAR(255);
    SELECT `password_hash` INTO v_hash FROM `users` WHERE `id` = p_user_id;
    IF v_hash IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Пользователь не найден';
    END IF;
    -- Проверка старого пароля в C# (bcrypt), здесь обновляем хеш
    UPDATE `users` SET `password_hash` = p_new_password_plain WHERE `id` = p_user_id;
END$$

-- Сброс пароля админом
CREATE PROCEDURE `sp_user_reset_password`(
    IN p_user_id INT,
    IN p_new_password_plain VARCHAR(255)
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM `users` WHERE `id` = p_user_id) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Пользователь не найден';
    END IF;
    UPDATE `users` SET `password_hash` = p_new_password_plain WHERE `id` = p_user_id;
END$$

-- Активация/деактивация
CREATE PROCEDURE `sp_user_set_active`(
    IN p_user_id INT,
    IN p_active TINYINT,
    IN p_requestor_id INT
)
BEGIN
    IF p_user_id = p_requestor_id AND p_active = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Нельзя деактивировать самого себя';
    END IF;
    UPDATE `users` SET `active` = p_active WHERE `id` = p_user_id;
    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Пользователь не найден';
    END IF;
END$$

-- Профиль пользователя
CREATE PROCEDURE `sp_user_get_profile`(IN p_user_id INT)
BEGIN
    SELECT u.`id`, u.`full_name`, u.`email`, u.`phone`, u.`role_id`, r.`code` AS `role_code`,
           u.`active`, u.`created_at`
    FROM `users` u
    JOIN `roles` r ON r.`id` = u.`role_id`
    WHERE u.`id` = p_user_id;
END$$

-- Список пользователей (пагинация, фильтры)
CREATE PROCEDURE `sp_user_list`(
    IN p_page INT,
    IN p_limit INT,
    IN p_search VARCHAR(255),
    IN p_role_code VARCHAR(50),
    IN p_active TINYINT
)
BEGIN
    DECLARE v_offset INT DEFAULT (p_page - 1) * p_limit;
    DECLARE v_total INT;

    SELECT COUNT(*) INTO v_total
    FROM `users` u
    JOIN `roles` r ON r.`id` = u.`role_id`
    WHERE (p_search = '' OR u.`full_name` LIKE CONCAT('%', p_search, '%')
                        OR u.`email` LIKE CONCAT('%', p_search, '%')
                        OR u.`phone` LIKE CONCAT('%', p_search, '%'))
      AND (p_role_code = '' OR r.`code` = p_role_code)
      AND (p_active = -1 OR u.`active` = p_active);

    SELECT u.`id`, u.`full_name`, u.`email`, u.`phone`, u.`role_id`, r.`code` AS `role_code`,
           u.`active`, u.`created_at`
    FROM `users` u
    JOIN `roles` r ON r.`id` = u.`role_id`
    WHERE (p_search = '' OR u.`full_name` LIKE CONCAT('%', p_search, '%')
                        OR u.`email` LIKE CONCAT('%', p_search, '%')
                        OR u.`phone` LIKE CONCAT('%', p_search, '%'))
      AND (p_role_code = '' OR r.`code` = p_role_code)
      AND (p_active = -1 OR u.`active` = p_active)
    ORDER BY u.`created_at` DESC
    LIMIT p_limit OFFSET v_offset;

    SELECT v_total AS `total_count`;
END$$

-- ============================================================
-- 2. СПРАВОЧНИКИ (CRUD)
-- ============================================================

CREATE PROCEDURE `sp_payment_method_crud`(
    IN p_action ENUM('CREATE','READ','UPDATE','DELETE'),
    IN p_id INT,
    IN p_code VARCHAR(50),
    IN p_name VARCHAR(100),
    IN p_active TINYINT
)
BEGIN
    CASE p_action
        WHEN 'CREATE' THEN
            IF EXISTS (SELECT 1 FROM `payment_methods` WHERE `code` = p_code) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Код способа оплаты уже существует';
            END IF;
            INSERT INTO `payment_methods` (`code`, `name`, `active`) VALUES (p_code, p_name, p_active);
            SELECT LAST_INSERT_ID() AS `id`;
        WHEN 'READ' THEN
            SELECT * FROM `payment_methods` WHERE `id` = p_id;
        WHEN 'UPDATE' THEN
            IF EXISTS (SELECT 1 FROM `payment_methods` WHERE `code` = p_code AND `id` <> p_id) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Код уже занят';
            END IF;
            UPDATE `payment_methods` SET `code` = p_code, `name` = p_name, `active` = p_active WHERE `id` = p_id;
        WHEN 'DELETE' THEN
            IF EXISTS (SELECT 1 FROM `orders` WHERE `payment_method_id` = p_id) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Способ оплаты используется в заказах';
            END IF;
            DELETE FROM `payment_methods` WHERE `id` = p_id;
    END CASE;
END$$

CREATE PROCEDURE `sp_water_type_crud`(
    IN p_action ENUM('CREATE','READ','UPDATE','DELETE'),
    IN p_id INT,
    IN p_name VARCHAR(150),
    IN p_volume_l DECIMAL(5,2),
    IN p_active TINYINT
)
BEGIN
    CASE p_action
        WHEN 'CREATE' THEN
            IF EXISTS (SELECT 1 FROM `water_types` WHERE `name` = p_name AND `volume_l` = p_volume_l) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Тип воды с таким названием и объёмом уже существует';
            END IF;
            INSERT INTO `water_types` (`name`, `volume_l`, `active`) VALUES (p_name, p_volume_l, p_active);
            SELECT LAST_INSERT_ID() AS `id`;
        WHEN 'READ' THEN SELECT * FROM `water_types` WHERE `id` = p_id;
        WHEN 'UPDATE' THEN
            IF EXISTS (SELECT 1 FROM `water_types` WHERE `name` = p_name AND `volume_l` = p_volume_l AND `id` <> p_id) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Такой тип воды уже существует';
            END IF;
            UPDATE `water_types` SET `name` = p_name, `volume_l` = p_volume_l, `active` = p_active WHERE `id` = p_id;
        WHEN 'DELETE' THEN
            IF EXISTS (SELECT 1 FROM `products` WHERE `water_type_id` = p_id) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Тип воды используется в товарах';
            END IF;
            DELETE FROM `water_types` WHERE `id` = p_id;
    END CASE;
END$$

CREATE PROCEDURE `sp_product_crud`(
    IN p_action ENUM('CREATE','READ','UPDATE','DELETE'),
    IN p_id INT,
    IN p_sku VARCHAR(50),
    IN p_name VARCHAR(255),
    IN p_water_type_id INT,
    IN p_price_kopecks INT,
    IN p_returnable TINYINT,
    IN p_capacity_units INT,
    IN p_active TINYINT
)
BEGIN
    CASE p_action
        WHEN 'CREATE' THEN
            IF EXISTS (SELECT 1 FROM `products` WHERE `sku` = p_sku) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Артикул уже существует';
            END IF;
            IF NOT EXISTS (SELECT 1 FROM `water_types` WHERE `id` = p_water_type_id) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Тип воды не найден';
            END IF;
            INSERT INTO `products` (`sku`, `name`, `water_type_id`, `price_kopecks`, `returnable`, `capacity_units`, `active`)
            VALUES (p_sku, p_name, p_water_type_id, p_price_kopecks, p_returnable, p_capacity_units, p_active);
            SELECT LAST_INSERT_ID() AS `id`;
        WHEN 'READ' THEN SELECT * FROM `products` WHERE `id` = p_id;
        WHEN 'UPDATE' THEN
            IF EXISTS (SELECT 1 FROM `products` WHERE `sku` = p_sku AND `id` <> p_id) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Артикул уже занят';
            END IF;
            UPDATE `products` SET `sku` = p_sku, `name` = p_name, `water_type_id` = p_water_type_id,
                `price_kopecks` = p_price_kopecks, `returnable` = p_returnable, `capacity_units` = p_capacity_units, `active` = p_active
            WHERE `id` = p_id;
        WHEN 'DELETE' THEN
            IF EXISTS (SELECT 1 FROM `order_items` WHERE `product_id` = p_id)
               OR EXISTS (SELECT 1 FROM `warehouse_stock` WHERE `product_id` = p_id) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Товар используется в заказах или остатках — деактивируйте вместо удаления';
            END IF;
            DELETE FROM `products` WHERE `id` = p_id;
    END CASE;
END$$

CREATE PROCEDURE `sp_warehouse_crud`(
    IN p_action ENUM('CREATE','READ','UPDATE','DELETE'),
    IN p_id INT,
    IN p_name VARCHAR(150),
    IN p_address VARCHAR(500),
    IN p_latitude DECIMAL(10,6),
    IN p_longitude DECIMAL(10,6),
    IN p_active TINYINT
)
BEGIN
    CASE p_action
        WHEN 'CREATE' THEN
            IF EXISTS (SELECT 1 FROM `warehouses` WHERE `name` = p_name) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Склад с таким названием уже существует';
            END IF;
            INSERT INTO `warehouses` (`name`, `address`, `latitude`, `longitude`, `active`)
            VALUES (p_name, p_address, p_latitude, p_longitude, p_active);
            SELECT LAST_INSERT_ID() AS `id`;
        WHEN 'READ' THEN SELECT * FROM `warehouses` WHERE `id` = p_id;
        WHEN 'UPDATE' THEN
            IF EXISTS (SELECT 1 FROM `warehouses` WHERE `name` = p_name AND `id` <> p_id) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Название склада уже занято';
            END IF;
            UPDATE `warehouses` SET `name` = p_name, `address` = p_address, `latitude` = p_latitude, `longitude` = p_longitude, `active` = p_active
            WHERE `id` = p_id;
        WHEN 'DELETE' THEN
            IF EXISTS (SELECT 1 FROM `orders` WHERE `warehouse_id` = p_id)
               OR EXISTS (SELECT 1 FROM `warehouse_stock` WHERE `warehouse_id` = p_id)
               OR EXISTS (SELECT 1 FROM `client_addresses` WHERE `warehouse_id` = p_id) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Склад используется — деактивируйте вместо удаления';
            END IF;
            DELETE FROM `warehouses` WHERE `id` = p_id;
    END CASE;
END$$

CREATE PROCEDURE `sp_vehicle_crud`(
    IN p_action ENUM('CREATE','READ','UPDATE','DELETE'),
    IN p_id INT,
    IN p_plate VARCHAR(20),
    IN p_model VARCHAR(100),
    IN p_capacity_units INT,
    IN p_active TINYINT
)
BEGIN
    CASE p_action
        WHEN 'CREATE' THEN
            IF EXISTS (SELECT 1 FROM `vehicles` WHERE `plate` = p_plate) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Госномер уже существует';
            END IF;
            INSERT INTO `vehicles` (`plate`, `model`, `capacity_units`, `active`) VALUES (p_plate, p_model, p_capacity_units, p_active);
            SELECT LAST_INSERT_ID() AS `id`;
        WHEN 'READ' THEN SELECT * FROM `vehicles` WHERE `id` = p_id;
        WHEN 'UPDATE' THEN
            IF EXISTS (SELECT 1 FROM `vehicles` WHERE `plate` = p_plate AND `id` <> p_id) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Госномер уже занят';
            END IF;
            UPDATE `vehicles` SET `plate` = p_plate, `model` = p_model, `capacity_units` = p_capacity_units, `active` = p_active WHERE `id` = p_id;
        WHEN 'DELETE' THEN
            IF EXISTS (SELECT 1 FROM `routes` WHERE `vehicle_id` = p_id) THEN
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Автомобиль используется в маршрутах — деактивируйте';
            END IF;
            DELETE FROM `vehicles` WHERE `id` = p_id;
    END CASE;
END$$

-- Активные справочники для форм
CREATE PROCEDURE `sp_directories_get_active`(IN p_type ENUM('payment','water_type','product','warehouse','vehicle'))
BEGIN
    CASE p_type
        WHEN 'payment' THEN SELECT `id`, `code`, `name` FROM `payment_methods` WHERE `active` = 1 ORDER BY `name`;
        WHEN 'water_type' THEN SELECT `id`, `name`, `volume_l` FROM `water_types` WHERE `active` = 1 ORDER BY `name`;
        WHEN 'product' THEN SELECT `id`, `sku`, `name`, `price_kopecks`, `returnable`, `capacity_units` FROM `products` WHERE `active` = 1 ORDER BY `name`;
        WHEN 'warehouse' THEN SELECT `id`, `name`, `address`, `latitude`, `longitude` FROM `warehouses` WHERE `active` = 1 ORDER BY `name`;
        WHEN 'vehicle' THEN SELECT `id`, `plate`, `model`, `capacity_units` FROM `vehicles` WHERE `active` = 1 ORDER BY `plate`;
    END CASE;
END$$

-- ============================================================
-- 3. КЛИЕНТЫ И АДРЕСА
-- ============================================================

CREATE PROCEDURE `sp_client_list`(
    IN p_page INT, IN p_limit INT,
    IN p_search VARCHAR(255), IN p_active TINYINT
)
BEGIN
    DECLARE v_offset INT DEFAULT (p_page - 1) * p_limit;
    DECLARE v_total INT;

    SELECT COUNT(*) INTO v_total
    FROM `users` u
    JOIN `roles` r ON r.`id` = u.`role_id`
    WHERE r.`code` = 'client'
      AND (p_search = '' OR u.`full_name` LIKE CONCAT('%', p_search, '%')
                          OR u.`email` LIKE CONCAT('%', p_search, '%')
                          OR u.`phone` LIKE CONCAT('%', p_search, '%'))
      AND (p_active = -1 OR u.`active` = p_active);

    SELECT u.`id`, u.`full_name`, u.`email`, u.`phone`, u.`active`,
           (SELECT COUNT(*) FROM `client_addresses` ca WHERE ca.`user_id` = u.`id` AND ca.`active` = 1) AS `addresses_count`,
           (SELECT COUNT(*) FROM `orders` o WHERE o.`client_id` = u.`id`) AS `orders_count`
    FROM `users` u
    JOIN `roles` r ON r.`id` = u.`role_id`
    WHERE r.`code` = 'client'
      AND (p_search = '' OR u.`full_name` LIKE CONCAT('%', p_search, '%')
                          OR u.`email` LIKE CONCAT('%', p_search, '%')
                          OR u.`phone` LIKE CONCAT('%', p_search, '%'))
      AND (p_active = -1 OR u.`active` = p_active)
    ORDER BY u.`created_at` DESC
    LIMIT p_limit OFFSET v_offset;

    SELECT v_total AS `total_count`;
END$$

CREATE PROCEDURE `sp_client_get`(IN p_client_id INT)
BEGIN
    SELECT u.`id`, u.`full_name`, u.`email`, u.`phone`, u.`active`, u.`created_at`
    FROM `users` u
    JOIN `roles` r ON r.`id` = u.`role_id`
    WHERE u.`id` = p_client_id AND r.`code` = 'client';

    SELECT ca.`id`, ca.`warehouse_id`, w.`name` AS `warehouse_name`,
           ca.`city`, ca.`street`, ca.`building`, ca.`apartment`,
           ca.`latitude`, ca.`longitude`, ca.`is_default`, ca.`active`
    FROM `client_addresses` ca
    JOIN `warehouses` w ON w.`id` = ca.`warehouse_id`
    WHERE ca.`user_id` = p_client_id
    ORDER BY ca.`is_default` DESC, ca.`id`;
END$$

CREATE PROCEDURE `sp_client_create`(
    IN p_full_name VARCHAR(255), IN p_phone VARCHAR(50), IN p_email VARCHAR(255), IN p_note VARCHAR(500),
    OUT p_user_id INT
)
BEGIN
    -- Используем временный пароль, клиент сменит при первом входе
    CALL `sp_client_register`(p_full_name, p_email, p_phone, 'temp_password_change_me', p_user_id);
END$$

CREATE PROCEDURE `sp_client_update`(
    IN p_client_id INT, IN p_full_name VARCHAR(255), IN p_phone VARCHAR(50),
    IN p_email VARCHAR(255), IN p_note VARCHAR(500), IN p_active TINYINT
)
BEGIN
    IF EXISTS (SELECT 1 FROM `users` WHERE `email` = p_email AND `id` <> p_client_id) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'E-mail уже занят';
    END IF;
    IF p_phone IS NOT NULL AND EXISTS (SELECT 1 FROM `users` WHERE `phone` = p_phone AND `id` <> p_client_id) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Телефон уже занят';
    END IF;
    UPDATE `users` SET `full_name` = p_full_name, `phone` = p_phone, `email` = p_email, `active` = p_active WHERE `id` = p_client_id;
END$$

CREATE PROCEDURE `sp_client_deactivate`(IN p_client_id INT)
BEGIN
    IF EXISTS (SELECT 1 FROM `orders` WHERE `client_id` = p_client_id AND `status` IN ('new','planned','in_delivery')) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Есть активные заказы — нельзя деактивировать';
    END IF;
    UPDATE `users` SET `active` = 0 WHERE `id` = p_client_id;
END$$

CREATE PROCEDURE `sp_client_address_list`(IN p_client_id INT)
BEGIN
    SELECT ca.`id`, ca.`warehouse_id`, w.`name` AS `warehouse_name`,
           ca.`city`, ca.`street`, ca.`building`, ca.`apartment`,
           ca.`latitude`, ca.`longitude`, ca.`is_default`, ca.`active`
    FROM `client_addresses` ca
    JOIN `warehouses` w ON w.`id` = ca.`warehouse_id`
    WHERE ca.`user_id` = p_client_id
    ORDER BY ca.`is_default` DESC, ca.`id`;
END$$

CREATE PROCEDURE `sp_client_address_add`(
    IN p_client_id INT, IN p_warehouse_id INT,
    IN p_city VARCHAR(100), IN p_street VARCHAR(255), IN p_building VARCHAR(20),
    IN p_apartment VARCHAR(20), IN p_latitude DECIMAL(10,6), IN p_longitude DECIMAL(10,6),
    IN p_is_default TINYINT, OUT p_address_id INT
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM `warehouses` WHERE `id` = p_warehouse_id AND `active` = 1) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Склад не найден или неактивен';
    END IF;
    INSERT INTO `client_addresses` (`user_id`, `warehouse_id`, `city`, `street`, `building`, `apartment`, `latitude`, `longitude`, `is_default`)
    VALUES (p_client_id, p_warehouse_id, p_city, p_street, p_building, p_apartment, p_latitude, p_longitude, p_is_default);
    SET p_address_id = LAST_INSERT_ID();
END$$

CREATE PROCEDURE `sp_client_address_update`(
    IN p_address_id INT, IN p_warehouse_id INT,
    IN p_city VARCHAR(100), IN p_street VARCHAR(255), IN p_building VARCHAR(20),
    IN p_apartment VARCHAR(20), IN p_latitude DECIMAL(10,6), IN p_longitude DECIMAL(10,6),
    IN p_is_default TINYINT, IN p_active TINYINT
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM `warehouses` WHERE `id` = p_warehouse_id) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Склад не найден';
    END IF;
    -- Проверка: если адрес использован в заказах, менять только координаты/флаги, не склад/адрес
    IF EXISTS (SELECT 1 FROM `orders` WHERE `delivery_address_id` = p_address_id) THEN
        UPDATE `client_addresses` SET `latitude` = p_latitude, `longitude` = p_longitude, `is_default` = p_is_default, `active` = p_active WHERE `id` = p_address_id;
    ELSE
        UPDATE `client_addresses` SET `warehouse_id` = p_warehouse_id, `city` = p_city, `street` = p_street,
            `building` = p_building, `apartment` = p_apartment, `latitude` = p_latitude, `longitude` = p_longitude,
            `is_default` = p_is_default, `active` = p_active WHERE `id` = p_address_id;
    END IF;
END$$

CREATE PROCEDURE `sp_client_address_set_default`(IN p_client_id INT, IN p_address_id INT)
BEGIN
    UPDATE `client_addresses` SET `is_default` = 0 WHERE `user_id` = p_client_id AND `is_default` = 1;
    UPDATE `client_addresses` SET `is_default` = 1 WHERE `id` = p_address_id AND `user_id` = p_client_id;
END$$

-- ============================================================
-- 4. КАТАЛОГ, ОСТАТКИ, РЕЗЕРВ
-- ============================================================

-- Каталог по складу
CREATE PROCEDURE `sp_catalog_get`(
    IN p_warehouse_id INT,
    IN p_page INT, IN p_limit INT,
    IN p_search VARCHAR(255), IN p_active_only TINYINT
)
BEGIN
    DECLARE v_offset INT DEFAULT (p_page - 1) * p_limit;
    DECLARE v_total INT;

    SELECT COUNT(*) INTO v_total
    FROM `v_available_stock` vas
    JOIN `products` p ON p.`id` = vas.`product_id`
    WHERE vas.`warehouse_id` = p_warehouse_id
      AND (p_active_only = 0 OR p.`active` = 1)
      AND (p_search = '' OR p.`name` LIKE CONCAT('%', p_search, '%') OR p.`sku` LIKE CONCAT('%', p_search, '%'));

    SELECT vas.`product_id`, vas.`sku`, vas.`name`, vas.`price_kopecks`,
           vas.`physical_qty`, vas.`reserved_quantity`, vas.`available_qty`,
           vas.`returnable`, vas.`capacity_units`
    FROM `v_available_stock` vas
    JOIN `products` p ON p.`id` = vas.`product_id`
    WHERE vas.`warehouse_id` = p_warehouse_id
      AND (p_active_only = 0 OR p.`active` = 1)
      AND (p_search = '' OR p.`name` LIKE CONCAT('%', p_search, '%') OR p.`sku` LIKE CONCAT('%', p_search, '%'))
    ORDER BY p.`name`
    LIMIT p_limit OFFSET v_offset;

    SELECT v_total AS `total_count`;
END$$

-- Доступное количество конкретного товара
CREATE PROCEDURE `sp_stock_get_available`(IN p_warehouse_id INT, IN p_product_id INT)
BEGIN
    SELECT `physical_qty`, `reserved_quantity`, `available_qty`, `min_quantity`
    FROM `v_available_stock`
    WHERE `warehouse_id` = p_warehouse_id AND `product_id` = p_product_id;
END$$

-- Проверка корзины перед заказом
CREATE PROCEDURE `sp_order_validate_cart`(
    IN p_warehouse_id INT,
    IN p_items_json JSON  -- [{product_id, qty}, ...]
)
BEGIN
    DECLARE v_idx INT DEFAULT 0;
    DECLARE v_count INT;
    DECLARE v_product_id INT;
    DECLARE v_qty INT;
    DECLARE v_available INT;

    SELECT JSON_LENGTH(p_items_json) INTO v_count;

    WHILE v_idx < v_count DO
        SET v_product_id = JSON_EXTRACT(p_items_json, CONCAT('$[', v_idx, '].product_id'));
        SET v_qty = JSON_EXTRACT(p_items_json, CONCAT('$[', v_idx, '].qty'));
        SELECT `available_qty` INTO v_available FROM `v_available_stock`
        WHERE `warehouse_id` = p_warehouse_id AND `product_id` = v_product_id;
        IF v_available IS NULL OR v_available < v_qty THEN
            SELECT `name` INTO @pname FROM `products` WHERE `id` = v_product_id;
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = CONCAT('Недостаточно товара «', @pname, '»: доступно ', v_available, ', запрошено ', v_qty);
        END IF;
        SET v_idx = v_idx + 1;
    END WHILE;
    SELECT 1 AS `ok`;
END$$

-- Резервирование товаров под заказ (вызывается из sp_order_create)
CREATE PROCEDURE `sp_stock_reserve`(
    IN p_warehouse_id INT,
    IN p_items_json JSON,
    IN p_order_id INT
)
BEGIN
    DECLARE v_idx INT DEFAULT 0;
    DECLARE v_count INT;
    DECLARE v_product_id INT;
    DECLARE v_qty INT;

    START TRANSACTION;

    SELECT JSON_LENGTH(p_items_json) INTO v_count;

    WHILE v_idx < v_count DO
        SET v_product_id = JSON_EXTRACT(p_items_json, CONCAT('$[', v_idx, '].product_id'));
        SET v_qty = JSON_EXTRACT(p_items_json, CONCAT('$[', v_idx, '].qty'));

        -- Блокируем строку остатка и проверяем
        SELECT `quantity`, `reserved_quantity` INTO @phys, @res
        FROM `warehouse_stock`
        WHERE `warehouse_id` = p_warehouse_id AND `product_id` = v_product_id
        FOR UPDATE;

        IF @phys IS NULL THEN
            ROLLBACK;
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Товар не найден на складе';
        END IF;

        IF @phys < @res + v_qty THEN
            ROLLBACK;
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Недостаточно физического остатка для резерва';
        END IF;

        UPDATE `warehouse_stock`
        SET `reserved_quantity` = `reserved_quantity` + v_qty
        WHERE `warehouse_id` = p_warehouse_id AND `product_id` = v_product_id;

        SET v_idx = v_idx + 1;
    END WHILE;

    COMMIT;
END$$

-- Освобождение резерва
CREATE PROCEDURE `sp_stock_release_reserve`(IN p_order_id INT)
BEGIN
    START TRANSACTION;

    UPDATE `warehouse_stock` ws
    JOIN `order_items` oi ON oi.`product_id` = ws.`product_id`
    SET ws.`reserved_quantity` = ws.`reserved_quantity` - oi.`quantity`
    WHERE oi.`order_id` = p_order_id
      AND ws.`warehouse_id` = (SELECT `warehouse_id` FROM `orders` WHERE `id` = p_order_id);

    COMMIT;
END$$

-- ============================================================
-- 5. ЗАКАЗЫ И АВТОНАЗНАЧЕНИЕ
-- ============================================================

-- Создание заказа клиентом
CREATE PROCEDURE `sp_order_create_client`(
    IN p_client_id INT,
    IN p_address_id INT,
    IN p_payment_method_id INT,
    IN p_comment TEXT,
    IN p_items_json JSON,
    OUT p_order_id INT,
    OUT p_status VARCHAR(20)
)
BEGIN
    DECLARE v_warehouse_id INT;
    DECLARE v_address_snapshot VARCHAR(500);
    DECLARE v_order_number VARCHAR(50);

    -- Получаем склад и снимок адреса
    SELECT ca.`warehouse_id`, CONCAT(ca.`city`, ', ', ca.`street`, ', ', ca.`building`,
           IFNULL(CONCAT(', кв. ', ca.`apartment`), ''))
    INTO v_warehouse_id, v_address_snapshot
    FROM `client_addresses` ca
    WHERE ca.`id` = p_address_id AND ca.`user_id` = p_client_id;

    IF v_warehouse_id IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Адрес не найден или не принадлежит клиенту';
    END IF;

    -- Проверка корзины
    CALL `sp_order_validate_cart`(v_warehouse_id, p_items_json);

    -- Генерируем номер заказа
    SET v_order_number = CONCAT('WR-', LPAD((SELECT COALESCE(MAX(CAST(SUBSTRING(`order_number`, 5) AS UNSIGNED)), 0) + 1 FROM `orders`), 4, '0'));

    START TRANSACTION;

    INSERT INTO `orders` (`order_number`, `client_id`, `created_by`, `delivery_date`, `delivery_address_id`,
                          `delivery_address_snapshot`, `warehouse_id`, `status`, `payment_method_id`, `comment`)
    VALUES (v_order_number, p_client_id, p_client_id, DATE_ADD(CURDATE(), INTERVAL 1 DAY),
            p_address_id, v_address_snapshot, v_warehouse_id, 'new', p_payment_method_id, p_comment);

    SET p_order_id = LAST_INSERT_ID();

    -- Позиции заказа
    INSERT INTO `order_items` (`order_id`, `product_id`, `quantity`, `unit_price_kopecks`)
    SELECT p_order_id, JSON_EXTRACT(item, '$.product_id'), JSON_EXTRACT(item, '$.qty'), pr.`price_kopecks`
    FROM JSON_TABLE(p_items_json, '$[*]' COLUMNS (
        product_id INT PATH '$.product_id',
        qty INT PATH '$.qty'
    )) AS jt
    JOIN `products` pr ON pr.`id` = jt.product_id;

    -- Резерв
    CALL `sp_stock_reserve`(v_warehouse_id, p_items_json, p_order_id);

    -- Попытка автоназначения
    CALL `sp_auto_assign_attempt`(p_order_id);

    SELECT `status` INTO p_status FROM `orders` WHERE `id` = p_order_id;

    COMMIT;
END$$

-- Создание заказа диспетчером
CREATE PROCEDURE `sp_order_create_dispatcher`(
    IN p_client_id INT,
    IN p_address_id INT,
    IN p_payment_method_id INT,
    IN p_comment TEXT,
    IN p_items_json JSON,
    OUT p_order_id INT,
    OUT p_status VARCHAR(20)
)
BEGIN
    -- Аналогично клиенту, но created_by = диспетчер (передавать отдельно при необходимости)
    CALL `sp_order_create_client`(p_client_id, p_address_id, p_payment_method_id, p_comment, p_items_json, p_order_id, p_status);
END$$

-- Повтор заказа
CREATE PROCEDURE `sp_order_repeat`(
    IN p_original_order_id INT,
    IN p_client_id INT,
    OUT p_order_id INT,
    OUT p_status VARCHAR(20)
)
BEGIN
    DECLARE v_items_json JSON;

    SELECT JSON_ARRAYAGG(JSON_OBJECT('product_id', `product_id`, 'qty', `quantity`))
    INTO v_items_json
    FROM `order_items` WHERE `order_id` = p_original_order_id;

    IF v_items_json IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'В исходном заказе нет позиций';
    END IF;

    -- Берём адрес по умолчанию клиента
    SELECT `id` INTO @def_addr FROM `client_addresses`
    WHERE `user_id` = p_client_id AND `is_default` = 1 AND `active` = 1 LIMIT 1;

    IF @def_addr IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'У клиента нет адреса по умолчанию';
    END IF;

    SELECT `comment`, `payment_method_id` INTO @comm, @pm FROM `orders` WHERE `id` = p_original_order_id;

    CALL `sp_order_create_client`(p_client_id, @def_addr, @pm, @comm, v_items_json, p_order_id, p_status);
END$$

-- Карточка заказа
CREATE PROCEDURE `sp_order_get`(IN p_order_id INT)
BEGIN
    SELECT * FROM `v_order_with_details` WHERE `id` = p_order_id;

    SELECT oi.`id`, oi.`product_id`, p.`sku`, p.`name`, oi.`quantity`, oi.`unit_price_kopecks`,
           (oi.`quantity` * oi.`unit_price_kopecks`) AS `total_kopecks`
    FROM `order_items` oi
    JOIN `products` p ON p.`id` = oi.`product_id`
    WHERE oi.`order_id` = p_order_id;
END$$

-- Список заказов
CREATE PROCEDURE `sp_order_list`(
    IN p_page INT, IN p_limit INT,
    IN p_search VARCHAR(255), IN p_status VARCHAR(20),
    IN p_date_from DATE, IN p_date_to DATE,
    IN p_warehouse_id INT, IN p_courier_id INT, IN p_client_id INT
)
BEGIN
    DECLARE v_offset INT DEFAULT (p_page - 1) * p_limit;
    DECLARE v_total INT;

    SELECT COUNT(*) INTO v_total
    FROM `v_order_with_details` o
    LEFT JOIN `routes` r ON r.`id` = (SELECT `route_id` FROM `route_stops` WHERE `order_id` = o.`id` LIMIT 1)
    WHERE (p_search = '' OR o.`order_number` LIKE CONCAT('%', p_search, '%')
                          OR o.`client_name` LIKE CONCAT('%', p_search, '%'))
      AND (p_status = '' OR o.`status` = p_status)
      AND (p_date_from IS NULL OR o.`delivery_date` >= p_date_from)
      AND (p_date_to IS NULL OR o.`delivery_date` <= p_date_to)
      AND (p_warehouse_id = 0 OR o.`warehouse_id` = p_warehouse_id)
      AND (p_courier_id = 0 OR r.`courier_id` = p_courier_id)
      AND (p_client_id = 0 OR o.`client_id` = p_client_id);

    SELECT o.`id`, o.`order_number`, o.`client_name`, o.`client_phone`, o.`delivery_address_snapshot`,
           o.`warehouse_name`, o.`delivery_date`, o.`status`, o.`payment_method`, o.`created_at`, o.`comment`
    FROM `v_order_with_details` o
    LEFT JOIN `routes` r ON r.`id` = (SELECT `route_id` FROM `route_stops` WHERE `order_id` = o.`id` LIMIT 1)
    WHERE (p_search = '' OR o.`order_number` LIKE CONCAT('%', p_search, '%')
                          OR o.`client_name` LIKE CONCAT('%', p_search, '%'))
      AND (p_status = '' OR o.`status` = p_status)
      AND (p_date_from IS NULL OR o.`delivery_date` >= p_date_from)
      AND (p_date_to IS NULL OR o.`delivery_date` <= p_date_to)
      AND (p_warehouse_id = 0 OR o.`warehouse_id` = p_warehouse_id)
      AND (p_courier_id = 0 OR r.`courier_id` = p_courier_id)
      AND (p_client_id = 0 OR o.`client_id` = p_client_id)
    ORDER BY o.`created_at` DESC
    LIMIT p_limit OFFSET v_offset;

    SELECT v_total AS `total_count`;
END$$

-- История статусов
CREATE PROCEDURE `sp_order_status_history`(IN p_order_id INT)
BEGIN
    SELECT sh.`id`, sh.`old_status`, sh.`new_status`, sh.`changed_at`, sh.`note`,
           u.`full_name` AS `user_name`
    FROM `status_history` sh
    LEFT JOIN `users` u ON u.`id` = sh.`user_id`
    WHERE sh.`order_id` = p_order_id
    ORDER BY sh.`changed_at`;
END$$

-- Отмена заказа
CREATE PROCEDURE `sp_order_cancel`(
    IN p_order_id INT, IN p_reason VARCHAR(500), IN p_user_id INT
)
BEGIN
    DECLARE v_status VARCHAR(20);
    DECLARE v_warehouse_id INT;

    SELECT `status`, `warehouse_id` INTO v_status, v_warehouse_id FROM `orders` WHERE `id` = p_order_id;

    IF v_status IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Заказ не найден';
    END IF;

    IF v_status NOT IN ('new','planned') THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Отменить можно только новые или запланированные заказы';
    END IF;

    START TRANSACTION;

    -- Освобождаем резерв
    CALL `sp_stock_release_reserve`(p_order_id);

    -- Если был маршрут — отменяем его
    IF EXISTS (SELECT 1 FROM `route_stops` WHERE `order_id` = p_order_id) THEN
        CALL `sp_route_cancel`((SELECT `route_id` FROM `route_stops` WHERE `order_id` = p_order_id LIMIT 1), p_reason);
    END IF;

    UPDATE `orders` SET `status` = 'cancelled' WHERE `id` = p_order_id;

    INSERT INTO `status_history` (`order_id`, `user_id`, `old_status`, `new_status`, `note`)
    VALUES (p_order_id, p_user_id, v_status, 'cancelled', p_reason);

    COMMIT;
END$$

-- Изменение склада заказа
CREATE PROCEDURE `sp_order_change_warehouse`(
    IN p_order_id INT, IN p_new_warehouse_id INT, IN p_user_id INT
)
BEGIN
    DECLARE v_status VARCHAR(20);
    DECLARE v_old_warehouse_id INT;

    SELECT `status`, `warehouse_id` INTO v_status, v_old_warehouse_id FROM `orders` WHERE `id` = p_order_id;

    IF v_status IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Заказ не найден';
    END IF;

    IF v_status NOT IN ('new','planned') THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Менять склад можно только для новых/запланированных заказов';
    END IF;

    IF EXISTS (SELECT 1 FROM `route_stops` rs JOIN `routes` r ON r.`id` = rs.`route_id`
               WHERE rs.`order_id` = p_order_id AND r.`status` = 'in_progress') THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Маршрут уже в работе — нельзя менять склад';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM `warehouses` WHERE `id` = p_new_warehouse_id AND `active` = 1) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Новый склад не найден или неактивен';
    END IF;

    START TRANSACTION;

    -- Перенос резерва
    CALL `sp_stock_release_reserve`(p_order_id);

    -- Новый резерв на новом складе
    SELECT JSON_ARRAYAGG(JSON_OBJECT('product_id', `product_id`, 'qty', `quantity`)) INTO @items
    FROM `order_items` WHERE `order_id` = p_order_id;
    CALL `sp_stock_reserve`(p_new_warehouse_id, @items, p_order_id);

    UPDATE `orders` SET `warehouse_id` = p_new_warehouse_id WHERE `id` = p_order_id;

    -- Если был маршрут — отменяем, будет пересоздан при автоназначении
    IF EXISTS (SELECT 1 FROM `route_stops` WHERE `order_id` = p_order_id) THEN
        CALL `sp_route_cancel`((SELECT `route_id` FROM `route_stops` WHERE `order_id` = p_order_id LIMIT 1), 'Смена склада');
    END IF;

    -- Повторное автоназначение
    CALL `sp_auto_assign_attempt`(p_order_id);

    COMMIT;
END$$

-- ============================================================
-- 6. АВТОНАЗНАЧЕНИЕ (FIFO)
-- ============================================================

-- Найти свободного курьера
CREATE PROCEDURE `sp_resource_find_free_courier`(OUT p_courier_id INT)
BEGIN
    SELECT u.`id` INTO p_courier_id
    FROM `users` u
    JOIN `roles` r ON r.`id` = u.`role_id`
    WHERE r.`code` = 'courier' AND u.`active` = 1
      AND `fn_is_courier_free`(u.`id`) = 1
    ORDER BY u.`id` LIMIT 1;
END$$

-- Найти свободный автомобиль
CREATE PROCEDURE `sp_resource_find_free_vehicle`(
    IN p_required_capacity INT,
    OUT p_vehicle_id INT
)
BEGIN
    SELECT v.`id` INTO p_vehicle_id
    FROM `vehicles` v
    WHERE v.`active` = 1 AND v.`capacity_units` >= p_required_capacity
      AND `fn_is_vehicle_free`(v.`id`) = 1
    ORDER BY v.`capacity_units` ASC LIMIT 1;
END$$

-- Найти свободного диспетчера
CREATE PROCEDURE `sp_resource_find_free_dispatcher`(OUT p_dispatcher_id INT)
BEGIN
    SELECT u.`id` INTO p_dispatcher_id
    FROM `users` u
    JOIN `roles` r ON r.`id` = u.`role_id`
    WHERE r.`code` = 'dispatcher' AND u.`active` = 1
      AND `fn_is_dispatcher_free`(u.`id`) = 1
    ORDER BY u.`id` LIMIT 1;
END$$

-- Попытка назначить один заказ
CREATE PROCEDURE `sp_auto_assign_attempt`(IN p_order_id INT)
BEGIN
    DECLARE v_warehouse_id INT;
    DECLARE v_courier_id INT;
    DECLARE v_vehicle_id INT;
    DECLARE v_dispatcher_id INT;
    DECLARE v_capacity_units INT;
    DECLARE v_route_date DATE;
    DECLARE v_time_start TIME;
    DECLARE v_time_end TIME;

    SELECT `warehouse_id` INTO v_warehouse_id FROM `orders` WHERE `id` = p_order_id;

    -- Считаем требуемую вместимость
    SELECT SUM(oi.`quantity` * p.`capacity_units`) INTO v_capacity_units
    FROM `order_items` oi
    JOIN `products` p ON p.`id` = oi.`product_id`
    WHERE oi.`order_id` = p_order_id;

    -- Находим ресурсы
    CALL `sp_resource_find_free_courier`(v_courier_id);
    CALL `sp_resource_find_free_vehicle`(v_capacity_units, v_vehicle_id);
    CALL `sp_resource_find_free_dispatcher`(v_dispatcher_id);

    IF v_courier_id IS NULL OR v_vehicle_id IS NULL OR v_dispatcher_id IS NULL THEN
        -- Ресурсы недоступны — заказ остаётся new, создаём уведомление
        INSERT INTO `notifications` (`user_id`, `type`, `title`, `message`)
        SELECT v_dispatcher_id, 'auto_assign_failed', 'Недостаточно ресурсов',
               CONCAT('Заказ ', (SELECT `order_number` FROM `orders` WHERE `id` = p_order_id), ' не может быть назначен: нет свободных ресурсов')
        WHERE v_dispatcher_id IS NOT NULL;
        RETURN;
    END IF;

    -- Определяем дату и окно (ближайшее доступное)
    SELECT `delivery_date` INTO v_route_date FROM `orders` WHERE `id` = p_order_id;
    -- Просто берём первое окно 09-12 для простоты (в реальности — проверка занятости)
    SET v_time_start = '09:00:00', v_time_end = '12:00:00';

    -- Создаём маршрут
    CALL `sp_route_create`(p_order_id, v_courier_id, v_vehicle_id, v_dispatcher_id, v_warehouse_id, v_route_date, v_time_start, v_time_end);

    -- Меняем статус заказа
    UPDATE `orders` SET `status` = 'planned' WHERE `id` = p_order_id;
END$$

-- Обработка очереди FIFO
CREATE PROCEDURE `sp_auto_assign_process_queue`()
BEGIN
    DECLARE v_done INT DEFAULT 0;
    DECLARE v_order_id INT;
    DECLARE cur CURSOR FOR
        SELECT `id` FROM `orders`
        WHERE `status` = 'new'
        ORDER BY `created_at` LIMIT 100;
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET v_done = 1;

    OPEN cur;
    read_loop: LOOP
        FETCH cur INTO v_order_id;
        IF v_done THEN LEAVE read_loop; END IF;
        CALL `sp_auto_assign_attempt`(v_order_id);
        -- Если заказ не назначен (остался new), прерываем — ресурсы кончились
        IF (SELECT `status` FROM `orders` WHERE `id` = v_order_id) = 'new' THEN
            LEAVE read_loop;
        END IF;
    END LOOP;
    CLOSE cur;
END$$

-- Создание маршрута (внутренняя)
CREATE PROCEDURE `sp_route_create`(
    IN p_order_id INT,
    IN p_courier_id INT, IN p_vehicle_id INT, IN p_dispatcher_id INT,
    IN p_warehouse_id INT, IN p_route_date DATE,
    IN p_time_start TIME, IN p_time_end TIME,
    OUT p_route_id INT
)
BEGIN
    DECLARE v_route_number VARCHAR(50);

    SET v_route_number = CONCAT('R-', LPAD((SELECT COALESCE(MAX(CAST(SUBSTRING(`route_number`, 3) AS UNSIGNED)), 0) + 1 FROM `routes`), 3, '0'));

    INSERT INTO `routes` (`route_number`, `route_date`, `courier_id`, `vehicle_id`, `dispatcher_id`, `warehouse_id`, `status`)
    VALUES (v_route_number, p_route_date, p_courier_id, p_vehicle_id, p_dispatcher_id, p_warehouse_id, 'planned');

    SET p_route_id = LAST_INSERT_ID();

    INSERT INTO `route_stops` (`route_id`, `order_id`, `stop_number`, `planned_time_start`, `planned_time_end`, `status`)
    VALUES (p_route_id, p_order_id, 1, p_time_start, p_time_end, 'pending');

    UPDATE `orders` SET `status` = 'planned' WHERE `id` = p_order_id;
END$$

-- ============================================================
-- 7. МАРШРУТЫ И ДОСТАВКА
-- ============================================================

CREATE PROCEDURE `sp_route_get`(IN p_route_id INT)
BEGIN
    SELECT * FROM `v_route_with_details` WHERE `id` = p_route_id;
END$$

CREATE PROCEDURE `sp_route_list`(
    IN p_page INT, IN p_limit INT,
    IN p_status VARCHAR(20), IN p_date DATE,
    IN p_courier_id INT, IN p_warehouse_id INT, IN p_dispatcher_id INT
)
BEGIN
    DECLARE v_offset INT DEFAULT (p_page - 1) * p_limit;
    DECLARE v_total INT;

    SELECT COUNT(*) INTO v_total
    FROM `routes` r
    WHERE (p_status = '' OR r.`status` = p_status)
      AND (p_date IS NULL OR r.`route_date` = p_date)
      AND (p_courier_id = 0 OR r.`courier_id` = p_courier_id)
      AND (p_warehouse_id = 0 OR r.`warehouse_id` = p_warehouse_id)
      AND (p_dispatcher_id = 0 OR r.`dispatcher_id` = p_dispatcher_id);

    SELECT r.`id`, r.`route_number`, r.`route_date`, r.`status`,
           courier.`full_name` AS `courier_name`, v.`plate` AS `vehicle_plate`,
           disp.`full_name` AS `dispatcher_name`, wh.`name` AS `warehouse_name`,
           rs.`stop_number`, rs.`planned_time_start`, rs.`planned_time_end`, rs.`status` AS `stop_status`,
           o.`order_number`, o.`client_name`
    FROM `routes` r
    JOIN `users` courier ON courier.`id` = r.`courier_id`
    JOIN `vehicles` v ON v.`id` = r.`vehicle_id`
    JOIN `users` disp ON disp.`id` = r.`dispatcher_id`
    JOIN `warehouses` wh ON wh.`id` = r.`warehouse_id`
    LEFT JOIN `route_stops` rs ON rs.`route_id` = r.`id`
    LEFT JOIN `orders` o ON o.`id` = rs.`order_id`
    WHERE (p_status = '' OR r.`status` = p_status)
      AND (p_date IS NULL OR r.`route_date` = p_date)
      AND (p_courier_id = 0 OR r.`courier_id` = p_courier_id)
      AND (p_warehouse_id = 0 OR r.`warehouse_id` = p_warehouse_id)
      AND (p_dispatcher_id = 0 OR r.`dispatcher_id` = p_dispatcher_id)
    ORDER BY r.`route_date` DESC, r.`id` DESC
    LIMIT p_limit OFFSET v_offset;

    SELECT v_total AS `total_count`;
END$$

-- Изменение назначения (до planned)
CREATE PROCEDURE `sp_route_reassign`(
    IN p_route_id INT,
    IN p_courier_id INT, IN p_vehicle_id INT, IN p_dispatcher_id INT,
    IN p_time_start TIME, IN p_time_end TIME
)
BEGIN
    DECLARE v_status VARCHAR(20);
    DECLARE v_order_id INT;

    SELECT r.`status`, rs.`order_id` INTO v_status, v_order_id
    FROM `routes` r
    LEFT JOIN `route_stops` rs ON rs.`route_id` = r.`id`
    WHERE r.`id` = p_route_id;

    IF v_status <> 'planned' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Изменять назначение можно только для запланированных маршрутов';
    END IF;

    -- Проверка свободности новых ресурсов
    IF NOT `fn_is_courier_free`(p_courier_id) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Курьер занят';
    END IF;
    IF NOT `fn_is_vehicle_free`(p_vehicle_id) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Автомобиль занят';
    END IF;
    IF NOT `fn_is_dispatcher_free`(p_dispatcher_id) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Диспетчер занят';
    END IF;

    -- Проверка вместимости
    SELECT SUM(oi.`quantity` * p.`capacity_units`) INTO @cap
    FROM `order_items` oi
    JOIN `products` p ON p.`id` = oi.`product_id`
    WHERE oi.`order_id` = v_order_id;

    SELECT `capacity_units` INTO @vcap FROM `vehicles` WHERE `id` = p_vehicle_id;
    IF @cap > @vcap THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Заказ не помещается в автомобиль';
    END IF;

    UPDATE `routes`
    SET `courier_id` = p_courier_id, `vehicle_id` = p_vehicle_id,
        `dispatcher_id` = p_dispatcher_id
    WHERE `id` = p_route_id;

    UPDATE `route_stops`
    SET `planned_time_start` = p_time_start, `planned_time_end` = p_time_end
    WHERE `route_id` = p_route_id;
END$$

-- Начало маршрута курьером
CREATE PROCEDURE `sp_route_start`(IN p_route_id INT, IN p_courier_id INT)
BEGIN
    DECLARE v_status VARCHAR(20);

    SELECT `status` INTO v_status FROM `routes` WHERE `id` = p_route_id AND `courier_id` = p_courier_id;

    IF v_status IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Маршрут не найден или не ваш';
    END IF;
    IF v_status <> 'planned' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Маршрут уже начат или завершён';
    END IF;

    UPDATE `routes` SET `status` = 'in_progress' WHERE `id` = p_route_id;
    UPDATE `route_stops` SET `status` = 'pending' WHERE `route_id` = p_route_id;

    UPDATE `orders` o
    JOIN `route_stops` rs ON rs.`order_id` = o.`id`
    SET o.`status` = 'in_delivery'
    WHERE rs.`route_id` = p_route_id;
END$$

-- Отметка прибытия
CREATE PROCEDURE `sp_route_arrive`(IN p_route_id INT, IN p_courier_id INT)
BEGIN
    DECLARE v_status VARCHAR(20);

    SELECT `status` INTO v_status FROM `routes` WHERE `id` = p_route_id AND `courier_id` = p_courier_id;

    IF v_status <> 'in_progress' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Маршрут не в работе';
    END IF;

    UPDATE `route_stops` SET `status` = 'arrived', `actual_arrival_time` = NOW() WHERE `route_id` = p_route_id;
END$$

-- Полная доставка (всё в одной транзакции)
CREATE PROCEDURE `sp_delivery_complete`(
    IN p_route_id INT, IN p_courier_id INT,
    IN p_delivered_items_json JSON,  -- [{order_item_id, qty_delivered}, ...]
    IN p_returned_containers_json JSON, -- [{water_type_id, qty}, ...]
    IN p_comment TEXT
)
BEGIN
    DECLARE v_route_status VARCHAR(20);
    DECLARE v_order_id INT;
    DECLARE v_warehouse_id INT;
    DECLARE v_stop_id INT;
    DECLARE v_delivery_id INT;

    SELECT r.`status`, rs.`order_id`, r.`warehouse_id`, rs.`id`
    INTO v_route_status, v_order_id, v_warehouse_id, v_stop_id
    FROM `routes` r
    JOIN `route_stops` rs ON rs.`route_id` = r.`id`
    WHERE r.`id` = p_route_id AND r.`courier_id` = p_courier_id;

    IF v_route_status IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Маршрут не найден или не ваш';
    END IF;
    IF v_route_status <> 'in_progress' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Маршрут не в работе';
    END IF;

    START TRANSACTION;

    -- 1) Создаём запись доставки
    INSERT INTO `deliveries` (`stop_id`, `delivered_at`, `comment`)
    VALUES (v_stop_id, NOW(), p_comment);
    SET v_delivery_id = LAST_INSERT_ID();

    -- 2) Проверяем и записываем фактические позиции
    -- (триггер trg_delivery_qty_check проверит не превышение заказанного)
    INSERT INTO `delivery_items` (`delivery_id`, `order_item_id`, `quantity_delivered`)
    SELECT v_delivery_id, JSON_EXTRACT(item, '$.order_item_id'), JSON_EXTRACT(item, '$.qty_delivered')
    FROM JSON_TABLE(p_delivered_items_json, '$[*]' COLUMNS (
        order_item_id INT PATH '$.order_item_id',
        qty_delivered INT PATH '$.qty_delivered'
    )) AS jt;

    -- 3) Триггер trg_delivery_full_check проверит полную доставку при смене статуса точки

    -- 4) Списание физических остатков и снятие резерва
    UPDATE `warehouse_stock` ws
    JOIN `delivery_items` di ON di.`delivery_id` = v_delivery_id
    JOIN `order_items` oi ON oi.`id` = di.`order_item_id`
    SET ws.`quantity` = ws.`quantity` - di.`quantity_delivered`,
        ws.`reserved_quantity` = ws.`reserved_quantity` - di.`quantity_delivered`
    WHERE ws.`warehouse_id` = v_warehouse_id
      AND ws.`product_id` = oi.`product_id`;

    -- Журнал движений
    INSERT INTO `stock_movements` (`warehouse_id`, `product_id`, `order_id`, `kind`, `quantity`, `comment`)
    SELECT v_warehouse_id, oi.`product_id`, oi.`order_id`, 'issue', di.`quantity_delivered`,
           CONCAT('Доставка ', (SELECT `route_number` FROM `routes` WHERE `id` = p_route_id))
    FROM `delivery_items` di
    JOIN `order_items` oi ON oi.`id` = di.`order_item_id`
    WHERE di.`delivery_id` = v_delivery_id;

    -- 5) Выдача тары клиенту (если есть возвратные позиции)
    INSERT INTO `container_movements` (`warehouse_id`, `client_id`, `delivery_id`, `water_type_id`, `kind`, `quantity`)
    SELECT v_warehouse_id, o.`client_id`, v_delivery_id, p.`water_type_id`, 'issued', di.`quantity_delivered`
    FROM `delivery_items` di
    JOIN `order_items` oi ON oi.`id` = di.`order_item_id`
    JOIN `products` p ON p.`id` = oi.`product_id`
    JOIN `orders` o ON o.`id` = oi.`order_id`
    WHERE di.`delivery_id` = v_delivery_id AND p.`returnable` = 1;

    -- 6) Меняем статусы
    UPDATE `route_stops` SET `status` = 'delivered' WHERE `id` = v_stop_id;
    UPDATE `routes` SET `status` = 'completed' WHERE `id` = p_route_id;
    UPDATE `orders` SET `status` = 'delivered' WHERE `id` = v_order_id;

    -- 7) Освобождаем ресурсы (автоматически через проверку fn_*)
    -- 8) Запускаем обработку очереди
    CALL `sp_auto_assign_process_queue`();

    COMMIT;
END$$

-- Недоставка
CREATE PROCEDURE `sp_delivery_fail`(
    IN p_route_id INT, IN p_courier_id INT,
    IN p_reason_category VARCHAR(50), IN p_comment TEXT,
    IN p_returned_containers_json JSON
)
BEGIN
    DECLARE v_route_status VARCHAR(20);
    DECLARE v_order_id INT;
    DECLARE v_warehouse_id INT;
    DECLARE v_stop_id INT;
    DECLARE v_delivery_id INT;

    SELECT r.`status`, rs.`order_id`, r.`warehouse_id`, rs.`id`
    INTO v_route_status, v_order_id, v_warehouse_id, v_stop_id
    FROM `routes` r
    JOIN `route_stops` rs ON rs.`route_id` = r.`id`
    WHERE r.`id` = p_route_id AND r.`courier_id` = p_courier_id;

    IF v_route_status IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Маршрут не найден или не ваш';
    END IF;
    IF v_route_status <> 'in_progress' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Маршрут не в работе';
    END IF;

    START TRANSACTION;

    -- 1) Доставка с delivered_at = NULL
    INSERT INTO `deliveries` (`stop_id`, `delivered_at`, `comment`)
    VALUES (v_stop_id, NULL, p_comment);
    SET v_delivery_id = LAST_INSERT_ID();

    -- 2) Позиции — все 0
    INSERT INTO `delivery_items` (`delivery_id`, `order_item_id`, `quantity_delivered`)
    SELECT v_delivery_id, oi.`id`, 0
    FROM `order_items` oi WHERE oi.`order_id` = v_order_id;

    -- 3) Резерв НЕ снимается — заказ можно повторить

    -- 4) Возврат тары, если курьер забирает пустые бутыли
    IF p_returned_containers_json IS NOT NULL AND JSON_LENGTH(p_returned_containers_json) > 0 THEN
        INSERT INTO `container_movements` (`warehouse_id`, `client_id`, `delivery_id`, `water_type_id`, `kind`, `quantity`, `confirmation_status`)
        SELECT v_warehouse_id, o.`client_id`, v_delivery_id, JSON_EXTRACT(item, '$.water_type_id'), 'returned',
               JSON_EXTRACT(item, '$.qty'), 'pending'
        FROM JSON_TABLE(p_returned_containers_json, '$[*]' COLUMNS (
            water_type_id INT PATH '$.water_type_id', qty INT PATH '$.qty'
        )) AS jt
        JOIN `orders` o ON o.`id` = v_order_id;
    END IF;

    -- 5) Статусы
    UPDATE `route_stops` SET `status` = 'not_delivered' WHERE `id` = v_stop_id;
    UPDATE `routes` SET `status` = 'completed' WHERE `id` = p_route_id;
    UPDATE `orders` SET `status` = 'not_delivered' WHERE `id` = v_order_id;

    -- 6) Автоматическая проблема (триггер trg_problem_auto_on_fail)
    -- 7) Освобождаем ресурсы
    CALL `sp_auto_assign_process_queue`();

    COMMIT;
END$$

-- Повторная постановка в очередь после недоставки
CREATE PROCEDURE `sp_order_requeue_after_fail`(IN p_order_id INT, IN p_user_id INT)
BEGIN
    DECLARE v_status VARCHAR(20);
    SELECT `status` INTO v_status FROM `orders` WHERE `id` = p_order_id;
    IF v_status <> 'not_delivered' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Только недоставленные заказы можно вернуть в очередь';
    END IF;

    START TRANSACTION;
    UPDATE `orders` SET `status` = 'new' WHERE `id` = p_order_id;
    CALL `sp_auto_assign_attempt`(p_order_id);
    COMMIT;
END$$

-- Отмена маршрута диспетчером
CREATE PROCEDURE `sp_route_cancel`(IN p_route_id INT, IN p_reason VARCHAR(500))
BEGIN
    DECLARE v_status VARCHAR(20);
    DECLARE v_order_id INT;

    SELECT r.`status`, rs.`order_id` INTO v_status, v_order_id
    FROM `routes` r LEFT JOIN `route_stops` rs ON rs.`route_id` = r.`id`
    WHERE r.`id` = p_route_id;

    IF v_status IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Маршрут не найден';
    END IF;

    IF v_status = 'planned' THEN
        -- Отменяем заказ полностью
        CALL `sp_order_cancel`(v_order_id, p_reason, 0);
    ELSE
        -- in_progress или completed — только отменяем маршрут
        UPDATE `routes` SET `status` = 'cancelled' WHERE `id` = p_route_id;
        UPDATE `route_stops` SET `status` = 'not_delivered' WHERE `route_id` = p_route_id;
        -- Заказ остаётся с резервом, статус не меняем
    END IF;
END$$

-- ============================================================
-- 8. СКЛАДСКИЕ ОПЕРАЦИИ
-- ============================================================

CREATE PROCEDURE `sp_stock_list`(
    IN p_page INT, IN p_limit INT,
    IN p_warehouse_id INT, IN p_product_id INT, IN p_low_only TINYINT
)
BEGIN
    DECLARE v_offset INT DEFAULT (p_page - 1) * p_limit;
    DECLARE v_total INT;

    SELECT COUNT(*) INTO v_total
    FROM `v_available_stock` vas
    WHERE (p_warehouse_id = 0 OR vas.`warehouse_id` = p_warehouse_id)
      AND (p_product_id = 0 OR vas.`product_id` = p_product_id)
      AND (p_low_only = 0 OR vas.`available_qty` <= vas.`min_quantity`);

    SELECT vas.`warehouse_id`, w.`name` AS `warehouse_name`,
           vas.`product_id`, vas.`sku`, vas.`name`,
           vas.`physical_qty`, vas.`reserved_quantity`, vas.`available_qty`, vas.`min_quantity`,
           CASE WHEN vas.`available_qty` <= vas.`min_quantity` THEN 'Низкий остаток' ELSE 'Достаточно' END AS `status`
    FROM `v_available_stock` vas
    JOIN `warehouses` w ON w.`id` = vas.`warehouse_id`
    WHERE (p_warehouse_id = 0 OR vas.`warehouse_id` = p_warehouse_id)
      AND (p_product_id = 0 OR vas.`product_id` = p_product_id)
      AND (p_low_only = 0 OR vas.`available_qty` <= vas.`min_quantity`)
    ORDER BY vas.`warehouse_id`, vas.`name`
    LIMIT p_limit OFFSET v_offset;

    SELECT v_total AS `total_count`;
END$$

CREATE PROCEDURE `sp_stock_low_list`(IN p_warehouse_id INT)
BEGIN
    SELECT vas.`product_id`, vas.`sku`, vas.`name`, vas.`available_qty`, vas.`min_quantity`
    FROM `v_available_stock` vas
    WHERE vas.`available_qty` <= vas.`min_quantity`
      AND (p_warehouse_id = 0 OR vas.`warehouse_id` = p_warehouse_id)
    ORDER BY vas.`warehouse_id`, vas.`available_qty`;
END$$

-- Поступление
CREATE PROCEDURE `sp_stock_receipt`(
    IN p_warehouse_id INT, IN p_product_id INT,
    IN p_qty INT, IN p_comment VARCHAR(500)
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM `warehouses` WHERE `id` = p_warehouse_id AND `active` = 1) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Склад не найден';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM `products` WHERE `id` = p_product_id AND `active` = 1) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Товар не найден';
    END IF;

    START TRANSACTION;

    INSERT INTO `warehouse_stock` (`warehouse_id`, `product_id`, `quantity`, `reserved_quantity`, `min_quantity`)
    VALUES (p_warehouse_id, p_product_id, p_qty, 0, 0)
    ON DUPLICATE KEY UPDATE `quantity` = `quantity` + p_qty;

    INSERT INTO `stock_movements` (`warehouse_id`, `product_id`, `kind`, `quantity`, `comment`)
    VALUES (p_warehouse_id, p_product_id, 'receipt', p_qty, p_comment);

    COMMIT;
END$$

-- Корректировка
CREATE PROCEDURE `sp_stock_adjust`(
    IN p_warehouse_id INT, IN p_product_id INT,
    IN p_new_physical_qty INT, IN p_reason VARCHAR(500)
)
BEGIN
    DECLARE v_old_qty INT;

    SELECT `quantity` INTO v_old_qty FROM `warehouse_stock`
    WHERE `warehouse_id` = p_warehouse_id AND `product_id` = p_product_id;

    IF v_old_qty IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Товар на складе не найден';
    END IF;

    IF p_new_physical_qty < 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Количество не может быть отрицательным';
    END IF;

    IF p_new_physical_qty < (SELECT `reserved_quantity` FROM `warehouse_stock`
                             WHERE `warehouse_id` = p_warehouse_id AND `product_id` = p_product_id) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Нельзя снизить физический остаток ниже резерва';
    END IF;

    START TRANSACTION;

    UPDATE `warehouse_stock` SET `quantity` = p_new_physical_qty
    WHERE `warehouse_id` = p_warehouse_id AND `product_id` = p_product_id;

    IF p_new_physical_qty > v_old_qty THEN
        INSERT INTO `stock_movements` (`warehouse_id`, `product_id`, `kind`, `quantity`, `comment`)
        VALUES (p_warehouse_id, p_product_id, 'adjustment_in', p_new_physical_qty - v_old_qty, p_reason);
    ELSE
        INSERT INTO `stock_movements` (`warehouse_id`, `product_id`, `kind`, `quantity`, `comment`)
        VALUES (p_warehouse_id, p_product_id, 'adjustment_out', v_old_qty - p_new_physical_qty, p_reason);
    END IF;

    COMMIT;
END$$

-- Журнал движений
CREATE PROCEDURE `sp_stock_movement_list`(
    IN p_page INT, IN p_limit INT,
    IN p_warehouse_id INT, IN p_product_id INT,
    IN p_kind VARCHAR(20), IN p_date_from DATE, IN p_date_to DATE
)
BEGIN
    DECLARE v_offset INT DEFAULT (p_page - 1) * p_limit;
    DECLARE v_total INT;

    SELECT COUNT(*) INTO v_total
    FROM `stock_movements` sm
    WHERE (p_warehouse_id = 0 OR sm.`warehouse_id` = p_warehouse_id)
      AND (p_product_id = 0 OR sm.`product_id` = p_product_id)
      AND (p_kind = '' OR sm.`kind` = p_kind)
      AND (p_date_from IS NULL OR sm.`occurred_at` >= p_date_from)
      AND (p_date_to IS NULL OR sm.`occurred_at` <= p_date_to);

    SELECT sm.`id`, w.`name` AS `warehouse`, p.`sku`, p.`name` AS `product`,
           sm.`kind`, sm.`quantity`, sm.`comment`, sm.`occurred_at`
    FROM `stock_movements` sm
    JOIN `warehouses` w ON w.`id` = sm.`warehouse_id`
    JOIN `products` p ON p.`id` = sm.`product_id`
    WHERE (p_warehouse_id = 0 OR sm.`warehouse_id` = p_warehouse_id)
      AND (p_product_id = 0 OR sm.`product_id` = p_product_id)
      AND (p_kind = '' OR sm.`kind` = p_kind)
      AND (p_date_from IS NULL OR sm.`occurred_at` >= p_date_from)
      AND (p_date_to IS NULL OR sm.`occurred_at` <= p_date_to)
    ORDER BY sm.`occurred_at` DESC
    LIMIT p_limit OFFSET v_offset;

    SELECT v_total AS `total_count`;
END$$

-- ============================================================
-- 9. ВОЗВРАТНАЯ ТАРА
-- ============================================================

-- Выдача тары при доставке (вызывается из sp_delivery_complete)
CREATE PROCEDURE `sp_container_movement_issue`(
    IN p_delivery_id INT,
    IN p_client_id INT,
    IN p_warehouse_id INT,
    IN p_items_json JSON  -- [{water_type_id, qty}, ...]
)
BEGIN
    DECLARE v_idx INT DEFAULT 0;
    DECLARE v_count INT;
    DECLARE v_water_type_id INT;
    DECLARE v_qty INT;

    SELECT JSON_LENGTH(p_items_json) INTO v_count;

    WHILE v_idx < v_count DO
        SET v_water_type_id = JSON_EXTRACT(p_items_json, CONCAT('$[', v_idx, '].water_type_id'));
        SET v_qty = JSON_EXTRACT(p_items_json, CONCAT('$[', v_idx, '].qty'));

        INSERT INTO `container_movements` (`warehouse_id`, `client_id`, `delivery_id`, `water_type_id`, `kind`, `quantity`)
        VALUES (p_warehouse_id, p_client_id, p_delivery_id, v_water_type_id, 'issued', v_qty);

        SET v_idx = v_idx + 1;
    END WHILE;
END$$

-- Курьер фиксирует возврат
CREATE PROCEDURE `sp_container_movement_return_by_courier`(
    IN p_delivery_id INT,
    IN p_items_json JSON  -- [{water_type_id, qty}, ...]
)
BEGIN
    DECLARE v_idx INT DEFAULT 0;
    DECLARE v_count INT;
    DECLARE v_water_type_id INT;
    DECLARE v_qty INT;
    DECLARE v_warehouse_id INT;
    DECLARE v_client_id INT;

    SELECT r.`warehouse_id`, o.`client_id` INTO v_warehouse_id, v_client_id
    FROM `deliveries` d
    JOIN `route_stops` rs ON rs.`id` = d.`stop_id`
    JOIN `routes` r ON r.`id` = rs.`route_id`
    JOIN `orders` o ON o.`id` = rs.`order_id`
    WHERE d.`id` = p_delivery_id;

    SELECT JSON_LENGTH(p_items_json) INTO v_count;

    WHILE v_idx < v_count DO
        SET v_water_type_id = JSON_EXTRACT(p_items_json, CONCAT('$[', v_idx, '].water_type_id'));
        SET v_qty = JSON_EXTRACT(p_items_json, CONCAT('$[', v_idx, '].qty'));

        INSERT INTO `container_movements` (`warehouse_id`, `client_id`, `delivery_id`, `water_type_id`, `kind`, `quantity`, `confirmation_status`)
        VALUES (v_warehouse_id, v_client_id, p_delivery_id, v_water_type_id, 'returned', v_qty, 'pending');

        SET v_idx = v_idx + 1;
    END WHILE;
END$$

-- Кладовщик подтверждает приёмку
CREATE PROCEDURE `sp_container_movement_confirm`(
    IN p_movement_id INT,
    IN p_confirmed_qty INT,
    IN p_storekeeper_id INT
)
BEGIN
    DECLARE v_qty INT;
    DECLARE v_status VARCHAR(20);

    SELECT `quantity`, `confirmation_status` INTO v_qty, v_status
    FROM `container_movements` WHERE `id` = p_movement_id;

    IF v_qty IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Движение тары не найдено';
    END IF;
    IF v_status = 'confirmed' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Уже подтверждено';
    END IF;
    IF p_confirmed_qty > v_qty THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Подтверждаемое количество не может превышать заявленное курьером';
    END IF;

    UPDATE `container_movements`
    SET `confirmation_status` = 'confirmed',
        `confirmed_by` = p_storekeeper_id,
        `confirmed_at` = NOW()
    WHERE `id` = p_movement_id;
END$$

-- Долг клиента по таре
CREATE PROCEDURE `sp_container_debt_get`(IN p_client_id INT)
BEGIN
    SELECT `client_id`, `water_type_id`, `water_type_name`,
           `issued_qty`, `confirmed_returned_qty`, `pending_returned_qty`, `current_debt`
    FROM `v_container_debt`
    WHERE `client_id` = p_client_id;
END$$

-- Ожидающие подтверждения
CREATE PROCEDURE `sp_container_pending_list`(IN p_page INT, IN p_limit INT)
BEGIN
    DECLARE v_offset INT DEFAULT (p_page - 1) * p_limit;
    DECLARE v_total INT;

    SELECT COUNT(*) INTO v_total
    FROM `container_movements`
    WHERE `confirmation_status` = 'pending';

    SELECT cm.`id`, cm.`warehouse_id`, w.`name` AS `warehouse`,
           cm.`client_id`, c.`full_name` AS `client_name`,
           cm.`water_type_id`, wt.`name` AS `water_type`,
           cm.`quantity`, cm.`occurred_at`
    FROM `container_movements` cm
    JOIN `warehouses` w ON w.`id` = cm.`warehouse_id`
    JOIN `users` c ON c.`id` = cm.`client_id`
    JOIN `water_types` wt ON wt.`id` = cm.`water_type_id`
    WHERE cm.`confirmation_status` = 'pending'
    ORDER BY cm.`occurred_at`
    LIMIT p_limit OFFSET v_offset;

    SELECT v_total AS `total_count`;
END$$

-- ============================================================
-- 10. ЧАТ, ПРОБЛЕМЫ, УВЕДОМЛЕНИЯ
-- ============================================================

CREATE PROCEDURE `sp_chat_get`(
    IN p_route_id INT, IN p_page INT, IN p_limit INT
)
BEGIN
    DECLARE v_offset INT DEFAULT (p_page - 1) * p_limit;
    DECLARE v_total INT;

    SELECT COUNT(*) INTO v_total
    FROM `notes`
    WHERE `type` = 'chat' AND `target_type` = 'route' AND `target_id` = p_route_id;

    SELECT n.`id`, n.`author_id`, a.`full_name` AS `author_name`,
           n.`recipient_id`, r.`full_name` AS `recipient_name`,
           n.`message`, n.`is_read`, n.`created_at`
    FROM `notes` n
    JOIN `users` a ON a.`id` = n.`author_id`
    LEFT JOIN `users` r ON r.`id` = n.`recipient_id`
    WHERE n.`type` = 'chat' AND n.`target_type` = 'route' AND n.`target_id` = p_route_id
    ORDER BY n.`created_at` DESC
    LIMIT p_limit OFFSET v_offset;

    SELECT v_total AS `total_count`;
END$$

CREATE PROCEDURE `sp_chat_send`(
    IN p_route_id INT, IN p_author_id INT, IN p_message TEXT
)
BEGIN
    DECLARE v_recipient_id INT;

    -- Определяем получателя (другой участник маршрута)
    SELECT CASE
        WHEN r.`courier_id` = p_author_id THEN r.`dispatcher_id`
        WHEN r.`dispatcher_id` = p_author_id THEN r.`courier_id`
        ELSE NULL
    END INTO v_recipient_id
    FROM `routes` r WHERE r.`id` = p_route_id;

    IF v_recipient_id IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Не удалось определить получателя';
    END IF;

    INSERT INTO `notes` (`author_id`, `type`, `target_type`, `target_id`, `recipient_id`, `message`)
    VALUES (p_author_id, 'chat', 'route', p_route_id, v_recipient_id, p_message);
END$$

CREATE PROCEDURE `sp_chat_mark_read`(IN p_route_id INT, IN p_user_id INT)
BEGIN
    UPDATE `notes`
    SET `is_read` = 1
    WHERE `type` = 'chat' AND `target_type` = 'route' AND `target_id` = p_route_id
      AND `recipient_id` = p_user_id AND `is_read` = 0;
END$$

CREATE PROCEDURE `sp_chat_unread_count`(IN p_user_id INT)
BEGIN
    SELECT COUNT(*) AS `unread_count`
    FROM `notes`
    WHERE `type` = 'chat' AND `recipient_id` = p_user_id AND `is_read` = 0;
END$$

CREATE PROCEDURE `sp_problem_create`(
    IN p_author_id INT,
    IN p_target_type ENUM('route','order','delivery'),
    IN p_target_id INT,
    IN p_category VARCHAR(50),
    IN p_description TEXT
)
BEGIN
    INSERT INTO `notes` (`author_id`, `type`, `target_type`, `target_id`, `category`, `problem_status`, `message`)
    VALUES (p_author_id, 'problem', p_target_type, p_target_id, p_category, 'open', p_description);
END$$

CREATE PROCEDURE `sp_problem_list`(
    IN p_page INT, IN p_limit INT,
    IN p_status VARCHAR(20), IN p_category VARCHAR(50),
    IN p_date_from DATE, IN p_date_to DATE
)
BEGIN
    DECLARE v_offset INT DEFAULT (p_page - 1) * p_limit;
    DECLARE v_total INT;

    SELECT COUNT(*) INTO v_total
    FROM `notes`
    WHERE `type` = 'problem'
      AND (p_status = '' OR `problem_status` = p_status)
      AND (p_category = '' OR `category` = p_category)
      AND (p_date_from IS NULL OR `created_at` >= p_date_from)
      AND (p_date_to IS NULL OR `created_at` <= p_date_to);

    SELECT n.`id`, n.`author_id`, a.`full_name` AS `author_name`,
           n.`target_type`, n.`target_id`, n.`category`, n.`problem_status`,
           n.`message`, n.`resolution_comment`, n.`resolved_by`,
           rb.`full_name` AS `resolved_by_name`, n.`resolved_at`, n.`created_at`
    FROM `notes` n
    JOIN `users` a ON a.`id` = n.`author_id`
    LEFT JOIN `users` rb ON rb.`id` = n.`resolved_by`
    WHERE n.`type` = 'problem'
      AND (p_status = '' OR n.`problem_status` = p_status)
      AND (p_category = '' OR n.`category` = p_category)
      AND (p_date_from IS NULL OR n.`created_at` >= p_date_from)
      AND (p_date_to IS NULL OR n.`created_at` <= p_date_to)
    ORDER BY n.`created_at` DESC
    LIMIT p_limit OFFSET v_offset;

    SELECT v_total AS `total_count`;
END$$

CREATE PROCEDURE `sp_problem_update_status`(
    IN p_problem_id INT,
    IN p_status ENUM('open','in_progress','resolved'),
    IN p_resolution_comment VARCHAR(1000),
    IN p_resolver_id INT
)
BEGIN
    DECLARE v_old_status VARCHAR(20);
    SELECT `problem_status` INTO v_old_status FROM `notes` WHERE `id` = p_problem_id AND `type` = 'problem';
    IF v_old_status IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Проблема не найдена';
    END IF;

    UPDATE `notes`
    SET `problem_status` = p_status,
        `resolution_comment` = p_resolution_comment,
        `resolved_by` = CASE WHEN p_status = 'resolved' THEN p_resolver_id ELSE NULL END,
        `resolved_at` = CASE WHEN p_status = 'resolved' THEN NOW() ELSE NULL END
    WHERE `id` = p_problem_id AND `type` = 'problem';
END$$

CREATE PROCEDURE `sp_notification_list`(
    IN p_user_id INT, IN p_page INT, IN p_limit INT, IN p_unread_only TINYINT
)
BEGIN
    DECLARE v_offset INT DEFAULT (p_page - 1) * p_limit;
    DECLARE v_total INT;

    SELECT COUNT(*) INTO v_total
    FROM `notifications`
    WHERE `user_id` = p_user_id AND (p_unread_only = 0 OR `is_read` = 0);

    SELECT `id`, `type`, `title`, `message`, `is_read`, `created_at`
    FROM `notifications`
    WHERE `user_id` = p_user_id AND (p_unread_only = 0 OR `is_read` = 0)
    ORDER BY `created_at` DESC
    LIMIT p_limit OFFSET v_offset;

    SELECT v_total AS `total_count`;
END$$

CREATE PROCEDURE `sp_notification_mark_read`(IN p_notification_id INT, IN p_user_id INT)
BEGIN
    UPDATE `notifications` SET `is_read` = 1 WHERE `id` = p_notification_id AND `user_id` = p_user_id;
END$$

-- ============================================================
-- 11. ОТЗЫВЫ
-- ============================================================

CREATE PROCEDURE `sp_review_upsert`(
    IN p_order_id INT, IN p_client_id INT,
    IN p_courier_rating TINYINT, IN p_courier_text VARCHAR(1000),
    IN p_order_rating TINYINT, IN p_order_text VARCHAR(1000)
)
BEGIN
    DECLARE v_status VARCHAR(20);
    SELECT `status` INTO v_status FROM `orders` WHERE `id` = p_order_id AND `client_id` = p_client_id;
    IF v_status IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Заказ не найден или не ваш';
    END IF;
    IF v_status <> 'delivered' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Отзыв можно оставить только после полной доставки';
    END IF;
    IF p_courier_rating IS NOT NULL AND (p_courier_rating < 1 OR p_courier_rating > 5) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Оценка курьера должна быть 1–5';
    END IF;
    IF p_order_rating IS NOT NULL AND (p_order_rating < 1 OR p_order_rating > 5) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Оценка заказа должна быть 1–5';
    END IF;

    INSERT INTO `reviews` (`order_id`, `client_id`, `courier_rating`, `courier_text`, `order_rating`, `order_text`)
    VALUES (p_order_id, p_client_id, p_courier_rating, p_courier_text, p_order_rating, p_order_text)
    ON DUPLICATE KEY UPDATE
        `courier_rating` = VALUES(`courier_rating`),
        `courier_text` = VALUES(`courier_text`),
        `order_rating` = VALUES(`order_rating`),
        `order_text` = VALUES(`order_text`),
        `updated_at` = NOW();
END$$

CREATE PROCEDURE `sp_review_get_courier`(IN p_courier_id INT)
BEGIN
    SELECT r.`id`, r.`order_id`, o.`order_number`, r.`client_id`, c.`full_name` AS `client_name`,
           r.`courier_rating`, r.`courier_text`, r.`order_rating`, r.`order_text`, r.`created_at`
    FROM `reviews` r
    JOIN `orders` o ON o.`id` = r.`order_id`
    JOIN `users` c ON c.`id` = r.`client_id`
    WHERE o.`courier_id` = p_courier_id  -- нужно добавить courier_id в orders или через route_stops
    ORDER BY r.`created_at` DESC;
END$$

-- Рейтинг курьера
CREATE PROCEDURE `sp_review_courier_rating`(IN p_courier_id INT)
BEGIN
    SELECT AVG(r.`courier_rating`) AS `avg_rating`, COUNT(*) AS `total_reviews`
    FROM `reviews` r
    JOIN `orders` o ON o.`id` = r.`order_id`
    JOIN `route_stops` rs ON rs.`order_id` = o.`id`
    JOIN `routes` rt ON rt.`id` = rs.`route_id`
    WHERE rt.`courier_id` = p_courier_id AND r.`courier_rating` IS NOT NULL;
END$$

-- Топ курьеров
CREATE PROCEDURE `sp_review_top_couriers`(IN p_limit INT)
BEGIN
    SELECT rt.`courier_id` AS `courier_id`, courier.`full_name` AS `courier_name`,
           COUNT(DISTINCT rt.`id`) AS `total_deliveries`,
           AVG(r.`courier_rating`) AS `avg_rating`,
           COUNT(r.`id`) AS `total_reviews`
    FROM `routes` rt
    JOIN `users` courier ON courier.`id` = rt.`courier_id`
    LEFT JOIN `route_stops` rs ON rs.`route_id` = rt.`id`
    LEFT JOIN `orders` o ON o.`id` = rs.`order_id`
    LEFT JOIN `reviews` r ON r.`order_id` = o.`id`
    WHERE rt.`status` = 'completed' AND o.`status` = 'delivered'
    GROUP BY rt.`courier_id`, courier.`full_name`
    HAVING COUNT(r.`id`) > 0
    ORDER BY `avg_rating` DESC, `total_deliveries` DESC
    LIMIT p_limit;
END$$

-- ============================================================
-- 12. DASHBOARD, КАЛЕНДАРЬ, МОНИТОР
-- ============================================================

CREATE PROCEDURE `sp_dashboard_metrics`(IN p_user_id INT)
BEGIN
    DECLARE v_role_code VARCHAR(50);
    SELECT r.`code` INTO v_role_code FROM `users` u JOIN `roles` r ON r.`id` = u.`role_id` WHERE u.`id` = p_user_id;

    CASE v_role_code
        WHEN 'admin' THEN
            SELECT (SELECT COUNT(*) FROM `users` WHERE `active` = 1) AS `active_users`,
                   (SELECT COUNT(*) FROM `orders` WHERE DATE(`created_at`) = CURDATE()) AS `orders_today`,
                   (SELECT COUNT(*) FROM `routes` WHERE `status` IN ('planned','in_progress')) AS `active_routes`,
                   (SELECT COUNT(*) FROM `notes` WHERE `type` = 'problem' AND `problem_status` = 'open') AS `open_problems`,
                   (SELECT COALESCE(SUM(oi.`quantity` * oi.`unit_price_kopecks`)/100, 0)
                    FROM `orders` o JOIN `order_items` oi ON oi.`order_id` = o.`id`
                    WHERE o.`status` = 'delivered' AND MONTH(o.`delivery_date`) = MONTH(CURDATE())) AS `revenue_month`;
        WHEN 'dispatcher' THEN
            SELECT (SELECT COUNT(*) FROM `orders` WHERE `status` = 'new') AS `new_orders`,
                   (SELECT COUNT(*) FROM `routes` WHERE `dispatcher_id` = p_user_id AND `status` = 'in_progress') AS `my_active_routes`,
                   (SELECT COUNT(*) FROM `routes` WHERE `status` = 'in_progress') AS `in_delivery`,
                   (SELECT COUNT(*) FROM `notes` WHERE `type` = 'problem' AND `problem_status` = 'open') AS `open_problems`,
                   (SELECT COUNT(*) FROM `v_available_stock` WHERE `available_qty` <= `min_quantity`) AS `low_stock`;
        WHEN 'storekeeper' THEN
            SELECT (SELECT COUNT(*) FROM `v_available_stock` WHERE `available_qty` <= `min_quantity`) AS `low_stock`,
                   (SELECT COUNT(*) FROM `stock_movements` WHERE `kind` = 'receipt' AND DATE(`occurred_at`) = CURDATE()) AS `receipts_today`,
                   (SELECT COUNT(*) FROM `container_movements` WHERE `confirmation_status` = 'pending') AS `pending_returns`,
                   (SELECT COUNT(*) FROM `stock_movements` WHERE DATE(`occurred_at`) = CURDATE()) AS `movements_today`,
                   (SELECT COALESCE(SUM(`current_debt`), 0) FROM `v_container_debt`) AS `container_debt`;
        WHEN 'courier' THEN
            SELECT (SELECT r.`id` FROM `routes` r WHERE r.`courier_id` = p_user_id AND r.`status` = 'in_progress' LIMIT 1) AS `current_route_id`,
                   (SELECT COUNT(*) FROM `routes` WHERE `courier_id` = p_user_id AND `status` = 'completed') AS `completed_deliveries`,
                   (SELECT COUNT(*) FROM `notes` WHERE `type` = 'problem' AND `target_type` = 'route' AND `target_id` IN (SELECT `id` FROM `routes` WHERE `courier_id` = p_user_id) AND `problem_status` = 'open') AS `open_problems`,
                   (SELECT COALESCE(AVG(r.`courier_rating`), 0) FROM `reviews` r JOIN `orders` o ON o.`id` = r.`order_id` JOIN `route_stops` rs ON rs.`order_id` = o.`id` JOIN `routes` rt ON rt.`id` = rs.`route_id` WHERE rt.`courier_id` = p_user_id) AS `avg_rating`,
                   (SELECT COUNT(*) FROM `notes` WHERE `type` = 'chat' AND `recipient_id` = p_user_id AND `is_read` = 0) AS `unread_messages`;
        WHEN 'client' THEN
            SELECT (SELECT COUNT(*) FROM `orders` WHERE `client_id` = p_user_id AND `status` IN ('new','planned','in_delivery')) AS `active_orders`,
                   (SELECT r.`id` FROM `routes` r JOIN `route_stops` rs ON rs.`route_id` = r.`id` JOIN `orders` o ON o.`id` = rs.`order_id` WHERE o.`client_id` = p_user_id AND r.`status` IN ('planned','in_progress') ORDER BY r.`route_date` LIMIT 1) AS `next_route_id`,
                   (SELECT COALESCE(SUM(`current_debt`), 0) FROM `v_container_debt` WHERE `client_id` = p_user_id) AS `container_debt`,
                   (SELECT COUNT(*) FROM `notifications` WHERE `user_id` = p_user_id AND `is_read` = 0) AS `unread_notifications`,
                   1 AS `can_order`; -- флаг что можно заказать
    END CASE;
END$$

CREATE PROCEDURE `sp_dashboard_events`(IN p_user_id INT, IN p_limit INT)
BEGIN
    SELECT `created_at` AS `time`,
           CASE
               WHEN `type` = 'order_status' THEN CONCAT('Заказ ', `message`)
               WHEN `type` = 'stock_low' THEN CONCAT('Склад: ', `message`)
               WHEN `type` = 'chat' THEN CONCAT('Чат: ', `message`)
               WHEN `type` = 'container_pending' THEN CONCAT('Тара: ', `message`)
               ELSE `message`
           END AS `event`,
           `type` AS `status`
    FROM `notifications`
    WHERE `user_id` = p_user_id
    ORDER BY `created_at` DESC
    LIMIT p_limit;
END$$

CREATE PROCEDURE `sp_calendar_get`(IN p_date_from DATE, IN p_date_to DATE)
BEGIN
    SELECT o.`id`, o.`order_number`, o.`client_name`, o.`delivery_date`,
           rs.`planned_time_start`, rs.`planned_time_end`, rs.`status` AS `stop_status`,
           r.`route_number`, r.`status` AS `route_status`,
           courier.`full_name` AS `courier_name`
    FROM `orders` o
    LEFT JOIN `route_stops` rs ON rs.`order_id` = o.`id`
    LEFT JOIN `routes` r ON r.`id` = rs.`route_id`
    LEFT JOIN `users` courier ON courier.`id` = r.`courier_id`
    WHERE o.`delivery_date` BETWEEN p_date_from AND p_date_to
    ORDER BY o.`delivery_date`, rs.`planned_time_start`;
END$$

CREATE PROCEDURE `sp_monitor_get`()
BEGIN
    -- Активные маршруты
    SELECT r.`id`, r.`route_number`, r.`route_date`, r.`status`,
           courier.`full_name` AS `courier_name`, v.`plate` AS `vehicle_plate`,
           rs.`order_id`, o.`order_number`, o.`client_name`,
           rs.`planned_time_start`, rs.`planned_time_end`, rs.`actual_arrival_time`, rs.`status` AS `stop_status`
    FROM `routes` r
    JOIN `users` courier ON courier.`id` = r.`courier_id`
    JOIN `vehicles` v ON v.`id` = r.`vehicle_id`
    LEFT JOIN `route_stops` rs ON rs.`route_id` = r.`id`
    LEFT JOIN `orders` o ON o.`id` = rs.`order_id`
    WHERE r.`status` IN ('planned','in_progress')
    ORDER BY r.`route_date`, rs.`planned_time_start`;

    -- Последние события
    SELECT `created_at`, `message` FROM `notifications` ORDER BY `created_at` DESC LIMIT 10;

    -- Открытые проблемы
    SELECT n.`id`, n.`target_type`, n.`target_id`, n.`category`, n.`problem_status`, n.`message`, n.`created_at`
    FROM `notes` n WHERE n.`type` = 'problem' AND n.`problem_status` = 'open' ORDER BY n.`created_at` DESC LIMIT 10;
END$$

-- ============================================================
-- 13. ОТЧЁТЫ
-- ============================================================

CREATE PROCEDURE `sp_report_orders_revenue`(
    IN p_date_from DATE, IN p_date_to DATE,
    IN p_warehouse_id INT, IN p_status VARCHAR(20)
)
BEGIN
    SELECT o.`id`, o.`order_number`, o.`client_name`, o.`delivery_date`, o.`status`,
           o.`warehouse_name`, o.`payment_method`,
           SUM(oi.`quantity` * oi.`unit_price_kopecks`) / 100 AS `total_rub`
    FROM `v_order_with_details` o
    JOIN `order_items` oi ON oi.`order_id` = o.`id`
    WHERE o.`delivery_date` BETWEEN p_date_from AND p_date_to
      AND (p_warehouse_id = 0 OR o.`warehouse_id` = p_warehouse_id)
      AND (p_status = '' OR o.`status` = p_status)
    GROUP BY o.`id`, o.`order_number`, o.`client_name`, o.`delivery_date`, o.`status`, o.`warehouse_name`, o.`payment_method`
    ORDER BY o.`delivery_date`;
END$$

CREATE PROCEDURE `sp_report_routes_deliveries`(
    IN p_date_from DATE, IN p_date_to DATE,
    IN p_courier_id INT
)
BEGIN
    SELECT r.`id`, r.`route_number`, r.`route_date`, r.`status`,
           courier.`full_name` AS `courier_name`, v.`plate` AS `vehicle_plate`,
           COUNT(rs.`id`) AS `stops_total`,
           SUM(CASE WHEN rs.`status` = 'delivered' THEN 1 ELSE 0 END) AS `delivered`,
           SUM(CASE WHEN rs.`status` = 'not_delivered' THEN 1 ELSE 0 END) AS `not_delivered`,
           SUM(CASE WHEN rs.`status` = 'pending' THEN 1 ELSE 0 END) AS `pending`
    FROM `routes` r
    JOIN `users` courier ON courier.`id` = r.`courier_id`
    JOIN `vehicles` v ON v.`id` = r.`vehicle_id`
    LEFT JOIN `route_stops` rs ON rs.`route_id` = r.`id`
    WHERE r.`route_date` BETWEEN p_date_from AND p_date_to
      AND (p_courier_id = 0 OR r.`courier_id` = p_courier_id)
    GROUP BY r.`id`, r.`route_number`, r.`route_date`, r.`status`, courier.`full_name`, v.`plate`
    ORDER BY r.`route_date`;
END$$

CREATE PROCEDURE `sp_report_stock_movements`(
    IN p_date_from DATE, IN p_date_to DATE,
    IN p_warehouse_id INT
)
BEGIN
    SELECT sm.`id`, w.`name` AS `warehouse`, p.`sku`, p.`name` AS `product`,
           sm.`kind`, sm.`quantity`, sm.`comment`, sm.`occurred_at`
    FROM `stock_movements` sm
    JOIN `warehouses` w ON w.`id` = sm.`warehouse_id`
    JOIN `products` p ON p.`id` = sm.`product_id`
    WHERE sm.`occurred_at` BETWEEN p_date_from AND p_date_to
      AND (p_warehouse_id = 0 OR sm.`warehouse_id` = p_warehouse_id)
    ORDER BY sm.`occurred_at`;
END$$

CREATE PROCEDURE `sp_report_container_turnover`(
    IN p_date_from DATE, IN p_date_to DATE
)
BEGIN
    SELECT wt.`name` AS `water_type`,
           SUM(CASE WHEN cm.`kind` = 'issued' THEN cm.`quantity` ELSE 0 END) AS `issued`,
           SUM(CASE WHEN cm.`kind` = 'returned' AND cm.`confirmation_status` = 'confirmed' THEN cm.`quantity` ELSE 0 END) AS `returned_confirmed`,
           SUM(CASE WHEN cm.`kind` = 'returned' AND cm.`confirmation_status` = 'pending' THEN cm.`quantity` ELSE 0 END) AS `pending`,
           (SUM(CASE WHEN cm.`kind` = 'issued' THEN cm.`quantity` ELSE 0 END) -
            SUM(CASE WHEN cm.`kind` = 'returned' AND cm.`confirmation_status` = 'confirmed' THEN cm.`quantity` ELSE 0 END)) AS `current_debt`
    FROM `container_movements` cm
    JOIN `water_types` wt ON wt.`id` = cm.`water_type_id`
    WHERE cm.`occurred_at` BETWEEN p_date_from AND p_date_to
    GROUP BY wt.`id`, wt.`name`;
END$$

CREATE PROCEDURE `sp_report_problems`(
    IN p_date_from DATE, IN p_date_to DATE,
    IN p_status VARCHAR(20)
)
BEGIN
    SELECT n.`id`, n.`target_type`, n.`target_id`, n.`category`, n.`problem_status`,
           n.`message`, n.`resolution_comment`, n.`resolved_at`, n.`created_at`
    FROM `notes` n
    WHERE n.`type` = 'problem'
      AND n.`created_at` BETWEEN p_date_from AND p_date_to
      AND (p_status = '' OR n.`problem_status` = p_status)
    ORDER BY n.`created_at`;
END$$

-- Данные для PDF
CREATE PROCEDURE `sp_pdf_order_card`(IN p_order_id INT)
BEGIN
    CALL `sp_order_get`(p_order_id);
END$$

CREATE PROCEDURE `sp_pdf_delivery_act`(IN p_delivery_id INT)
BEGIN
    SELECT d.`id`, d.`stop_id`, d.`delivered_at`, d.`comment`, d.`act_path`,
           rs.`route_id`, r.`route_number`, o.`order_number`, o.`client_name`, o.`delivery_address_snapshot`,
           o.`warehouse_name`, o.`delivery_date`
    FROM `deliveries` d
    JOIN `route_stops` rs ON rs.`id` = d.`stop_id`
    JOIN `routes` r ON r.`id` = rs.`route_id`
    JOIN `orders` o ON o.`id` = rs.`order_id`
    WHERE d.`id` = p_delivery_id;

    SELECT oi.`product_id`, p.`name`, di.`quantity_delivered`, p.`returnable`
    FROM `delivery_items` di
    JOIN `order_items` oi ON oi.`id` = di.`order_item_id`
    JOIN `products` p ON p.`id` = oi.`product_id`
    WHERE di.`delivery_id` = p_delivery_id;
END$$

CREATE PROCEDURE `sp_pdf_route_sheet`(IN p_route_id INT)
BEGIN
    SELECT r.`id`, r.`route_number`, r.`route_date`, r.`status`,
           courier.`full_name` AS `courier_name`, v.`plate` AS `vehicle_plate`,
           wh.`name` AS `warehouse_name`, disp.`full_name` AS `dispatcher_name`
    FROM `routes` r
    JOIN `users` courier ON courier.`id` = r.`courier_id`
    JOIN `vehicles` v ON v.`id` = r.`vehicle_id`
    JOIN `warehouses` wh ON wh.`id` = r.`warehouse_id`
    JOIN `users` disp ON disp.`id` = r.`dispatcher_id`
    WHERE r.`id` = p_route_id;

    SELECT rs.`stop_number`, rs.`planned_time_start`, rs.`planned_time_end`,
           o.`order_number`, o.`client_name`, o.`delivery_address_snapshot`,
           rs.`status` AS `stop_status`
    FROM `route_stops` rs
    JOIN `orders` o ON o.`id` = rs.`order_id`
    WHERE rs.`route_id` = p_route_id
    ORDER BY rs.`stop_number`;
END$$

-- ============================================================
-- 14. ИМПОРТ CSV
-- ============================================================

CREATE PROCEDURE `sp_import_products`(IN p_staging_table VARCHAR(64))
BEGIN
    DECLARE v_errors TEXT DEFAULT '';
    DECLARE v_row INT DEFAULT 0;
    DECLARE v_sku VARCHAR(50);
    DECLARE v_name VARCHAR(255);
    DECLARE v_water_type_id INT;
    DECLARE v_price INT;
    DECLARE v_returnable TINYINT;
    DECLARE v_capacity INT;
    DECLARE v_active TINYINT;
    DECLARE v_done INT DEFAULT 0;
    DECLARE cur CURSOR FOR SELECT `sku`, `name`, `water_type_id`, `price_kopecks`, `returnable`, `capacity_units`, `active` FROM `staging_products`;
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET v_done = 1;

    -- Валидация
    OPEN cur;
    read_loop: LOOP
        FETCH cur INTO v_sku, v_name, v_water_type_id, v_price, v_returnable, v_capacity, v_active;
        IF v_done THEN LEAVE read_loop; END IF;
        SET v_row = v_row + 1;
        IF v_sku IS NULL OR v_sku = '' THEN SET v_errors = CONCAT(v_errors, 'Строка ', v_row, ': пустой артикул; ');
        ELSEIF EXISTS (SELECT 1 FROM `products` WHERE `sku` = v_sku) THEN SET v_errors = CONCAT(v_errors, 'Строка ', v_row, ': артикул ', v_sku, ' уже существует; ');
        END IF;
        IF NOT EXISTS (SELECT 1 FROM `water_types` WHERE `id` = v_water_type_id) THEN SET v_errors = CONCAT(v_errors, 'Строка ', v_row, ': неверный water_type_id; ');
        END IF;
        IF v_price < 0 THEN SET v_errors = CONCAT(v_errors, 'Строка ', v_row, ': отрицательная цена; ');
        END IF;
        IF v_capacity <= 0 THEN SET v_errors = CONCAT(v_errors, 'Строка ', v_row, ': capacity_units <= 0; ');
        END IF;
    END LOOP;
    CLOSE cur;

    IF v_errors <> '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = v_errors;
    END IF;

    -- Импорт
    START TRANSACTION;
    INSERT INTO `products` (`sku`, `name`, `water_type_id`, `price_kopecks`, `returnable`, `capacity_units`, `active`)
    SELECT `sku`, `name`, `water_type_id`, `price_kopecks`, `returnable`, `capacity_units`, `active`
    FROM `staging_products`
    ON DUPLICATE KEY UPDATE
        `name` = VALUES(`name`), `water_type_id` = VALUES(`water_type_id`),
        `price_kopecks` = VALUES(`price_kopecks`), `returnable` = VALUES(`returnable`),
        `capacity_units` = VALUES(`capacity_units`), `active` = VALUES(`active`);
    COMMIT;
END$$

CREATE PROCEDURE `sp_import_stock_receipts`(IN p_staging_table VARCHAR(64))
BEGIN
    DECLARE v_errors TEXT DEFAULT '';
    DECLARE v_row INT DEFAULT 0;
    DECLARE v_warehouse_name VARCHAR(150);
    DECLARE v_product_sku VARCHAR(50);
    DECLARE v_qty INT;
    DECLARE v_comment VARCHAR(500);
    DECLARE v_warehouse_id INT;
    DECLARE v_product_id INT;
    DECLARE v_done INT DEFAULT 0;
    DECLARE cur CURSOR FOR SELECT `warehouse_name`, `product_sku`, `quantity`, `comment` FROM `staging_stock_receipts`;
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET v_done = 1;

    OPEN cur;
    read_loop: LOOP
        FETCH cur INTO v_warehouse_name, v_product_sku, v_qty, v_comment;
        IF v_done THEN LEAVE read_loop; END IF;
        SET v_row = v_row + 1;
        SELECT `id` INTO v_warehouse_id FROM `warehouses` WHERE `name` = v_warehouse_name AND `active` = 1;
        IF v_warehouse_id IS NULL THEN SET v_errors = CONCAT(v_errors, 'Строка ', v_row, ': склад не найден; ');
        END IF;
        SELECT `id` INTO v_product_id FROM `products` WHERE `sku` = v_product_sku AND `active` = 1;
        IF v_product_id IS NULL THEN SET v_errors = CONCAT(v_errors, 'Строка ', v_row, ': товар не найден; ');
        END IF;
        IF v_qty <= 0 THEN SET v_errors = CONCAT(v_errors, 'Строка ', v_row, ': количество <= 0; ');
        END IF;
    END LOOP;
    CLOSE cur;

    IF v_errors <> '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = v_errors;
    END IF;

    START TRANSACTION;
    INSERT INTO `warehouse_stock` (`warehouse_id`, `product_id`, `quantity`, `reserved_quantity`, `min_quantity`)
    SELECT w.`id`, p.`id`, s.`quantity`, 0, 0
    FROM `staging_stock_receipts` s
    JOIN `warehouses` w ON w.`name` = s.`warehouse_name`
    JOIN `products` p ON p.`sku` = s.`product_sku`
    ON DUPLICATE KEY UPDATE `quantity` = `quantity` + VALUES(`quantity`);

    INSERT INTO `stock_movements` (`warehouse_id`, `product_id`, `kind`, `quantity`, `comment`)
    SELECT w.`id`, p.`id`, 'receipt', s.`quantity`, s.`comment`
    FROM `staging_stock_receipts` s
    JOIN `warehouses` w ON w.`name` = s.`warehouse_name`
    JOIN `products` p ON p.`sku` = s.`product_sku`;
    COMMIT;
END$$

DELIMITER ;