# Каталог операций БД — WaterRoute

> Назначение: единый источник для написания `procedures.sql` и `triggers.sql`.  
> Все обращения к данным из C# выполняются **только через `CALL sp_...`** — никаких прямых `SELECT/INSERT/UPDATE/DELETE` в коде.  
> Параметры процедур имеют префикс `p_`, возвращаемые курсорные наборы — без префикса.  
> Ошибки валидации выбрасываются `SIGNAL SQLSTATE '45000'` с понятным текстом.  
> Транзакционные процедуры начинают и фиксируют/откатывают транзакцию внутри себя.  
> Роли: `admin`, `dispatcher`, `storekeeper`, `courier`, `client`. Проверка прав — в сервисах приложения, но критические ограничения дублируются в процедурах.  
> База: MySQL 8.0, движок InnoDB, `utf8mb4`, `FOREIGN_KEY_CHECKS=ON`.

---

## 1. Соглашения и общие правила

| Аспект | Правило |
|---|---|
| Именование процедур | `sp_<domain>_<action>` (например `sp_order_create`, `sp_stock_movement_issue`) |
| Входные параметры | `p_<name>` с явным типом, `IN`/`OUT`/`INOUT` |
| Выходные курсоры | Просто `SELECT ...` внутри процедуры |
| Ошибки | `SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '...'` |
| Транзакции | `START TRANSACTION;` ... `COMMIT;` / `ROLLBACK;` внутри процедуры |
| Пагинация | Параметры `p_page INT, p_limit INT`, возврат `total_count` через `OUT` параметр или отдельный `SELECT FOUND_ROWS()` |
| Пагинация списков | Все списковые процедуры поддерживают `p_page`, `p_limit`, `p_search`, `p_status` и т.д. |
| Сортировка | `ORDER BY` поле — параметр `p_sort_by`, направление — `p_sort_dir` (`ASC`/`DESC`) |
| Временная зона | Все `DATETIME` хранятся в UTC; приложение конвертирует при отображении |
| Проверка активности | Любая процедура, меняющая данные, проверяет `active = 1` у задействованных сущностей |

---

## 2. Статусные модели (для проверок в процедурах)

### 2.1 Заказ (`orders.status`)
```
new → planned → in_delivery → delivered
              ↘ cancelled
              ↘ not_delivered
```
- `new` — создан клиентом/диспетчером, товар не зарезервирован.
- `planned` — ресурсы назначены, резерв создан.
- `in_delivery` — курьер нажал «Начать маршрут» / «Прибыл».
- `delivered` — полная доставка, списано физическое кол-во и резерв.
- `not_delivered` — недоставка, резерв сохранён, создана проблема.
- `cancelled` — отмена, резерв освобождён.

### 2.2 Маршрут (`routes.status`)
```
planned → in_progress → completed
           ↘ cancelled
```
- `planned` — создан, ресурсы заняты.
- `in_progress` — курьер начал выполнение.
- `completed` — доставка завершена (delivered / not_delivered).
- `cancelled` — отменён диспетчером, ресурсы свободны.

### 2.3 Точка (`route_stops.status`)
```
pending → arrived → delivered
              ↘ not_delivered
```

---

## 3. Домен: Аутентификация и пользователи

