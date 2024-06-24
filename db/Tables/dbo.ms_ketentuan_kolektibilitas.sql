CREATE TABLE [dbo].[ms_ketentuan_kolektibilitas]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_debitur] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_rekening] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_jenis_kredit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_sektor_ekonomi] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_penggunaan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_sifat_kredit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_outstanding] [decimal] (38, 6) NULL,
[dm_plafon_awal] [decimal] (38, 6) NULL,
[dm_plafon] [decimal] (38, 6) NULL,
[dm_baki_debet] [decimal] (38, 6) NULL,
[dm_jenis_fasilitas_pinjaman] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tanggal_mulai] [date] NULL,
[dm_tanggal_awal_pinjaman] [date] NULL,
[dm_tanggal_jatuh_tempo] [date] NULL,
[dm_jumlah_hari_tunggakan] [int] NULL,
[dm_kolektibilitas_sekarang] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kolektibilitas_bulan_sebelumnya] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kategori_status_kolektibilitas] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kolektibilitas_dpd] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tunggakan_pokok] [decimal] (38, 6) NULL,
[dm_tunggakan_bunga] [decimal] (38, 6) NULL,
[dm_denda] [decimal] (38, 6) NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ms_ketentuan_kolektibilitas] ADD CONSTRAINT [PK_ms_ketentuan_kolektibilitas] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) ON [PRIMARY]
GO
