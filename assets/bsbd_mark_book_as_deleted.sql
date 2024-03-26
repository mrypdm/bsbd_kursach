create trigger bsbd_mark_book_as_deleted on Books instead of delete as
begin
    update Books
    set
        IsDeleted = 1,
        Count = 0
    from Books c join deleted d on c.Id = d.Id

    delete bt from BooksToTags bt join deleted d on bt.BookId = d.Id
end