CREATE TABLE [dbo].[macro_policy_evaluation_analysis]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_jenis_kredit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_penggunaan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_sifat_kredit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jumlah_rekening] [int] NULL,
[dm_outstanding] [decimal] (38, 6) NULL,
[dm_outstanding_sebelumnya] [decimal] (38, 6) NULL,
[dm_plafon] [decimal] (38, 6) NULL,
[dm_plafon_sebelumnya] [decimal] (38, 6) NULL,
[dm_jenis_fasilitas_pinjaman] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kolektibilitas_dpd] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kolektibilitas_sekarang] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kolektibilitas_bulan_sebelumnya] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jumlah_hari_tunggakan] [int] NULL
)
GO
ALTER TABLE [dbo].[macro_policy_evaluation_analysis] ADD CONSTRAINT [PK_macro_policy_evaluation_analysis] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