| Операция | Процедура | Роли | Назначение | Основные `IN` параметры | Результат / проверки |
|---|---|---|---|---|---|
| Вход пользователя | `sp_auth_login` | все | Поиск по email, проверка bcrypt-хеша, активность | `p_email VARCHAR(255)`, `p_password_plain VARCHAR(255)` | `user_id, full_name, email, role_id, role_code` или ошибка «Неверные данные / пользователь неактивен» |
| Авто-вход по сохранённой сессии | `sp_auth_validate_session` | все | Проверка локального токена/хранящегося идентификатора | `p_user_id INT`, `p_session_token VARCHAR(255)` | Сохранённый токен = текущий → успех; иначе ошибка |
| Регистрация клиента | `sp_client_register` | public | Создание пользователя с ролью `client` | `p_full_name`, `p_email`, `p_phone`, `p_password_plain` | `user_id`, уникальность email/телефона, bcrypt-хеш |
| Создание сотрудника админом | `sp_user_create_employee` | admin | Создание пользователя любой роли, кроме client | `p_full_name`, `p_email`, `p_phone`, `p_role_code`, `p_password_plain` | `user_id`, роль существует, email уникален |
| Создание клиента диспетчером | `sp_user_create_client_by_dispatcher` | dispatcher | То же, роль фиксированно `client` | `p_full_name`, `p_email`, `p_phone`, `p_password_plain` | `user_id` |
| Смена пароля | `sp_user_change_password` | все | Смена пароля с проверкой текущего | `p_user_id`, `p_old_password`, `p_new_password` | Обновлённый bcrypt-хеш |
| Сброс пароля админом | `sp_user_reset_password` | admin | Установка нового пароля без старого | `p_user_id`, `p_new_password` | Обновлённый bcrypt-хеш |
| Деактивация / активация | `sp_user_set_active` | admin | `active = 0/1` | `p_user_id`, `p_active TINYINT` | Нельзя деактивировать самого себя |
| Получение профиля | `sp_user_get_profile` | own / admin | Данные текущего пользователя | `p_user_id` | Все поля кроме `password_hash` |
| Список пользователей (пагинация, фильтры) | `sp_user_list` | admin | Реестр с поиском | `p_page`, `p_limit`, `p_search`, `p_role_code`, `p_active` | `rows`, `total_count` |

---

## 4. Домен: Справочники (админ)

| Операция | Процедура | Роли | IN параметры | Проверки |
|---|---|---|---|---|
| CRUD способов оплаты | `sp_payment_method_crud` | admin | `p_action` (`CREATE/READ/UPDATE/DELETE`), `p_id`, `p_code`, `p_name`, `p_active` | `code` уникален, `DELETE` только если нет заказов |
| CRUD типов воды/тары | `sp_water_type_crud` | admin | `p_action`, `p_id`, `p_name`, `p_volume_l`, `p_active` | `name+volume` уникален |
| CRUD товаров | `sp_product_crud` | admin | `p_action`, `p_id`, `p_sku`, `p_name`, `p_water_type_id`, `p_price_kopecks`, `p_returnable`, `p_capacity_units`, `p_active` | `sku` уникален, цена ≥ 0, `capacity_units` > 0 |
| CRUD складов | `sp_warehouse_crud` | admin | `p_action`, `p_id`, `p_name`, `p_address`, `p_latitude`, `p_longitude`, `p_active` | `name` уникален |
| CRUD автомобилей | `sp_vehicle_crud` | admin | `p_action`, `p_id`, `p_plate`, `p_model`, `p_capacity_units`, `p_active` | `plate` уникален, вместимость > 0 |
| Получение активных справочников для форм | `sp_directories_get_active` | все | `p_type ENUM('payment','water_type','product','warehouse','vehicle')` | Только `active = 1` |

---

## 5. Домен: Клиенты и адреса

| Операция | Процедура | Роли | IN параметры | Проверки |
|---|---|---|---|---|
| Список клиентов (пагинация) | `sp_client_list` | dispatcher, admin | `p_page`, `p_limit`, `p_search`, `p_active` | Поиск по ФИО/телефону/email |
| Карточка клиента с адресами | `sp_client_get` | dispatcher, admin, own(client) | `p_client_id` | Клиент видит только себя |
| Создание клиента (диспетчер) | `sp_client_create` | dispatcher | `p_full_name`, `p_phone`, `p_email`, `p_note` | Телефон/email уникальны |
| Редактирование клиента | `sp_client_update` | dispatcher, admin | `p_client_id`, `p_full_name`, `p_phone`, `p_email`, `p_note`, `p_active` | Проверка уникальности при изменении |
| Деактивация клиента | `sp_client_deactivate` | dispatcher, admin | `p_client_id` | Только если нет активных заказов |
| Адреса клиента (список) | `sp_client_address_list` | dispatcher, client, admin | `p_client_id` | Клиент видит свои адреса |
| Добавление адреса | `sp_client_address_add` | dispatcher, client | `p_client_id`, `p_warehouse_id`, `p_city`, `p_street`, `p_building`, `p_apartment`, `p_latitude`, `p_longitude`, `p_is_default` | Склад существует, 1 адрес по умолчанию на клиента |
| Редактирование адреса | `sp_client_address_update` | dispatcher, client | `p_address_id`, `p_warehouse_id`, `p_city`, `p_street`, `p_building`, `p_apartment`, `p_latitude`, `p_longitude`, `p_is_default`, `p_active` | Использованные адреса нельзя удалять, только деактивировать |
| Установка адреса по умолчанию | `sp_client_address_set_default` | dispatcher, client | `p_client_id`, `p_address_id` | Снимает флаг у остальных |

