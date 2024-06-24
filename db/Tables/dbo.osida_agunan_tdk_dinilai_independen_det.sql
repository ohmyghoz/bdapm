CREATE TABLE [dbo].[osida_agunan_tdk_dinilai_independen_det]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_kantor_cabang] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_kantor_cabang] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_rekening] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_nasabah] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_agunan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_agunan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_ikat] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_pemilik_agunan] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_bukti_kepemilikan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_alamat_agunan] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_lokasi_agunan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nilai_agunan_njop] [decimal] (38, 6) NULL,
[dm_nilai_agunan_ljk] [decimal] (38, 6) NULL
)
GO
ALTER TABLE [dbo].[osida_agunan_tdk_dinilai_independen_det] ADD CONSTRAINT [PK_osida_agunan_tdk_dinilai_independen_det] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
