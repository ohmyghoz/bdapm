CREATE TABLE [dbo].[da_anomali_tempat_lahir_debitur]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_bulan_data] [date] NOT NULL,
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_kantor_cabang] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_kantor_cabang] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_id_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_id_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_bentuk_badan_usaha] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_lengkap] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tempat_lahir] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tanggal_lahir] [date] NULL,
[dm_kode_anomali] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[da_anomali_tempat_lahir_debitur] ADD CONSTRAINT [PK_da_anomali_tempat_lahir_debitur] PRIMARY KEY CLUSTERED ([rowid], [dm_bulan_data]) ON [PRIMARY]
GO
