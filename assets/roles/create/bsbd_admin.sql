create role bsbd_admin_role authorization bsbd_owner

grant insert, update, delete on Books to bsbd_admin_role
grant insert, delete on BooksToTags to bsbd_admin_role

grant insert, update, delete on Clients to bsbd_admin_role

grant insert, update, delete on Tags to bsbd_admin_role

grant insert, update, delete on Reviews to bsbd_admin_role

grant insert on Orders to bsbd_admin_role
grant insert on OrdersToBooks to bsbd_admin_role

alter role bsbd_worker_role add member bsbd_admin_role
