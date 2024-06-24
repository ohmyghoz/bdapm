CREATE TABLE [dbo].[ma_aml_cft_analysis]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_debitur] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nomor_rekening] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_kode_credit_nature] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_jenis_pinjaman] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_skema_pinjaman] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_agunan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_agunan] [decimal] (38, 6) NULL,
[dm_plafon] [decimal] (38, 6) NULL,
[dm_outstanding] [decimal] (38, 6) NULL,
[dm_baki_debet] [decimal] (38, 6) NULL,
[dm_kolektibilitas] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ma_aml_cft_analysis] ADD CONSTRAINT [PK_ma_aml_cft_analysis] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
