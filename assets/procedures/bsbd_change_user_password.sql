CREATE proc bsbd_change_user_password @userName nvarchar(50),
                                      @newPassword nvarchar(50),
                                      @oldPassword nvarchar(50) = null as
    if dbo.bsbd_validate_injection(@userName) = 1
       or dbo.bsbd_validate_injection(@newPassword) = 1
       or dbo.bsbd_validate_injection (@oldPassword) = 1
    begin
        raiserror ('Credential cannot contains symbols: [];"''', 15, 1)
        return
    end

    declare @changePasswordSql nvarchar(max)

    if @oldPassword is null
    begin
        set @changePasswordSql =
            'alter user [' + @userName + '] ' +
            'with password=N''' + @newPassword + ''''
    end
    else
    begin
        set @changePasswordSql =
            'alter user [' + @userName + '] ' +
            'with password=N''' + @newPassword + ''' ' +
            'old_password=N''' + @oldPassword + ''''
    end

    exec (@changePasswordSql)
go

grant alter, execute on bsbd_change_user_password to bsbd_owner_role
go

grant execute on bsbd_change_user_password to bsbd_readonly_role
go
