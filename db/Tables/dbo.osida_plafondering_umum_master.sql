CREATE TABLE [dbo].[osida_plafondering_umum_master]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_periode] [date] NOT NULL,
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_nama_debitur] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_jumlah_rekeninglunas_bulanlau] [int] NULL,
[dm_plafonawal_bulanlalu] [decimal] (38, 6) NULL,
[dm_bakidebet_bulanlalu] [decimal] (38, 6) NULL,
[dm_tunggakanbunga_bulanlalu] [decimal] (38, 6) NULL,
[dm_jumlah_rekeningbaru_bulanpelaporan] [decimal] (38, 6) NULL,
[dm_plafonawal_bulanpelaporan] [decimal] (38, 6) NULL,
[dm_bakidebet_bulanpelaporan] [decimal] (38, 6) NULL,
[dm_tunggakanbunga_bulanpelaporan] [decimal] (38, 6) NULL,
[dm_jumlah_plafon_rekening_baru] [decimal] (38, 6) NULL,
[dm_id_filter] [nvarchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PartitionTable_ByYearMonthDateScheme_BDAP] ([dm_periode])
GO
ALTER TABLE [dbo].[osida_plafondering_umum_master] ADD CONSTRAINT [PK_osida_plafondering_umum_master] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) WITH (FILLFACTOR=80) ON [PartitionTable_ByYearMonthDateScheme_BDAP] ([dm_periode])
GO
