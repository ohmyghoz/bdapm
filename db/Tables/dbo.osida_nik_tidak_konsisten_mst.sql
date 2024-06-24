CREATE TABLE [dbo].[osida_nik_tidak_konsisten_mst]
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
[dm_jenis_kelamin] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tgl_lahir] [date] NULL,
[dm_no_identitas] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jumlah_rekening] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_total_plafon_aktif] [decimal] (38, 6) NULL,
[dm_total_baki_debet] [decimal] (38, 6) NULL,
[dm_kualitas_terburuk] [decimal] (38, 6) NULL,
[dm_tunggakan_pokok] [decimal] (38, 6) NULL,
[dm_tunggakan_bunga] [decimal] (38, 6) NULL
)
GO
ALTER TABLE [dbo].[osida_nik_tidak_konsisten_mst] ADD CONSTRAINT [PK_osida_nik_tidak_konsisten_mst] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
