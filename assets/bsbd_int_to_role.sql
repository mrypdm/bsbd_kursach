CREATE function bsbd_int_to_role(@roleNumber int)
    returns nvarchar(max) as
begin
    if @roleNumber = 0
    begin
        return 'bsbd_owner_role'
    end

    if @roleNumber = 1
    begin
        return 'bsbd_admin_role'
    end

    if @roleNumber = 2
    begin
        return 'bsbd_worker_role'
    end

    return null
end
go
