create procedure bsbd_delete_user @userName nvarchar(50) as
    if dbo.bsbd_validate_injection (@userName) = 1
    begin
        raiserror ('Credential cannot contains symbols: [];"''', 15, 1)
        return
    end

    declare @deleteUserSql nvarchar(max)
    set @deleteUserSql = 'drop user [' + @userName + ']'
    exec (@deleteUserSql)
go

grant alter, execute on bsbd_delete_user to bsbd_owner_role
go