---

## 6. Домен: Каталог, остатки и корзина (проверка перед заказом)

| Операция | Процедура | Роли | IN параметры | Результат / проверки |
|---|---|---|---|---|
| Каталог по складу | `sp_catalog_get` | client, dispatcher, storekeeper | `p_warehouse_id`, `p_page`, `p_limit`, `p_search`, `p_active_only` | `sku, name, price_kopecks, available_qty, returnable, capacity_units` |
| Доступное кол-во конкретного товара | `sp_stock_get_available` | client, dispatcher, storekeeper | `p_warehouse_id`, `p_product_id` | `physical_qty - reserved_qty` (≥ 0) |
| Проверка корзины целиком | `sp_order_validate_cart` | client, dispatcher | JSON/временная таблица `p_items (product_id, qty)` + `p_warehouse_id` | Успех / ошибка с именем товара, где `available < qty` |
| Резервирование товаров под заказ | `sp_stock_reserve` | (внутренняя, вызывается из `sp_order_create`) | `p_warehouse_id`, временная таблица `p_items`, `p_order_id` | Блокировка строк `warehouse_stock` (`SELECT ... FOR UPDATE`), уменьшение `available` через увеличение `reserved_qty`, проверка `physical_qty >= reserved_qty + new_qty` |
| Освобождение резерва | `sp_stock_release_reserve` | (внутренняя) | `p_order_id` | Уменьшение `reserved_qty` по позициям заказа |

---

## 7. Домен: Заказы и автоматическое распределение

| Операция | Процедура | Роли | IN параметры | Транзакция / проверки |
|---|---|---|---|---|
| Создание заказа клиентом | `sp_order_create_client` | client | `p_client_id`, `p_address_id`, `p_payment_method_id`, `p_comment`, временная таблица `p_items (product_id, qty)` | 1) Проверка корзины (`sp_order_validate_cart`). 2) Резерв (`sp_stock_reserve`). 3) `INSERT orders + order_items` с ценой на момент. 4) Попытка автоназначения (`sp_auto_assign_attempt`). 5) Возврат `order_id`, `status` (`planned` или `new` если ресурсов нет). |
| Создание заказа диспетчером | `sp_order_create_dispatcher` | dispatcher | `p_client_id`, `p_address_id`, `p_payment_method_id`, `p_comment`, `p_items` | Аналогично клиенту, но диспетчер может переопределить склад |
| Повтор заказа | `sp_order_repeat` | client, dispatcher | `p_original_order_id`, `p_client_id` (для диспетчера может отличаться) | Новая проверка остатков, новый резерв, новый заказ |
| Карточка заказа | `sp_order_get` | все (с проверкой прав) | `p_order_id` | Заказ, позиции, история статусов, адрес-снапшот, склад |
| Список заказов (пагинация, фильтры) | `sp_order_list` | по роли | `p_page`, `p_limit`, `p_search`, `p_status`, `p_date_from`, `p_date_to`, `p_warehouse_id`, `p_courier_id`, `p_client_id` (для клиента фиксируется свой) | `rows`, `total_count` |
| История статусов заказа | `sp_order_status_history` | все | `p_order_id` | `changed_at, old_status, new_status, user_id, note` |
| Отмена заказа (только new/planned) | `sp_order_cancel` | client (own new), dispatcher, admin | `p_order_id`, `p_reason` | 1) Проверка статуса. 2) Освобождение резерва. 3) `status = cancelled`. 4) Запись в историю. 5) Если был маршрут — отмена маршрута (`sp_route_cancel`). |
| Изменение склада заказа (до planned) | `sp_order_change_warehouse` | dispatcher, admin | `p_order_id`, `p_new_warehouse_id` | 1) Статус `new` или `planned` без начатого маршрута. 2) Перенос резерва `sp_stock_release_reserve` + `sp_stock_reserve` на новом. 3) Обновление `orders.warehouse_id`. 4) Повторное автоназначение. |
| Изменение оплаты/комментария | `sp_order_update_details` | dispatcher, admin | `p_order_id`, `p_payment_method_id`, `p_comment` | Только если статус позволяет |

