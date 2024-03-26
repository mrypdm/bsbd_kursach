/****** Object:  Table [dbo].[OrdersToBooks]    Script Date: 26.03.2024 20:18:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OrdersToBooks](
	[OrderId] [int] NOT NULL,
	[BookId] [int] NOT NULL,
	[Count] [int] NOT NULL,
 CONSTRAINT [PK_OrdersToBooks] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC,
	[BookId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[OrdersToBooks]  WITH CHECK ADD  CONSTRAINT [FK_OrdersToBooks_Books] FOREIGN KEY([BookId])
REFERENCES [dbo].[Books] ([Id])
GO

ALTER TABLE [dbo].[OrdersToBooks] CHECK CONSTRAINT [FK_OrdersToBooks_Books]
GO

ALTER TABLE [dbo].[OrdersToBooks]  WITH CHECK ADD  CONSTRAINT [FK_OrdersToBooks_Orders] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[OrdersToBooks] CHECK CONSTRAINT [FK_OrdersToBooks_Orders]
GO
