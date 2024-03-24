create trigger bsbd_prevent_ud_orders on Orders instead of update, delete as
begin
	rollback transaction
end