---

## 8. Домен: Автоматическое назначение (FIFO-очередь)

| Операция | Процедура | Роли | IN параметры | Логика |
|---|---|---|---|---|
| Попытка назначить один заказ | `sp_auto_assign_attempt` | (внутренняя) | `p_order_id` | 1) Найти свободного курьера (`users.role='courier' AND active=1` без маршрутов `planned/in_progress`). 2) Свободный автомобиль (`vehicles.active=1` без маршрутов). 3) Свободного диспетчера (`role='dispatcher' AND active=1` без активных маршрутов). 4) Проверить вместимость авто: сумма `capacity_units` позиций заказа ≤ `vehicle.capacity_units`. 5) Если все найдены — `sp_route_create` + смена статуса заказа на `planned`. 6) Иначе — заказ остаётся `new`, создаётся уведомление. |
| Обработка очереди FIFO | `sp_auto_assign_process_queue` | (внутренняя, вызывается по событию) | — | `SELECT * FROM orders WHERE status='new' AND reserved_qty>0 ORDER BY created_at LIMIT 100`, для каждого вызывать `sp_auto_assign_attempt`. Останавливается при нехватке любого ресурса. |
| Найти свободного курьера | `sp_resource_find_free_courier` | (внутренняя) | — | `SELECT u.id FROM users u JOIN roles r ON u.role_id=r.id WHERE r.code='courier' AND u.active=1 AND NOT EXISTS (SELECT 1 FROM routes rt WHERE rt.courier_id=u.id AND rt.status IN ('planned','in_progress')) LIMIT 1` |
| Найти свободный автомобиль | `sp_resource_find_free_vehicle` | (внутренняя) | `p_required_capacity INT` | Аналогично, с проверкой `capacity_units >= p_required_capacity` |
| Найти свободного диспетчера | `sp_resource_find_free_dispatcher` | (внутренняя) | — | Аналогично для `role='dispatcher'` |

---

## 9. Домен: Маршруты и доставка

