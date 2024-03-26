create trigger bsbd_verify_order on OrdersToBooks after insert as
begin
	if exists(select b.Id from inserted i join Books b on i.BookId = b.Id where i.Count > b.Count)
	begin
		raiserror('Not enough books to order', 15, 1)
        select b.Id from inserted i join Books b on i.BookId = b.Id where i.Count > b.Count
		rollback transaction
	end

	update Books
	set Count = b.Count - i.Count
	from Books b join inserted i on b.Id = i.BookId
end