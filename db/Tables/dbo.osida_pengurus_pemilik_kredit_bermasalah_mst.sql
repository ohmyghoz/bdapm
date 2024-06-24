CREATE TABLE [dbo].[osida_pengurus_pemilik_kredit_bermasalah_mst]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_nasabah] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kualitas] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_total_plafon] [decimal] (38, 6) NULL,
[dm_total_baki_debet] [decimal] (38, 6) NULL,
[dm_nama_pengurus_pemilik] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_id_pengurus_pemilik] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jabatan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_pangsa_pemilik] [decimal] (38, 6) NULL,
[dm_total_baki_debet_bermasalah] [decimal] (38, 6) NULL,
[dm_total_tunggakan_bunga] [decimal] (38, 6) NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[osida_pengurus_pemilik_kredit_bermasalah_mst] ADD CONSTRAINT [PK_osida_pengurus_pemilik_kredit_bermasalah_mst] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) ON [PRIMARY]
GO
