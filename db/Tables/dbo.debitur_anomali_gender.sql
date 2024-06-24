CREATE TABLE [dbo].[debitur_anomali_gender]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_debitur] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_jenis_id_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_id_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_id_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_gender] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tanggal_lahir] [date] NULL,
[dm_status_valid_id_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_status_valid_gender_based_kode] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_status_valid_gender_based_ktp] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
ALTER TABLE [dbo].[debitur_anomali_gender] ADD CONSTRAINT [PK_debitur_anomali_gender] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
