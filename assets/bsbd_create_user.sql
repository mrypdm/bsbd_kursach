CREATE proc bsbd_create_user @userName nvarchar(50), @password nvarchar(50), @role int as
    if dbo.bsbd_validate_injection(@userName) = 1
       or dbo.bsbd_validate_injection(@password) = 1
    begin
        raiserror ('Credential cannot contains symbols: [];"''', 15, 1)
        return
    end

    declare @roleString nvarchar(max)
    set @roleString = dbo.bsbd_int_to_role(@role)
    if @roleString IS NULL
    begin
        raiserror ('Role must be in range [0; 2]', 15, 1)
        return
    end

    declare @createUserSql nvarchar(max)
    set @createUserSql =
        'create user [' + @userName + '] ' +
        ' with password = N''' + @password + ''''
    exec (@createUserSql)

    declare @addToRoleSql nvarchar(max)
    set @addToRoleSql =
        'alter role [' + @roleString + '] ' +
        'add member [' + @userName + ']'
    exec (@addToRoleSql)
go

grant alter, execute on bsbd_create_user to bsbd_owner_role
go