| Операция | Процедура | Роли | IN параметры | Проверки |
|---|---|---|---|---|
| Создание маршрута (внутренняя) | `sp_route_create` | (внутренняя) | `p_order_id`, `p_courier_id`, `p_vehicle_id`, `p_dispatcher_id`, `p_warehouse_id`, `p_route_date`, `p_time_window_start`, `p_time_window_end` | Все ресурсы свободны, вместимость ок, создаёт `routes` + `route_stops` (одна остановка, `stop_number=1`), статус `planned` |
| Карточка маршрута | `sp_route_get` | dispatcher, courier (own), admin | `p_route_id` | Маршрут, остановка, заказ, клиент, адрес, окно |
| Список маршрутов | `sp_route_list` | по роли | `p_page`, `p_limit`, `p_status`, `p_date`, `p_courier_id`, `p_warehouse_id`, `p_dispatcher_id` | Курьер видит только свои |
| Изменение назначения (до planned) | `sp_route_reassign` | dispatcher, admin | `p_route_id`, `p_courier_id`, `p_vehicle_id`, `p_dispatcher_id`, `p_time_window_start`, `p_time_window_end` | Статус `planned`, новые ресурсы свободны, вместимость ок |
| Начало маршрута курьером | `sp_route_start` | courier (own) | `p_route_id` | Статус `planned` → `in_progress`, `routes.status=in_progress`, `route_stops.status=pending` |
| Отметка прибытия | `sp_route_arrive` | courier (own) | `p_route_id` | `route_stops.status=arrived`, `actual_arrival_time=NOW()` |
| Полная доставка | `sp_delivery_complete` | courier (own) | `p_route_id`, временная таблица `p_delivered_items (order_item_id, qty_delivered)`, `p_returned_containers_json`, `p_comment` | **Всё в одной транзакции**: 1) Проверка `qty_delivered = ordered_qty` для каждой позиции. 2) `delivery` запись. 3) `delivery_items`. 4) `sp_stock_write_off` (физическое списание + снятие резерва). 5) `sp_container_movement_issue` (выдача тары клиенту). 6) `route_stops.status=delivered`, `routes.status=completed`. 7) Освобождение курьера/авто/диспетчера. 8) Запуск `sp_auto_assign_process_queue`. 9) Запись в `status_history`. |
| Недоставка | `sp_delivery_fail` | courier (own) | `p_route_id`, `p_reason_category`, `p_comment`, `p_returned_containers_json` | 1) `delivery` запись с `delivered_at=NULL`. 2) `delivery_items` — все `qty_delivered=0`. 3) **Резерв НЕ снимается**. 4) `route_stops.status=not_delivered`, `routes.status=completed`. 5) Обязательное создание проблемы `sp_problem_create` (авто). 6) Освобождение ресурсов. 7) Запуск очереди. |
| Повторная постановка в очередь после недоставки | `sp_order_requeue_after_fail` | dispatcher, admin | `p_order_id` | 1) Статус заказа `not_delivered`. 2) Резерв сохранён. 3) `status=new`. 4) Вызов `sp_auto_assign_attempt`. |
| Отмена маршрута диспетчером | `sp_route_cancel` | dispatcher, admin | `p_route_id`, `p_reason` | Если статус `planned` — отмена заказа (`sp_order_cancel`), ресурсы свободны. Если `in_progress` — только отмена маршрута, заказ остаётся с резервом, создаётся уведомление. |

---

## 10. Домен: Складские операции

| Операция | Процедура | Роли | IN параметры | Проверки |
|---|---|---|---|---|
| Список остатков | `sp_stock_list` | storekeeper, admin, dispatcher | `p_page`, `p_limit`, `p_warehouse_id`, `p_product_id`, `p_low_only` | `physical, reserved, available, min_qty` |
| Низкие остатки | `sp_stock_low_list` | storekeeper, admin | `p_warehouse_id` | `available <= min_qty` |
| Поступление | `sp_stock_receipt` | storekeeper | `p_warehouse_id`, `p_product_id`, `p_qty`, `p_comment` | `physical_qty += qty`, запись в `stock_movements(kind='receipt')` |
| Списание (внутреннее, не для ручного) | `sp_stock_write_off` | (внутренняя) | `p_warehouse_id`, `p_product_id`, `p_qty`, `p_order_id` | `physical_qty -= qty`, `reserved_qty -= qty`, `stock_movements(kind='issue')` |
| Корректировка | `sp_stock_adjust` | storekeeper, admin | `p_warehouse_id`, `p_product_id`, `p_new_physical_qty`, `p_reason` | Разница → `stock_movements(kind='adjustment_in/out')` |
| Журнал движений | `sp_stock_movement_list` | storekeeper, admin | `p_page`, `p_limit`, `p_warehouse_id`, `p_product_id`, `p_kind`, `p_date_from`, `p_date_to` | Все движения с остатком на момент |

---

## 11. Домен: Возвратная тара

| Операция | Процедура | Роли | IN параметры | Логика |
|---|---|---|---|---|
| Выдача тары при доставке (внутренняя) | `sp_container_movement_issue` | (внутренняя) | `p_delivery_id`, `p_client_id`, `p_warehouse_id`, JSON `p_items (water_type_id, qty)` | `container_movements(kind='issued')`, увеличение долга клиента |
| Фиксация возврата курьером | `sp_container_movement_return_by_courier` | courier | `p_delivery_id`, JSON `p_items (water_type_id, qty)` | `container_movements(kind='returned', confirmation_status='pending')` |
| Подтверждение приёмки кладовщиком | `sp_container_movement_confirm` | storekeeper | `p_movement_id`, `p_confirmed_qty` | `confirmation_status='confirmed'`, `confirmed_by`, `confirmed_at`, уменьшение долга клиента на `confirmed_qty` |
| Долг клиента по таре | `sp_container_debt_get` | client (own), dispatcher, admin | `p_client_id` | Сумма `issued - confirmed` по типам тары |
| Ожидающие подтверждения возвраты | `sp_container_pending_list` | storekeeper, admin | `p_page`, `p_limit` | `confirmation_status='pending'` |

