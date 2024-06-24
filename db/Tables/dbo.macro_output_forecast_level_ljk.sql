CREATE TABLE [dbo].[macro_output_forecast_level_ljk]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_fasilitas_pinjaman] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tipe_forecasting] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nilai] [decimal] (38, 6) NULL,
[dm_status] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_date_generated] [date] NULL,
[dm_pperiode] [date] NULL
)
GO
ALTER TABLE [dbo].[macro_output_forecast_level_ljk] ADD CONSTRAINT [PK_macro_output_forecast_level_ljk] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
