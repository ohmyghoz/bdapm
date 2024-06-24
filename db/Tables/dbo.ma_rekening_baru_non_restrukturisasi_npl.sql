CREATE TABLE [dbo].[ma_rekening_baru_non_restrukturisasi_npl]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_jenis_pinjaman] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_rekening] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_debitur] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_outstanding] [decimal] (38, 6) NULL,
[dm_baki_debet] [decimal] (38, 6) NULL,
[dm_plafon] [decimal] (38, 6) NULL,
[dm_jumlah_hari_tunggakan] [int] NULL,
[dm_npl] [int] NULL,
[dm_tanggal_mulai] [date] NULL,
[dm_tanggal_awal_pinjaman] [date] NULL,
[dm_tanggal_jatuh_tempo] [date] NULL,
[dm_selisih_awalvsperiode] [int] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ma_rekening_baru_non_restrukturisasi_npl] ADD CONSTRAINT [PK_ma_rekening_baru_non_restrukturisasi_npl] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