---

## 12. Домен: Чат, проблемы, уведомления (расширение `notes`)

| Операция | Процедура | Роли | IN параметры | Примечания |
|---|---|---|---|---|
| История чата маршрута | `sp_chat_get` | courier (own), dispatcher (assigned), admin | `p_route_id`, `p_page`, `p_limit` | `type='chat'`, `target_type='route'` |
| Отправка сообщения | `sp_chat_send` | courier, dispatcher | `p_route_id`, `p_author_id`, `p_message` | `recipient_id` определяется автоматически (другой участник) |
| Отметка прочтения | `sp_chat_mark_read` | courier, dispatcher | `p_route_id`, `p_user_id` | `is_read=1` для сообщений до текущего момента |
| Количество непрочитанных | `sp_chat_unread_count` | courier, dispatcher | `p_user_id` | Для показа в навигации |
| Создание проблемы | `sp_problem_create` | courier, dispatcher | `p_author_id`, `p_target_type ENUM('route','order','delivery')`, `p_target_id`, `p_category`, `p_description` | `type='problem'`, `problem_status='open'` |
| Список проблем | `sp_problem_list` | dispatcher, admin | `p_page`, `p_limit`, `p_status`, `p_category`, `p_date_from`, `p_date_to` | Фильтры |
| Изменение статуса проблемы | `sp_problem_update_status` | dispatcher, admin | `p_problem_id`, `p_status ENUM('open','in_progress','resolved')`, `p_resolution_comment` | При `resolved` — `resolved_by`, `resolved_at` |
| Уведомления пользователя | `sp_notification_list` | все | `p_user_id`, `p_page`, `p_limit`, `p_unread_only` | `type IN ('chat','problem','stock_low','order_status',...)` |
| Отметка уведомления прочитанным | `sp_notification_mark_read` | все | `p_notification_id` | `is_read=1` |

---

## 13. Домен: Отзывы

| Операция | Процедура | Роли | IN параметры | Проверки |
|---|---|---|---|---|
| Создание/обновление отзывов заказа | `sp_review_upsert` | client (own) | `p_order_id`, `p_courier_rating`, `p_courier_text`, `p_order_rating`, `p_order_text` | Заказ `status='delivered'`, один отзыв на заказ, `1..5` |
| Получение отзывов курьера | `sp_review_get_courier` | courier (own), admin, dispatcher | `p_courier_id` | Все отзывы курьера с текстом |
| Рейтинг курьера (агрегат) | `sp_review_courier_rating` | courier, admin, dispatcher | `p_courier_id` | `avg_rating`, `total_reviews` |
| Топ курьеров | `sp_review_top_couriers` | admin, dispatcher | `p_limit` | `courier_id, full_name, avg_rating, total_deliveries, total_reviews` |

---

## 14. Домен: Dashboard, календарь, монитор, отчёты

