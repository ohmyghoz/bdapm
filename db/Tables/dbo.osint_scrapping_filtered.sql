CREATE TABLE [dbo].[osint_scrapping_filtered]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_pperiode_scrapping] [date] NOT NULL,
[dm_scrapping_time] [datetime] NULL,
[dm_keyword] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_judul_berita] [varchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_url] [varchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_snippet] [varchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_text] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_clean_text] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_pday_scrapping] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_pcategory_scrapping] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
ALTER TABLE [dbo].[osint_scrapping_filtered] ADD CONSTRAINT [PK_osint_scrapping_filtered] PRIMARY KEY CLUSTERED  ([rowid])
GO
