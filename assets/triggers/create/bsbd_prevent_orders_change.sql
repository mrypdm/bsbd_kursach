create trigger bsbd_prevent_orders_change on Orders after update, delete as
begin 
    rollback transaction 
end
