USE [bsbd_kursach]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [IX_Books_Author]    Script Date: 26.03.2024 20:24:31 ******/
CREATE NONCLUSTERED INDEX [IX_Books_Author] ON [dbo].[Books]
(
	[Author] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
