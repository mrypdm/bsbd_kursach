create role bsbd_worker_role authorization bsbd_owner

grant insert on Books to bsbd_worker_role
grant update on Books (Title, Author, ReleaseDate, Count) to bsbd_worker_role
grant insert, delete on BooksToTags to bsbd_worker_role

grant insert on Clients to bsbd_worker_role
grant update on Clients (FirstName, LastName, Phone, Gender) to bsbd_worker_role

grant insert, update on Tags to bsbd_worker_role

grant insert on Reviews to bsbd_worker_role

grant insert on Orders to bsbd_worker_role
grant insert on OrdersToBooks to bsbd_worker_role

alter role bsbd_readonly_role add member bsbd_worker_role
