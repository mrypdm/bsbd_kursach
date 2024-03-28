create role bsbd_admin_role authorization bsbd_owner

grant delete on Books to bsbd_admin_role
grant delete on Tags to bsbd_admin_role
-- is this needed?
-- grant delete on Clients to bsbd_admin_role
-- grant delete on Orders to bsbd_admin_role
grant update, delete on Reviews to bsbd_admin_role
grant delete on OrdersToBooks to bsbd_admin_role

alter role bsbd_worker_role add member bsbd_admin_role
