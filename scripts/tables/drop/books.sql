ALTER TABLE [dbo].[Books] DROP CONSTRAINT [DF_Books_IsDeleted]
GO

ALTER TABLE [dbo].[Books] DROP CONSTRAINT [DF_Books_Count]
GO

/****** Object:  Table [dbo].[Books]    Script Date: 26.03.2024 20:18:10 ******/
DROP TABLE [dbo].[Books]
GO
