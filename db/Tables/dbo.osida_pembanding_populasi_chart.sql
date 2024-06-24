CREATE TABLE [dbo].[osida_pembanding_populasi_chart]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_plafon_takeover_231_hari_terakhir] [decimal] (38, 6) NULL,
[dm_jumlah_rekening_takeover_231_hari_terakhir] [int] NULL,
[dm_plafon_1_tahun_terakhir] [decimal] (38, 6) NULL,
[dm_jumlah_rekening_1_tahun_terakhir] [int] NULL,
[dm_plafon_231_hari_terakhir] [decimal] (38, 6) NULL,
[dm_jumlah_rekening_231_hari_terakhir] [int] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[osida_pembanding_populasi_chart] ADD CONSTRAINT [PK_osida_pembanding_populasi_chart] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) ON [PRIMARY]
GO