| Операция | Процедура | Роли | IN параметры | Результат |
|---|---|---|---|---|
| Метрики главной | `sp_dashboard_metrics` | по роли | `p_user_id` | Набор показателей под роль (см. PLAN 2.1) |
| События для главной | `sp_dashboard_events` | по роли | `p_user_id`, `p_limit` | Последние 10–20 строк |
| Данные календаря | `sp_calendar_get` | dispatcher, admin | `p_date_from`, `p_date_to` | Заказы/маршруты за период с окнами |
| Данные монитора | `sp_monitor_get` | dispatcher, admin | — | Активные маршруты, последние события, открытые проблемы |
| Отчёт «Заказы и выручка» | `sp_report_orders_revenue` | admin, dispatcher | `p_date_from`, `p_date_to`, `p_warehouse_id`, `p_status` | JOIN orders+items+clients, группировка, суммы |
| Отчёт «Маршруты и недоставки» | `sp_report_routes_deliveries` | admin, dispatcher | `p_date_from`, `p_date_to`, `p_courier_id` | Статистика по маршрутам, courier, недоставки |
| Отчёт «Топ курьеров» | `sp_report_top_couriers` | admin, dispatcher | `p_date_from`, `p_date_to`, `p_limit` | Рейтинг + кол-во доставок |
| Отчёт «Склад и движения» | `sp_report_stock_movements` | admin, storekeeper | `p_date_from`, `p_date_to`, `p_warehouse_id` | Остатки, поступления, списания |
| Отчёт «Оборот тары» | `sp_report_container_turnover` | admin, storekeeper | `p_date_from`, `p_date_to` | Выдано/возвращено/долг по типам |
| Отчёт «Проблемы» | `sp_report_problems` | admin, dispatcher | `p_date_from`, `p_date_to`, `p_status` | Регистрация/закрытие/среднее время |
| PDF-карточка заказа | `sp_pdf_order_card` | все | `p_order_id` | Данные для генерации PDF |
| PDF-акт доставки | `sp_pdf_delivery_act` | courier, dispatcher, admin | `p_delivery_id` | Данные для генерации PDF |
| PDF-маршрутный лист | `sp_pdf_route_sheet` | courier, dispatcher, admin | `p_route_id` | Данные для генерации PDF |

---

## 15. Домен: Импорт CSV (all-or-nothing)

| Операция | Процедура | Роли | IN параметры | Логика |
|---|---|---|---|---|
| Импорт товаров | `sp_import_products` | admin, storekeeper | Временная таблица `p_staging (sku, name, water_type_id, price_kopecks, returnable, capacity_units, active)` | 1) Валидация всех строк: уникальность sku, FK water_type, price≥0, capacity>0. 2) При любой ошибке — `ROLLBACK`, возврат списка ошибок с номерами строк. 3) Иначе `INSERT ... ON DUPLICATE KEY UPDATE` для существующих, `INSERT` для новых. |
| Импорт складских поступлений | `sp_import_stock_receipts` | admin, storekeeper | Временная таблица `p_staging (warehouse_name, product_sku, qty, comment)` | 1) Проверка существования склада и товара, qty>0. 2) Ошибки с номерами строк. 3) Транзакционное проведение: `sp_stock_receipt` для каждой строки. |

---

## 16. Триггеры (только защита инвариантов и аудит)

| Триггер | Таблица | Событие | Действие |
|---|---|---|---|
| `trg_stock_no_negative` | `warehouse_stock` | BEFORE UPDATE | `IF NEW.quantity < 0 THEN SIGNAL ...` |
| `trg_stock_reserved_not_exceed_physical` | `warehouse_stock` | BEFORE UPDATE | `IF NEW.reserved_qty > NEW.quantity THEN SIGNAL ...` |
| `trg_delivery_qty_check` | `delivery_items` | BEFORE INSERT/UPDATE | `IF NEW.quantity_delivered > (SELECT quantity FROM order_items WHERE id=NEW.order_item_id) THEN SIGNAL ...` |
| `trg_delivery_full_check` | `delivery` | BEFORE INSERT (status='delivered') | Проверить, что для каждой позиции заказа есть `delivery_items` с полным количеством |
| `trg_status_history` | `orders` | AFTER UPDATE (status changed) | `INSERT INTO status_history (order_id, user_id, old_status, new_status, changed_at, note)` |
| `trg_container_debt_consistency` | `container_movements` | AFTER INSERT/UPDATE | Пересчёт долга клиента, если `confirmed` — проверка `confirmed_qty <= returned_qty` |
| `trg_problem_auto_on_fail` | `route_stops` | AFTER UPDATE (status='not_delivered') | Автоматический `INSERT INTO notes (type='problem', target_type='route', target_id=NEW.route_id, ...)` |

---

## 17. Контрольный список покрытия экранов

