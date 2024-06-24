CREATE TABLE [dbo].[osint_edge_wordpair]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_pperiode_scrapping] [date] NOT NULL,
[dm_key_word] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_bigram] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_freq] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_node1] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_node2] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_pday_scrapping] [int] NULL,
[dm_pcategory_scrapping] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
ALTER TABLE [dbo].[osint_edge_wordpair] ADD CONSTRAINT [PK_osint_edge_wordpair] PRIMARY KEY CLUSTERED  ([rowid])
GO
