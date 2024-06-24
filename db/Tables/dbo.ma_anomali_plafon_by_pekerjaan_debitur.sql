CREATE TABLE [dbo].[ma_anomali_plafon_by_pekerjaan_debitur]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_kode_jenis_pinjaman] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_jenis_penggunaan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_rekening] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_nama_debitur] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_outstanding] [decimal] (38, 6) NOT NULL,
[dm_baki_debet] [decimal] (38, 6) NOT NULL,
[dm_plafon] [decimal] (38, 6) NOT NULL,
[dm_avg_plafon] [decimal] (38, 6) NULL,
[dm_status_plafon] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_profesi_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_profesi_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tanggal_kontrak] [date] NULL,
[dm_tanggal_jatuh_tempo] [date] NULL,
[dm_kondisi] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_plafon_above_threshold] [decimal] (38, 6) NULL,
[dm_plafon_below_threshold] [decimal] (38, 6) NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ma_anomali_plafon_by_pekerjaan_debitur] ADD CONSTRAINT [PK_ma_anomali_plafon_by_pekerjaan_debitur] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
