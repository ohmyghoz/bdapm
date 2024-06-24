CREATE TABLE [dbo].[osint_tfidf_wordcloud]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_pperiode_scrapping] [date] NOT NULL,
[dm_key_word] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_token] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_score] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_freq] [int] NULL,
[dm_pday_scrapping] [int] NULL,
[dm_pcategory_scrapping] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
ALTER TABLE [dbo].[osint_tfidf_wordcloud] ADD CONSTRAINT [PK_osint_tfidf_wordcloud] PRIMARY KEY CLUSTERED  ([rowid])
GO
