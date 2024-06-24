CREATE TABLE [dbo].[osida_kredit_macet_tdk_hb_mst]
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
[dm_jenis_kredit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_penggunaan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_sifat_kredit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tgl_awal_kredit] [date] NULL,
[dm_tgl_mulai] [date] NULL,
[dm_tgl_jatuh_tempo] [date] NULL,
[dm_status_pengajuan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_valuta] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_plafon_awal] [decimal] (38, 6) NULL,
[dm_plafon] [decimal] (38, 6) NULL,
[dm_baki_debet] [decimal] (38, 6) NULL,
[dm_kolektibilitas] [decimal] (38, 6) NULL,
[dm_jumlah_hari_menunggak] [decimal] (38, 6) NULL,
[dm_tunggakan_pokok] [decimal] (38, 6) NULL,
[dm_tunggakan_bunga] [decimal] (38, 6) NULL,
[dm_tgl_macet] [date] NULL,
[dm_jw_macet] [decimal] (38, 6) NULL,
[dm_jml_agunan] [decimal] (38, 6) NULL,
[dm_njop_nilai_wajar] [decimal] (38, 6) NULL,
[dm_menurut_pelapor] [decimal] (38, 6) NULL,
[dm_menurut_penilai_independen] [decimal] (38, 6) NULL,
[dm_menurut_pelapor_persentase] [decimal] (38, 6) NULL
)
GO
ALTER TABLE [dbo].[osida_kredit_macet_tdk_hb_mst] ADD CONSTRAINT [PK_osida_kredit_macet_tdk_hb_mst] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
