CREATE TABLE [dbo].[ms_perubahan_suku_bunga_no_restrukturisasi]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_rekening] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_debitur] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_jenis_pinjaman] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kolektibilitas] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_sektor_ekonomi] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_suku_bunga] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_suku_bunga_sekarang] [decimal] (12, 5) NULL,
[dm_suku_bunga_sebelumnya] [decimal] (12, 5) NULL,
[dm_perubahan_suku_bunga] [decimal] (12, 5) NULL,
[dm_status_threshold_suku_bunga] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
ALTER TABLE [dbo].[ms_perubahan_suku_bunga_no_restrukturisasi] ADD CONSTRAINT [PK_ms_perubahan_suku_bunga_no_restrukturisasi] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
