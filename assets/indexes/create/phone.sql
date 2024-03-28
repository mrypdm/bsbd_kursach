SET ANSI_PADDING ON
GO

/****** Object:  Index [IX_Clients_Phone]    Script Date: 26.03.2024 20:24:54 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Clients_Phone] ON [dbo].[Clients] ([Phone] ASC)
WHERE ([IsDeleted] = 0)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
