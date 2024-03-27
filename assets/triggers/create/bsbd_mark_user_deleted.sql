create trigger bsbd_mark_user_deleted on Clients instead of delete as
begin
    update Clients
    set
        FirstName = '',
        LastName = '',
        Phone = '0000000000',
        IsDeleted = 1
    from Clients c join deleted d on c.Id = d.Id
end