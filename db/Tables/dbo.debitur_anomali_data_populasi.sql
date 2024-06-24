CREATE TABLE [dbo].[debitur_anomali_data_populasi]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_total] [int] NULL,
[dm_pjenis_data_populasi] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
ALTER TABLE [dbo].[debitur_anomali_data_populasi] ADD CONSTRAINT [PK_debitur_anomali_data_populasi] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
