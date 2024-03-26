USE [bsbd_kursach]
GO

ALTER TABLE [dbo].[BooksToTags] DROP CONSTRAINT [FK_BooksToTags_Tags]
GO

ALTER TABLE [dbo].[BooksToTags] DROP CONSTRAINT [FK_BooksToTags_Books]
GO

/****** Object:  Table [dbo].[BooksToTags]    Script Date: 26.03.2024 20:18:02 ******/
DROP TABLE [dbo].[BooksToTags]
GO
