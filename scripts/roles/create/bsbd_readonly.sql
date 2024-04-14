create role bsbd_readonly_role authorization bsbd_owner

grant select on Books to bsbd_readonly_role
grant select on Tags to bsbd_readonly_role
grant select on Clients to bsbd_readonly_role
grant select on Orders to bsbd_readonly_role
grant select on Reviews to bsbd_readonly_role
grant select on OrdersToBooks to bsbd_readonly_role
grant select on BooksToTags to bsbd_readonly_role
grant select on bsbd_principals to bsbd_readonly_role
grant execute on bsbd_change_user_password to bsbd_readonly_role
