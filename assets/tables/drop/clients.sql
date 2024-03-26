USE [bsbd_kursach]
GO

ALTER TABLE [dbo].[Clients] DROP CONSTRAINT [CK_Clients]
GO

ALTER TABLE [dbo].[Clients] DROP CONSTRAINT [DF_Clients_IsDeleted]
GO

/****** Object:  Table [dbo].[Clients]    Script Date: 26.03.2024 20:18:35 ******/
DROP TABLE [dbo].[Clients]
GO
