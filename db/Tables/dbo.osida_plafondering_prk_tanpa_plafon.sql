CREATE TABLE [dbo].[osida_plafondering_prk_tanpa_plafon]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_periode] [date] NOT NULL,
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_no_rekening] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_nama_debitur] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_jenis_kredit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_jenis_penggunaan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_sifat_kredit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_tanggal_akad_awal] [date] NOT NULL,
[dm_tanggal_akad_akhir] [date] NOT NULL,
[dm_tanggal_mulai] [date] NOT NULL,
[dm_tanggal_jatuh_tempo] [date] NOT NULL,
[dm_valuta] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_plafon_awal] [decimal] (38, 6) NOT NULL,
[dm_bulanlaporan_plafon] [decimal] (38, 6) NULL,
[dm_bulanlaporan_bakidebet] [decimal] (38, 6) NULL,
[dm_bulanlalu_m1_plafon] [decimal] (38, 6) NULL,
[dm_bulanlalu_m1_bakidebet] [decimal] (38, 6) NULL,
[dm_bulansebelumnya_m2_plafon] [decimal] (38, 6) NULL,
[dm_bulansebelumnya_m2_bakidebet] [decimal] (38, 6) NULL,
[dm_kolektibilitas] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_jumlah_hari_tunggakan] [int] NULL,
[dm_tunggakan_pokok] [decimal] (38, 6) NULL,
[dm_tunggakan_bunga] [decimal] (38, 6) NULL,
[dm_denda] [decimal] (38, 6) NULL,
[dm_kondisi] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_kantor_cabang] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_nama_kantor_cabang] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_akad_akhir] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PartitionTable_ByYearMonthDateScheme_BDAP] ([dm_periode])
GO
ALTER TABLE [dbo].[osida_plafondering_prk_tanpa_plafon] ADD CONSTRAINT [PK_osida_plafondering_prk_tanpa_plafon] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) WITH (FILLFACTOR=80) ON [PartitionTable_ByYearMonthDateScheme_BDAP] ([dm_periode])
GO
