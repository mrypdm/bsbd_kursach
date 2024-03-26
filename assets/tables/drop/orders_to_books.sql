USE [bsbd_kursach]
GO

ALTER TABLE [dbo].[OrdersToBooks] DROP CONSTRAINT [FK_OrdersToBooks_Orders]
GO

ALTER TABLE [dbo].[OrdersToBooks] DROP CONSTRAINT [FK_OrdersToBooks_Books]
GO

/****** Object:  Table [dbo].[OrdersToBooks]    Script Date: 26.03.2024 20:18:51 ******/
DROP TABLE [dbo].[OrdersToBooks]
GO
