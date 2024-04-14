create trigger bsbd_prevent_books_in_orders_change on OrdersToBooks after update, delete as
begin 
    rollback transaction 
end
