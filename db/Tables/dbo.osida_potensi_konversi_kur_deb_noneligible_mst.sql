CREATE TABLE [dbo].[osida_potensi_konversi_kur_deb_noneligible_mst]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nik] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_id_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jumlah_rekening_pelunasan_dipercepat] [decimal] (38, 6) NULL,
[dm_total_plafon_awal] [decimal] (38, 6) NULL,
[dm_total_tunggakan_bunga_bulan_lalu] [decimal] (38, 6) NULL,
[dm_jumlah_rekening_fasilitas_kredit_baru] [decimal] (38, 6) NULL,
[dm_total_plafon_kredit_baru] [decimal] (38, 6) NULL,
[dm_total_baki_debet_kredit_baru] [decimal] (38, 6) NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[osida_potensi_konversi_kur_deb_noneligible_mst] ADD CONSTRAINT [PK_osida_potensi_konversi_kur_deb_noneligible_mst] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) ON [PRIMARY]
GO
