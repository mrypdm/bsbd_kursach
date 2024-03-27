create trigger bsbd_prevent_update_orders_to_books on OrdersToBooks instead of update as
begin
    rollback transaction
end