CREATE TABLE [dbo].[micro_plafond_usability_acc_detail]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_fasilitas_pinjaman] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_debitur] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_rekening] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_akad_awal] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_akad_akhir] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tanggal_awal_pinjaman] [date] NULL,
[dm_tanggal_mulai] [date] NULL,
[dm_tanggal_jatuh_tempo] [date] NULL,
[dm_valuta] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_sifat_kredit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_kredit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_penggunaan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_outstanding] [decimal] (38, 6) NULL,
[dm_plafon_awal] [decimal] (38, 6) NULL,
[dm_plafon] [decimal] (38, 6) NULL,
[dm_baki_debet] [decimal] (38, 6) NULL,
[dm_kelas_plafon] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_persen_penggunaan_plafon] [decimal] (38, 6) NULL,
[dm_kolektibilitas] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_kondisi] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tanggal_kondisi] [date] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[micro_plafond_usability_acc_detail] ADD CONSTRAINT [PK_micro_plafond_usability_acc_detail] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
