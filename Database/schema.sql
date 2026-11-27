-- WaterRoute — MySQL 8.0 schema (исправленная версия под каталог операций)
-- Кодировка: utf8mb4, движок: InnoDB, FK: RESTRICT (каскады в коде/процедурах)
-- Запуск: mysql -u root -p < schema.sql

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS `notifications`, `reviews`, `notes`, `status_history`, `container_movements`,
    `delivery_items`, `deliveries`, `route_stops`, `routes`,
    `order_items`, `orders`, `client_addresses`,
    `warehouse_stock`, `stock_movements`, `products`, `water_types`,
    `vehicles`, `payment_methods`, `warehouses`, `users`, `roles`;

SET FOREIGN_KEY_CHECKS = 1;

-- ============================================================
-- 1. СПРАВОЧНИКИ
-- ============================================================

CREATE TABLE `roles` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `code` VARCHAR(50) NOT NULL UNIQUE,
    `name` VARCHAR(100) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `roles` (`code`, `name`) VALUES
    ('admin', 'Администратор'),
    ('dispatcher', 'Диспетчер'),
    ('storekeeper', 'Кладовщик'),
    ('courier', 'Курьер'),
    ('client', 'Клиент');

CREATE TABLE `water_types` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(150) NOT NULL UNIQUE,
    `volume_l` DECIMAL(5,2) NOT NULL DEFAULT 0,
    `active` TINYINT NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `water_types` (`name`, `volume_l`) VALUES
    ('Бутыль 19 л', 19.00),
    ('ПЭТ 5 л', 5.00),
    ('Без тары', 0.00);

CREATE TABLE `payment_methods` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `code` VARCHAR(50) NOT NULL UNIQUE,
    `name` VARCHAR(100) NOT NULL,
    `active` TINYINT NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `payment_methods` (`code`, `name`) VALUES
    ('cash', 'Наличные'),
    ('card', 'Карта'),
    ('online', 'Онлайн'),
    ('transfer', 'Безнал');

-- ============================================================
-- 2. ПОЛЬЗОВАТЕЛИ (единая таблица для всех ролей)
-- ============================================================

CREATE TABLE `users` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `role_id` INT NOT NULL,
    `full_name` VARCHAR(255) NOT NULL,
    `email` VARCHAR(255) NOT NULL UNIQUE,
    `phone` VARCHAR(50) NULL,
    `password_hash` VARCHAR(255) NOT NULL,
    `active` TINYINT NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `fk_users_role` FOREIGN KEY (`role_id`) REFERENCES `roles`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE INDEX `idx_users_role_active` ON `users`(`role_id`, `active`);

-- ============================================================
-- 3. СКЛАДЫ
-- ============================================================

CREATE TABLE `warehouses` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(150) NOT NULL UNIQUE,
    `address` VARCHAR(500) NOT NULL,
    `latitude` DECIMAL(10,6) NULL,
    `longitude` DECIMAL(10,6) NULL,
    `active` TINYINT NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================
-- 4. КЛИЕНТЫ И АДРЕСА
-- ============================================================

