CREATE TABLE [dbo].[osida_kredit_no_agunan]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_no_rekening] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_nama_debitur] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_jenis_kredit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_jenis_penggunaan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_sifat_kredit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_tanggal_awal_pinjaman] [date] NOT NULL,
[dm_tanggal_mulai] [date] NOT NULL,
[dm_tanggal_jatuh_tempo] [date] NOT NULL,
[dm_valuta] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_plafon_awal] [decimal] (38, 6) NOT NULL,
[dm_plafon] [decimal] (38, 6) NOT NULL,
[dm_baki_debet] [decimal] (38, 6) NOT NULL,
[dm_kolektibilitas] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_jumlah_hari_tunggakan] [int] NULL,
[dm_tunggakan_pokok] [decimal] (38, 6) NULL,
[dm_tunggakan_bunga] [decimal] (38, 6) NULL,
[dm_denda] [decimal] (38, 6) NULL,
[dm_kondisi] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tanggal_kondisi] [date] NULL,
[dm_kode_operasi] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_kantor_cabang] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_nama_kantor_cabang] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_akad_akhir] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PartitionTable_ByYearMonthDateScheme_BDAP] ([dm_periode])
GO
ALTER TABLE [dbo].[osida_kredit_no_agunan] ADD CONSTRAINT [PK_osida_kredit_no_agunan] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) WITH (FILLFACTOR=80) ON [PartitionTable_ByYearMonthDateScheme_BDAP] ([dm_periode])
GO
