CREATE TABLE [dbo].[osida_pemberian_kur_deb_noneligible_mst]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_kantor_cabang] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_kantor_cabang] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_nasabah] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nik] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jml_rekening] [decimal] (38, 6) NULL,
[dm_baki_debet_bulan_lalu] [decimal] (38, 6) NULL,
[dm_tunggakan_bunga_bulan_lalu] [decimal] (38, 6) NULL,
[dm_jml_rek_fasilitas_kredit_baru] [decimal] (38, 6) NULL,
[dm_plafon_kredit_baru] [decimal] (38, 6) NULL,
[dm_baki_debet_kredit_baru] [decimal] (38, 6) NULL
)
GO
ALTER TABLE [dbo].[osida_pemberian_kur_deb_noneligible_mst] ADD CONSTRAINT [PK_osida_pemberian_kur_deb_noneligible_mst] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