CREATE TABLE `client_addresses` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `user_id` INT NOT NULL,
    `warehouse_id` INT NOT NULL,
    `city` VARCHAR(100) NOT NULL DEFAULT 'Санкт-Петербург',
    `street` VARCHAR(255) NOT NULL,
    `building` VARCHAR(20) NOT NULL,
    `apartment` VARCHAR(20) NULL,
    `latitude` DECIMAL(10,6) NULL,
    `longitude` DECIMAL(10,6) NULL,
    `is_default` TINYINT NOT NULL DEFAULT 0,
    `active` TINYINT NOT NULL DEFAULT 1,
    CONSTRAINT `fk_ca_user` FOREIGN KEY (`user_id`) REFERENCES `users`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_ca_warehouse` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE INDEX `idx_ca_user_active` ON `client_addresses`(`user_id`, `active`);
CREATE INDEX `idx_ca_warehouse` ON `client_addresses`(`warehouse_id`);

-- Один адрес по умолчанию на клиента (триггер ниже)

-- ============================================================
-- 5. ТОВАРЫ И ОСТАТКИ
-- ============================================================

CREATE TABLE `products` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `sku` VARCHAR(50) NOT NULL UNIQUE,
    `name` VARCHAR(255) NOT NULL,
    `water_type_id` INT NOT NULL,
    `price_kopecks` INT NOT NULL CHECK (`price_kopecks` >= 0),
    `returnable` TINYINT NOT NULL DEFAULT 0,
    `capacity_units` INT NOT NULL DEFAULT 1 CHECK (`capacity_units` > 0),
    `active` TINYINT NOT NULL DEFAULT 1,
    CONSTRAINT `fk_products_water_type` FOREIGN KEY (`water_type_id`) REFERENCES `water_types`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE INDEX `idx_products_active` ON `products`(`active`);

CREATE TABLE `warehouse_stock` (
    `warehouse_id` INT NOT NULL,
    `product_id` INT NOT NULL,
    `quantity` INT NOT NULL DEFAULT 0 CHECK (`quantity` >= 0),
    `reserved_quantity` INT NOT NULL DEFAULT 0 CHECK (`reserved_quantity` >= 0),
    `min_quantity` INT NOT NULL DEFAULT 0 CHECK (`min_quantity` >= 0),
    PRIMARY KEY (`warehouse_id`, `product_id`),
    CONSTRAINT `fk_ws_warehouse` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_ws_product` FOREIGN KEY (`product_id`) REFERENCES `products`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `chk_ws_reserved_not_exceed` CHECK (`reserved_quantity` <= `quantity`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Журнал складских движений
CREATE TABLE `stock_movements` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `warehouse_id` INT NOT NULL,
    `product_id` INT NOT NULL,
    `order_id` INT NULL,
    `kind` ENUM('receipt','issue','adjustment_in','adjustment_out') NOT NULL,
    `quantity` INT NOT NULL CHECK (`quantity` > 0),
    `comment` VARCHAR(500) NULL,
    `occurred_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `fk_sm_warehouse` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_sm_product` FOREIGN KEY (`product_id`) REFERENCES `products`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE INDEX `idx_sm_warehouse_date` ON `stock_movements`(`warehouse_id`, `occurred_at`);
CREATE INDEX `idx_sm_order` ON `stock_movements`(`order_id`);

-- ============================================================
-- 6. АВТОМОБИЛИ
-- ============================================================

CREATE TABLE `vehicles` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `plate` VARCHAR(20) NOT NULL UNIQUE,
    `model` VARCHAR(100) NOT NULL,
    `capacity_units` INT NOT NULL CHECK (`capacity_units` > 0),
    `active` TINYINT NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================
-- 7. ЗАКАЗЫ
-- ============================================================

CREATE TABLE `orders` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `order_number` VARCHAR(50) NOT NULL UNIQUE,
    `client_id` INT NOT NULL,
    `created_by` INT NOT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `delivery_date` DATE NOT NULL,
    `delivery_address_id` INT NULL,
    `delivery_address_snapshot` VARCHAR(500) NOT NULL,
    `warehouse_id` INT NOT NULL,
    `status` ENUM('new','planned','in_delivery','delivered','not_delivered','cancelled') NOT NULL DEFAULT 'new',
    `payment_method_id` INT NOT NULL,
    `comment` TEXT NULL,
    CONSTRAINT `fk_orders_client` FOREIGN KEY (`client_id`) REFERENCES `users`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_orders_creator` FOREIGN KEY (`created_by`) REFERENCES `users`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_orders_address` FOREIGN KEY (`delivery_address_id`) REFERENCES `client_addresses`(`id`) ON DELETE SET NULL,
    CONSTRAINT `fk_orders_warehouse` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_orders_payment` FOREIGN KEY (`payment_method_id`) REFERENCES `payment_methods`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `chk_orders_date` CHECK (`delivery_date` >= DATE(`created_at`))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE INDEX `idx_orders_date_status` ON `orders`(`delivery_date`, `status`);
CREATE INDEX `idx_orders_client_status` ON `orders`(`client_id`, `status`);
CREATE INDEX `idx_orders_warehouse_status` ON `orders`(`warehouse_id`, `status`);

CREATE TABLE `order_items` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `order_id` INT NOT NULL,
    `product_id` INT NOT NULL,
    `quantity` INT NOT NULL CHECK (`quantity` > 0),
    `unit_price_kopecks` INT NOT NULL CHECK (`unit_price_kopecks` >= 0),
    CONSTRAINT `fk_oi_order` FOREIGN KEY (`order_id`) REFERENCES `orders`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_oi_product` FOREIGN KEY (`product_id`) REFERENCES `products`(`id`) ON DELETE RESTRICT,
    UNIQUE KEY `uk_oi_order_product` (`order_id`, `product_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================
-- 8. МАРШРУТЫ И ДОСТАВКА
-- ============================================================

CREATE TABLE `routes` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `route_number` VARCHAR(50) NOT NULL UNIQUE,
    `route_date` DATE NOT NULL,
    `courier_id` INT NOT NULL,
    `vehicle_id` INT NOT NULL,
    `dispatcher_id` INT NOT NULL,
    `warehouse_id` INT NOT NULL,
    `status` ENUM('planned','in_progress','completed','cancelled') NOT NULL DEFAULT 'planned',
    CONSTRAINT `fk_routes_courier` FOREIGN KEY (`courier_id`) REFERENCES `users`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_routes_vehicle` FOREIGN KEY (`vehicle_id`) REFERENCES `vehicles`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_routes_dispatcher` FOREIGN KEY (`dispatcher_id`) REFERENCES `users`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_routes_warehouse` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Занятость ресурса ограничивается только активными маршрутами и проверяется процедурно.
CREATE INDEX `idx_routes_date_status` ON `routes`(`route_date`, `status`);
CREATE INDEX `idx_routes_courier_date` ON `routes`(`courier_id`, `route_date`);
CREATE INDEX `idx_routes_vehicle_date` ON `routes`(`vehicle_id`, `route_date`);
CREATE INDEX `idx_routes_dispatcher_date` ON `routes`(`dispatcher_id`, `route_date`);

CREATE TABLE `route_stops` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `route_id` INT NOT NULL,
    `order_id` INT NOT NULL,
    `stop_number` INT NOT NULL DEFAULT 1 CHECK (`stop_number` > 0),
    `planned_time_start` TIME NOT NULL,
    `planned_time_end` TIME NOT NULL,
    `actual_arrival_time` DATETIME NULL,
    `status` ENUM('pending','arrived','delivered','not_delivered') NOT NULL DEFAULT 'pending',
    CONSTRAINT `fk_rs_route` FOREIGN KEY (`route_id`) REFERENCES `routes`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_rs_order` FOREIGN KEY (`order_id`) REFERENCES `orders`(`id`) ON DELETE RESTRICT,
    UNIQUE KEY `uk_rs_route_stop` (`route_id`, `stop_number`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Единственная активная попытка доставки проверяется в sp_route_create.
CREATE INDEX `idx_rs_order` ON `route_stops`(`order_id`);

-- Фактическая доставка по точке
CREATE TABLE `deliveries` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `stop_id` INT NOT NULL UNIQUE,
    `delivered_at` DATETIME NULL,
    `comment` TEXT NULL,
    `act_path` VARCHAR(500) NULL,
    CONSTRAINT `fk_del_stop` FOREIGN KEY (`stop_id`) REFERENCES `route_stops`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Фактические переданные количества (только при полной доставке)
CREATE TABLE `delivery_items` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `delivery_id` INT NOT NULL,
    `order_item_id` INT NOT NULL,
    `quantity_delivered` INT NOT NULL CHECK (`quantity_delivered` >= 0),
    CONSTRAINT `fk_di_delivery` FOREIGN KEY (`delivery_id`) REFERENCES `deliveries`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_di_order_item` FOREIGN KEY (`order_item_id`) REFERENCES `order_items`(`id`) ON DELETE RESTRICT,
    UNIQUE KEY `uk_di_delivery_item` (`delivery_id`, `order_item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================
-- 9. ВОЗВРАТНАЯ ТАРА
-- ============================================================

CREATE TABLE `container_movements` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `warehouse_id` INT NOT NULL,
    `client_id` INT NOT NULL,
    `delivery_id` INT NULL,
    `water_type_id` INT NOT NULL,
    `kind` ENUM('issued','returned') NOT NULL,
    `quantity` INT NOT NULL CHECK (`quantity` > 0),
    `confirmation_status` ENUM('pending','confirmed') NOT NULL DEFAULT 'pending',
    `confirmed_by` INT NULL,
    `confirmed_at` DATETIME NULL,
    `comment` VARCHAR(500) NULL,
    `occurred_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `fk_cm_warehouse` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_cm_client` FOREIGN KEY (`client_id`) REFERENCES `users`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_cm_delivery` FOREIGN KEY (`delivery_id`) REFERENCES `deliveries`(`id`) ON DELETE SET NULL,
    CONSTRAINT `fk_cm_water_type` FOREIGN KEY (`water_type_id`) REFERENCES `water_types`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_cm_confirmed_by` FOREIGN KEY (`confirmed_by`) REFERENCES `users`(`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE INDEX `idx_cm_client_status` ON `container_movements`(`client_id`, `confirmation_status`);
CREATE INDEX `idx_cm_delivery` ON `container_movements`(`delivery_id`);

-- ============================================================
-- 10. ИСТОРИЯ СТАТУСОВ, ЧАТ/ПРОБЛЕМЫ/УВЕДОМЛЕНИЯ (notes), ОТЗЫВЫ
-- ============================================================

CREATE TABLE `status_history` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `order_id` INT NOT NULL,
    `user_id` INT NULL,
    `old_status` VARCHAR(20) NOT NULL,
    `new_status` VARCHAR(20) NOT NULL,
    `changed_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `note` VARCHAR(500) NULL,
    CONSTRAINT `fk_sh_order` FOREIGN KEY (`order_id`) REFERENCES `orders`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_sh_user` FOREIGN KEY (`user_id`) REFERENCES `users`(`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE INDEX `idx_sh_order_date` ON `status_history`(`order_id`, `changed_at`);

-- Универсальная таблица: заметки, чат по маршруту, проблемы
CREATE TABLE `notes` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `author_id` INT NOT NULL,
    `type` ENUM('note','chat','problem') NOT NULL DEFAULT 'note',
    `target_type` ENUM('route','order','delivery','user') NULL,   -- 'user' нужен триггерам аудита
    `target_id` INT NULL,
    `recipient_id` INT NULL,              -- для чата
    `category` VARCHAR(50) NULL,          -- для проблем
    `problem_status` ENUM('open','in_progress','resolved') NULL,
    `resolution_comment` VARCHAR(1000) NULL,
    `resolved_by` INT NULL,
    `resolved_at` DATETIME NULL,
    `message` TEXT NOT NULL,
    `is_read` TINYINT NOT NULL DEFAULT 0,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `fk_notes_author` FOREIGN KEY (`author_id`) REFERENCES `users`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_notes_recipient` FOREIGN KEY (`recipient_id`) REFERENCES `users`(`id`) ON DELETE SET NULL,
    CONSTRAINT `fk_notes_resolved_by` FOREIGN KEY (`resolved_by`) REFERENCES `users`(`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE INDEX `idx_notes_route_chat` ON `notes`(`target_type`, `target_id`, `type`);
CREATE INDEX `idx_notes_recipient_unread` ON `notes`(`recipient_id`, `is_read`, `type`);
CREATE INDEX `idx_notes_problem` ON `notes`(`type`, `problem_status`);

-- Отзывы (отдельно от notes)
CREATE TABLE `reviews` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `order_id` INT NOT NULL UNIQUE,
    `client_id` INT NOT NULL,
    `courier_rating` TINYINT NULL CHECK (`courier_rating` BETWEEN 1 AND 5),
    `courier_text` VARCHAR(1000) NULL,
    `order_rating` TINYINT NULL CHECK (`order_rating` BETWEEN 1 AND 5),
    `order_text` VARCHAR(1000) NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT `fk_reviews_order` FOREIGN KEY (`order_id`) REFERENCES `orders`(`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_reviews_client` FOREIGN KEY (`client_id`) REFERENCES `users`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE INDEX `idx_reviews_courier` ON `reviews`(`order_id`); -- JOIN orders для courier

-- Уведомления
CREATE TABLE `notifications` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `user_id` INT NOT NULL,
    `type` VARCHAR(50) NOT NULL,
    `title` VARCHAR(255) NOT NULL,
    `message` TEXT NOT NULL,
    `is_read` TINYINT NOT NULL DEFAULT 0,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `fk_notif_user` FOREIGN KEY (`user_id`) REFERENCES `users`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE INDEX `idx_notif_user_unread` ON `notifications`(`user_id`, `is_read`);

-- Триггеры вынесены в отдельный файл triggers.sql.

-- ============================================================
-- 12. ПРЕДСТАВЛЕНИЯ (views) для удобства отчётов
-- ============================================================

CREATE OR REPLACE VIEW `v_available_stock` AS
SELECT
    ws.`warehouse_id`,
    ws.`product_id`,
    p.`sku`,
    p.`name`,
    p.`price_kopecks`,
    p.`returnable`,
    p.`capacity_units`,
    ws.`quantity` AS `physical_qty`,
    ws.`reserved_quantity`,
    (ws.`quantity` - ws.`reserved_quantity`) AS `available_qty`,
    ws.`min_quantity`
FROM `warehouse_stock` ws
JOIN `products` p ON p.`id` = ws.`product_id`
WHERE p.`active` = 1;

CREATE OR REPLACE VIEW `v_order_with_details` AS
SELECT
    o.`id`,
    o.`order_number`,
    o.`client_id`,
    c.`full_name` AS `client_name`,
    c.`phone` AS `client_phone`,
    o.`delivery_address_snapshot`,
    o.`warehouse_id`,
    w.`name` AS `warehouse_name`,
    o.`delivery_date`,
    o.`status`,
    o.`payment_method_id`,
    pm.`name` AS `payment_method`,
    o.`created_at`,
    o.`comment`
FROM `orders` o
JOIN `users` c ON c.`id` = o.`client_id`
JOIN `warehouses` w ON w.`id` = o.`warehouse_id`
JOIN `payment_methods` pm ON pm.`id` = o.`payment_method_id`;

CREATE OR REPLACE VIEW `v_route_with_details` AS
SELECT
    r.`id`,
    r.`route_number`,
    r.`route_date`,
    r.`courier_id`,
    courier.`full_name` AS `courier_name`,
    r.`vehicle_id`,
    v.`plate` AS `vehicle_plate`,
    v.`capacity_units` AS `vehicle_capacity`,
    r.`dispatcher_id`,
    disp.`full_name` AS `dispatcher_name`,
    r.`warehouse_id`,
    wh.`name` AS `warehouse_name`,
    r.`status`,
    rs.`id` AS `stop_id`,
    rs.`order_id`,
    rs.`stop_number`,
    rs.`planned_time_start`,
    rs.`planned_time_end`,
    rs.`actual_arrival_time`,
    rs.`status` AS `stop_status`
FROM `routes` r
JOIN `users` courier ON courier.`id` = r.`courier_id`
JOIN `vehicles` v ON v.`id` = r.`vehicle_id`
JOIN `users` disp ON disp.`id` = r.`dispatcher_id`
JOIN `warehouses` wh ON wh.`id` = r.`warehouse_id`
LEFT JOIN `route_stops` rs ON rs.`route_id` = r.`id`;

CREATE OR REPLACE VIEW `v_container_debt` AS
SELECT
    cm.`client_id`,
    cm.`water_type_id`,
    wt.`name` AS `water_type_name`,
    SUM(CASE WHEN cm.`kind` = 'issued' THEN cm.`quantity` ELSE 0 END) AS `issued_qty`,
    SUM(CASE WHEN cm.`kind` = 'returned' AND cm.`confirmation_status` = 'confirmed' THEN cm.`quantity` ELSE 0 END) AS `confirmed_returned_qty`,
    SUM(CASE WHEN cm.`kind` = 'returned' AND cm.`confirmation_status` = 'pending' THEN cm.`quantity` ELSE 0 END) AS `pending_returned_qty`,
    (SUM(CASE WHEN cm.`kind` = 'issued' THEN cm.`quantity` ELSE 0 END) -
     SUM(CASE WHEN cm.`kind` = 'returned' AND cm.`confirmation_status` = 'confirmed' THEN cm.`quantity` ELSE 0 END)) AS `current_debt`
FROM `container_movements` cm
JOIN `water_types` wt ON wt.`id` = cm.`water_type_id`
GROUP BY cm.`client_id`, cm.`water_type_id`, wt.`name`;