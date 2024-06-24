CREATE TABLE [dbo].[osida_alih_deb_ke_bank_mst]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_kantor_cabang] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_kantor_cabang] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_id_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_rekening] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_total_plafon_awal] [decimal] (38, 6) NULL,
[dm_total_baki_debet] [decimal] (38, 6) NULL,
[dm_tunggakan] [decimal] (38, 6) NULL,
[dm_jml_rekening_baru] [decimal] (38, 6) NULL,
[dm_plafon_awal] [decimal] (38, 6) NULL,
[dm_baki_debet] [decimal] (38, 6) NULL
)
GO
ALTER TABLE [dbo].[osida_alih_deb_ke_bank_mst] ADD CONSTRAINT [PK_osida_alih_deb_ke_bank_mst] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
