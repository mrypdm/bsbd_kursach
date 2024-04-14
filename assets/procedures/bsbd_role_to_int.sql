create function bsbd_role_to_int(@roleName nvarchar(max))
    returns int as
begin
    if @roleName = 'bsbd_security_role'
        begin
            return 0
        end

    if @roleName = 'bsbd_admin_role'
        begin
            return 1
        end

    if @roleName = 'bsbd_worker_role'
        begin
            return 2
        end

    return 3
end
