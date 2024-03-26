USE [bsbd_kursach]
GO

ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [FK_Orders_Clients]
GO

/****** Object:  Table [dbo].[Orders]    Script Date: 26.03.2024 20:18:41 ******/
DROP TABLE [dbo].[Orders]
GO
