create trigger bsbd_prevent_update_orders on Orders instead of update as
begin
    rollback transaction
end