# Создание БД

1. Создали БД **bsbd_kursach**
2. Создали таблицы, индексы и ограничения
   1. Таблицы: Клиент, Книга, Отзыв, Заказ, Тэг, Заказ-Книга, Книга-Тег
   2. Индексы: Клиент-Телефон, Книга-Название, Книга-Автор
   3. Ограничения: Телефон-10-цифр
3. Создали роль **owner** с правами securty, access, read, write и УЗ главного админа **bsbd_main_admin**
4. Создали роль **readonly** (select на все таблицы)
5. Создали роль **worker**:
   1. update/insert books
   2. u/i/delete books-tags
   3. u/i tags
   4. u/i clients
   5. u/i orders
   6. u/i/d orders-books
   7. i reviews
6. Создали роль **admin**:
   1. u/i/d books
   2. u/i/d books-tags
   3. u/i/d tags
   4. u/i/d clients
   5. i orders
   6. i orders-books
   7. u/i/d reviews

![](./assets/db-diagram.png "Диаграмма БД")

# Создание приложения

1. Создан клиент БД
2. Создан тестовый консольный UI

## В планах

1. Переписать LINQ запросы на SQL
2. Убрать логин админа с сервера, оставив его только в БД
3. Перевести добавление заказа и удаление пользователя из кода в триггеры
4. ГУЙ
5. Возможно попереносить всякие строки в appsettings.json