| Экран / сценарий | Процедуры, которые должны быть реализованы |
|---|---|
| Вход / авто-вход | `sp_auth_login`, `sp_auth_validate_session` |
| Регистрация клиента | `sp_client_register` |
| Админ: пользователи | `sp_user_list`, `sp_user_create_employee`, `sp_user_set_active`, `sp_user_reset_password`, `sp_user_get_profile` |
| Админ: справочники | `sp_directories_get_active`, `sp_payment_method_crud`, `sp_water_type_crud`, `sp_product_crud`, `sp_warehouse_crud`, `sp_vehicle_crud` |
| Диспетчер: клиенты | `sp_client_list`, `sp_client_get`, `sp_client_create`, `sp_client_update`, `sp_client_deactivate`, `sp_client_address_list`, `sp_client_address_add`, `sp_client_address_update`, `sp_client_address_set_default` |
| Клиент: профиль/адреса | `sp_user_get_profile`, `sp_client_address_list`, `sp_client_address_add`, `sp_client_address_update`, `sp_client_address_set_default`, `sp_container_debt_get` |
| Каталог/корзина | `sp_catalog_get`, `sp_stock_get_available`, `sp_order_validate_cart` |
| Оформление заказа | `sp_order_create_client`, `sp_order_create_dispatcher`, `sp_order_repeat` |
| Реестр заказов | `sp_order_list`, `sp_order_get`, `sp_order_status_history`, `sp_order_cancel`, `sp_order_change_warehouse`, `sp_order_update_details` |
| Маршруты/планировщик | `sp_route_list`, `sp_route_get`, `sp_route_reassign`, `sp_route_start`, `sp_route_arrive`, `sp_delivery_complete`, `sp_delivery_fail`, `sp_route_cancel`, `sp_order_requeue_after_fail` |
| Автоназначение | `sp_auto_assign_attempt`, `sp_auto_assign_process_queue`, `sp_resource_find_free_courier/vehicle/dispatcher` |
| Склад | `sp_stock_list`, `sp_stock_low_list`, `sp_stock_receipt`, `sp_stock_adjust`, `sp_stock_movement_list` |
| Тара | `sp_container_movement_return_by_courier`, `sp_container_movement_confirm`, `sp_container_debt_get`, `sp_container_pending_list` |
| Чат | `sp_chat_get`, `sp_chat_send`, `sp_chat_mark_read`, `sp_chat_unread_count` |
| Проблемы | `sp_problem_list`, `sp_problem_create`, `sp_problem_update_status` |
| Уведомления | `sp_notification_list`, `sp_notification_mark_read` |
| Отзывы | `sp_review_upsert`, `sp_review_get_courier`, `sp_review_courier_rating`, `sp_review_top_couriers` |
| Главная/дашборд | `sp_dashboard_metrics`, `sp_dashboard_events` |
| Календарь | `sp_calendar_get` |
| Монитор | `sp_monitor_get` |
| Отчёты | Все `sp_report_*` + PDF-процедуры |
| Импорт | `sp_import_products`, `sp_import_stock_receipts` |
| Карта/офлайн | Не требуют отдельных процедур (координаты в адресах/складах) |

---

## 18. Отложенные решения (требуют проверки пользователем)

1. **Точный состав временной таблицы для импорта** — нужен ли отдельный `staging` для каждого импорта или общий JSON-параметр?
2. **Параметр `p_time_window` в автоназначении** — выбирать ближайшее свободное окно или первое попавшееся?
3. **Максимальное число одновременных активных маршрутов у диспетчера** — сейчас 1 (строго), но в обсуждении был вариант Б (сколько может). Сейчас зафиксировано 1.
4. **Хранение координат** — `DECIMAL(10,6)` достаточно или нужен `POINT`/`GIS`?
5. **Тип тары в `container_movements`** — сейчас `water_type_id` (FK на `water_types`), но `water_types` содержит и воду. Может, отдельный справочник `container_types`?
6. **Уведомления: сохранять все или только непрочитанные** — таблица растёт, нужна политика очистки?
6. **Права на PDF** — курьер может экспортировать только свой маршрутный лист и акт своей доставки. Проверить в `sp_pdf_*` проверку прав.
7. **Индексы для пагинации** — составные `(status, created_at)`, `(warehouse_id, product_id)` и т.д. добавлять в `schema.sql` после утверждения списка процедур.

---
