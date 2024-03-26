create trigger bsbd_delete_order on OrdersToBooks after delete as
begin
    declare @currentOrdersCount int
    -- if order is deleted then there is no order in Orders table
    -- then join return empty result => count = 0
    -- if order is not deleted but some of the books does then order is in Orders table
    -- then join return not empty resul => count = 1
    set @currentOrdersCount = (select count(distinct o.Id) from Orders o join deleted d on o.Id = d.OrderId)

    if @currentOrdersCount != 0 begin
        raiserror('Cannot delete book from order, because order is not deleted',15,1)
        rollback transaction
    end

    update Books
    set Count = b.Count + d.Count
    from Books b join deleted d on b.Id = d.BookId
end