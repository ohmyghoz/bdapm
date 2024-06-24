CREATE TABLE [dbo].[osida_pengurus_pemilik_kredit_bermasalah_det_pengurus]
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
[dm_nama_pengurus] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_id_pengurus] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_kredit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_penggunaan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_sifat_kredit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tanggal_mulai] [date] NULL,
[dm_tanggal_jatuh_tempo] [date] NULL,
[dm_tanggal_awal_kredit] [date] NULL,
[dm_status_pengajuan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_valuta] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_plafon_awal] [decimal] (38, 6) NULL,
[dm_plafon] [decimal] (38, 6) NULL,
[dm_baki_debet] [decimal] (38, 6) NULL,
[dm_kolektibilitas] [decimal] (38, 6) NULL,
[dm_tgl_macet] [date] NULL,
[dm_jml_hari_tunggakan] [decimal] (38, 6) NULL,
[dm_tunggakan_pokok] [decimal] (38, 6) NULL,
[dm_tunggakan_bunga] [decimal] (38, 6) NULL,
[dm_kondisi] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tanggal_kondisi] [date] NULL,
[dm_kode_operasi] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
ALTER TABLE [dbo].[osida_pengurus_pemilik_kredit_bermasalah_det_pengurus] ADD CONSTRAINT [PK_osida_pengurus_pemilik_kredit_bermasalah_det_pengurus] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
