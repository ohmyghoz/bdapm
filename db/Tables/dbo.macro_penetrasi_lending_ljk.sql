CREATE TABLE [dbo].[macro_penetrasi_lending_ljk]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_fasilitas_pinjaman] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kategori] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_deskripsi_kategori] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jumlah_cif] [int] NULL,
[dm_jumlah_rekening] [int] NULL,
[dm_jumlah_rekening_nonzero_os] [int] NULL,
[dm_plafon_awal] [decimal] (38, 6) NULL,
[dm_avg_plafon_awal] [decimal] (38, 6) NULL,
[dm_plafon_efektif] [decimal] (38, 6) NULL,
[dm_avg_plafon_efektif] [decimal] (38, 6) NULL,
[dm_outstanding] [decimal] (38, 6) NULL,
[dm_avg_outstanding] [decimal] (38, 6) NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[macro_penetrasi_lending_ljk] ADD CONSTRAINT [PK_macro_penetrasi_lending_ljk] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
