CREATE TABLE [dbo].[log_monitoring_bda_slik_det]
(
[row_id] [bigint] NOT NULL IDENTITY(1, 1),
[segmentasi] [varchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[bulan_laporan] [date] NOT NULL,
[tanggal_cetak] [date] NULL,
[tanggal_terima] [date] NULL,
[selisih_waktu] [decimal] (38, 6) NULL,
[pyearmonth] [date] NULL
)
GO
ALTER TABLE [dbo].[log_monitoring_bda_slik_det] ADD CONSTRAINT [PK_log_monitoring_bda_slik_det] PRIMARY KEY CLUSTERED  ([row_id])
GO
