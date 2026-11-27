# MAP.md — Карта критериев (Требование → Реализация)

Для защиты учебной практики: соответствие требованиям методички и разделов задания.

| № | Требование методички / задания | Где реализовано | Статус |
|---|---|---|---|
| **1. Архитектура и стек** |
| 1.1 | C# WinForms, .NET 10 | `WaterRoute.csproj`, `Program.cs` | ✅ |
| 1.2 | MySQL 8.0 | `Database/schema.sql`, `seed.sql` | ✅ |
| 1.3 | ADO.NET (MySqlConnector), только `CALL sp_...` | `Infrastructure/Repository.cs` | ✅ |
| 1.4 | Mapsui.WindowsForms + OpenStreetMap | `Infrastructure/MapService.cs`, `Controls/RouteMapControl.cs` | ✅ |
| 1.5 | PDF через Microsoft Print to PDF | `Services/ReportExportService.cs` | ✅ |
| 1.6 | BCrypt для паролей | `Services/AuthService.cs` | ✅ |
| 1.7 | DI / Microsoft.Extensions.Hosting | `Program.cs`, `Infrastructure/ServiceCollectionExtensions.cs` | ✅ |
| **2. База данных** |
| 2.1 | ≥ 16 таблиц, 3НФ | `schema.sql` (21 таблица) | ✅ |
| 2.2 | PK, FK с RESTRICT, каскады в коде | `schema.sql` + `procedures.sql` | ✅ |
| 2.3 | Уникальные ограничения (номер заказа, артикул, email, телефон) | `schema.sql` | ✅ |
| 2.4 | Пароли только bcrypt | `sp_auth_login`, `AuthService` | ✅ |
| 2.5 | `schema.sql`, `seed.sql` (5 ролей, 25 users, 3 склада, 6 товаров, 30 заказов, 8 маршрутов) | `seed.sql` | ✅ |
| 2.6 | README для БД | `Database/README.md` | ✅ |
| **3. Роли и доступ** |
| 3.1 | 5 ролей: admin, dispatcher, storekeeper, courier, client | `roles` таблица, `UserRole` enum | ✅ |
| 3.2 | Администратор: пользователи/роли/справочники/просмотр/отчёты | `Forms/UserManagementForm`, `DirectoryManagementForm`, `AdminReportsForm` | ✅ |
| 3.3 | Диспетчер: клиенты/адреса, заказы/очередь, планировщик, монитор, чат/проблемы | `DispatcherClientForm`, `DispatcherOrderForm`, `DispatcherRouteForm`, `DispatcherMonitorForm` | ✅ |
| 3.4 | Кладовщик: остатки/операции, тара (подтверждение/долги), импорт CSV, отчёты | `StorekeeperStockForm`, `StorekeeperContainersForm`, `StorekeeperImportForm`, `StockMovementForm` | ✅ |
| 3.5 | Курьер: маршруты, выполнение/недоставка, тара, чат, проблемы, отзывы | `CourierRoutesForm`, `CourierDeliveryForm`, `CourierChatForm`, `CourierProblemsForm`, `CourierReviewsForm` | ✅ |
| 3.5 | Клиент: профиль/адреса, каталог/корзина/заказ, история/повтор/отмена, карта/диспетчер, отзывы/PDF | `ClientProfileForm`, `CatalogPage`, `ClientOrderHistoryForm`, `ClientDeliveryTrackingForm`, `ReviewEditForm` | ✅ |
| 3.6 | Регистрация клиента / создание сотрудника админом | `RegistrationForm`, `UserManagementForm` | ✅ |
| 3.7 | Авто-вход (DPAPI), запоминание сессии | `SessionStore`, `AuthService` | ✅ |
| **4. Заказы, склады, остатки** |
| 4.1 | Адресная книга клиента, 1 адрес по умолчанию, снимок адреса в заказе | `client_addresses`, `orders.delivery_address_snapshot` | ✅ |
| 4.2 | Привязка адреса к складу, заказ = 1 склад | `orders.warehouse_id`, `client_addresses.warehouse_id` | ✅ |
| 4.3 | Резервирование: `available = physical - reserved` | `warehouse_stock.reserved_quantity`, `sp_stock_reserve` | ✅ |
| 4.4 | Недоступный товар не добавляется в корзину | `CatalogPage`, `sp_order_validate_cart` | ✅ |
| 4.5 | Резерв при заказе, снятие при отмене/доставке | `sp_stock_reserve`, `sp_stock_release_reserve` | ✅ |
| 4.6 | Статусы: new→planned→in_delivery→delivered/not_delivered/cancelled | `orders.status`, `sp_auto_assign_attempt`, `sp_delivery_complete/fail` | ✅ |
| 4.7 | Способ оплаты — справочник, без реальных платежей | `payment_methods`, `orders.payment_method_id` | ✅ |
| **5. Маршруты и доставка** |
| 5.1 | 1 заказ = 1 маршрут = 1 остановка | `routes`, `route_stops` (1 stop) | ✅ |
| 5.2 | Автоназначение FIFO: свободный курьер/авто/диспетчер | `sp_auto_assign_attempt`, `sp_resource_find_free_*` | ✅ |
| 5.3 | Пожелание клиента — произвольный комментарий, система выбирает окно | `orders.comment`, `sp_auto_assign_attempt` | ✅ |
| 5.4 | Полная доставка или недоставка без передачи (принцип «всё или ничего») | `sp_delivery_complete`, `sp_delivery_fail`, триггер `trg_delivery_full_check` | ✅ |
| 5.5 | Недоставка → проблема + резерв сохраняется + повтор в очередь | `sp_delivery_fail`, `trg_problem_auto_on_fail`, `sp_order_requeue_after_fail` | ✅ |
| 5.6 | Возврат тары: курьер фиксирует → кладовщик подтверждает | `container_movements`, `sp_container_movement_return_by_courier`, `sp_container_movement_confirm` | ✅ |
| 5.7 | Карта: онлайн OSM, маркеры склада/клиента, линия маршрута | `MapService`, `RouteMapControl`, `MapViewForm` | ✅ |
| 5.8 | При потере связи — окно с телефоном диспетчера | `OfflineCallForm` | ✅ |
| 5.9 | Чат по маршруту: курьер ↔ диспетчер, история | `ChatPage`, `sp_chat_get/send/mark_read` | ✅ |
| **6. Удаление, процедуры, триггеры** |
| 6.1 | Без `ON DELETE CASCADE`, проверки в коде/процедурах | `sp_order_cancel`, `sp_route_cancel_resources`, `sp_stock_release_reserve` | ✅ |
| 6.2 | Все операции через `CALL sp_...` | `Repository.cs`, все `sp_...` процедуры | ✅ |
| 6.3 | Документ операций БД | `Docs/DB_OPERATIONS.md` | ✅ |
| 6.4 | Триггеры: защита остатков, доставка, история, адрес по умолчанию, автопроблема, тара | `triggers.sql` (8 триггеров) | ✅ |
| **7. Отчёты, импорт, экспорт** |
| 7.1 | 6 отчётов через JOIN в процедурах | `sp_report_*` в `procedures.sql` | ✅ |
| 7.2 | Экспорт 1 CSV / 1 PDF на отчёт | `ReportExportService`, `ReportsPage`, `AdminReportsForm` | ✅ |
| 7.3 | PDF через Microsoft Print to PDF (без библиотек) | `ReportExportService.cs` | ✅ |
| 7.4 | CSV-импорт товаров и поступлений: all-or-nothing, шаблоны, отчёт об ошибках | `CsvImportService`, `CsvFileService`, `ImportPage`, `StorekeeperImportForm` | ✅ |
| **8. Интерфейс** |
| 8.1 | Главная с 4–5 плиток по ролям | `DashboardPage` | ✅ |
| 8.2 | Реестры: поиск/фильтр/сортировка/пагинация 20 | `RegistryPage` | ✅ |
| 8.3 | Календарь месяц/неделя, цветовая маркировка | `CalendarPage` | ✅ |
| 8.3 | Монитор с автообновлением 30 сек | `MonitorPage`, `_refreshTimer` | ✅ |
| 8.4 | Горячие клавиши: F5, Ctrl+F, Ctrl+N, Esc | `ProcessCmdKey` в `MainForm`, `DashboardPage`, `MonitorPage` | ✅ |
| 8.5 | Мин. 1180×720, адаптивная вёрстка | `MainForm.MinimumSize`, `TableLayoutPanel`/`Dock` | ✅ |
| 8.6 | Внутренние уведомления (Toast), центр уведомлений | `ToastNotificationService`, `NotificationsPage` | ✅ |
| 8.7 | Название WaterRoute, стиль из макетов | `MainForm.Text`, `UITheme`, макеты в Canva | ✅ |
| **9. Документация** |
| 17.1 | ER-диаграмма | `Database/ER.png` (из `schema.sql`) | ✅ |
| 17.2 | UML Use Case / Activity | `Database/UseCase.png`, `Activity.png` | ✅ |
| 17.3 | IDEF0 A-0 / A0 | `Database/IDEF0_A-0.png`, `IDEF0_A0.png` | ✅ |
| 17.4 | README с запуском, демо-учётки, ограничения | `README.md`, `Database/README.md` | ✅ |
| 17.5 | Карта критериев (этот файл) | `MAP.md` | ✅ |
| 17.6 | CHANGELOG / PLAN.md | `PLAN.md` | ✅ |

---

> **Примечание:** offline-карта, MBTiles, GPS, частичная доставка, REST API, мобильное приложение, JWT, реальные платежи, внешние уведомления — исключены из текущего этапа (решение от 26.11.2026).