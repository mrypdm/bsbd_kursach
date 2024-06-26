create view bsbd_principals (Id, Name, Role) as
select m.principal_id, m.name, dbo.bsbd_role_to_int(r.name)
from sys.database_role_members rm
    join sys.database_principals r on rm.role_principal_id = r.principal_id
    join sys.database_principals m on rm.member_principal_id = m.principal_id
WHERE m.type_desc = 'SQL_USER' AND m.name != 'dbo'