create role bsbd_security_role authorization bsbd_owner

grant execute, alter on bsbd_create_user to bsbd_security_role
grant execute, alter on bsbd_delete_user to bsbd_security_role
grant alter on bsbd_change_user_password to bsbd_security_role

alter role bsbd_admin_role add member bsbd_security_role

alter role db_securityadmin add member bsbd_security_role
alter role db_accessadmin add member bsbd_security_role

alter role bsbd_security add member bsbd_owner
