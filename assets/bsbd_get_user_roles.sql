create proc bsbd_get_user_roles @userName nvarchar(50) as
select r.name as [role]
from sys.database_role_members rm
    join sys.database_principals r on rm.role_principal_id = r.principal_id
    join sys.database_principals m on rm.member_principal_id = m.principal_id
where m.name = @userName