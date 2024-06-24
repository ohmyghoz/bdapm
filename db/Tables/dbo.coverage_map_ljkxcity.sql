CREATE TABLE [dbo].[coverage_map_ljkxcity]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_dati1] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_dati2] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_cakupan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nilai] [decimal] (38, 6) NULL,
[dm_kode_kota] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
ALTER TABLE [dbo].[coverage_map_ljkxcity] ADD CONSTRAINT [PK_coverage_map_ljkxcity] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
