USE [bsbd_kursach]
GO

/****** Object:  Table [dbo].[Books]    Script Date: 26.03.2024 20:18:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Books](
	[Id] [int] IDENTITY(0,1) NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Author] [nvarchar](50) NOT NULL,
	[ReleaseDate] [datetime2](7) NOT NULL,
	[Count] [int] NOT NULL,
	[Price] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Books] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Books] ADD  CONSTRAINT [DF_Books_Count]  DEFAULT ((0)) FOR [Count]
GO

ALTER TABLE [dbo].[Books] ADD  CONSTRAINT [DF_Books_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
