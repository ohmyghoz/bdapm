CREATE TABLE [dbo].[ms_pemberian_pinjaman_karyawan_ljk]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_debitur] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_rekening] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_jenis_pinjaman] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_outstanding] [decimal] (38, 6) NULL,
[dm_avg_outstanding_market] [decimal] (38, 6) NULL,
[dm_plafon] [decimal] (38, 6) NULL,
[dm_avg_plafon_market] [decimal] (38, 6) NULL,
[dm_baki_debet] [decimal] (38, 6) NULL,
[dm_avg_baki_debet_market] [decimal] (38, 6) NULL,
[dm_suku_bunga] [decimal] (38, 6) NULL,
[dm_avg_suku_bunga] [decimal] (38, 6) NULL,
[dm_above_threshold_plafon_market] [decimal] (38, 6) NULL,
[dm_below_threshold_plafon_market] [decimal] (38, 6) NULL,
[dm_status_plafon] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_above_threshold_baki_debet_market] [decimal] (38, 6) NULL,
[dm_below_threshold_baki_debet_market] [decimal] (38, 6) NULL,
[dm_status_baki_debet] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_pekerjaan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tempat_bekerja] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_bidang_usaha] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ms_pemberian_pinjaman_karyawan_ljk] ADD CONSTRAINT [PK_ms_pemberian_pinjaman_karyawan_ljk] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) ON [PRIMARY]
GO
