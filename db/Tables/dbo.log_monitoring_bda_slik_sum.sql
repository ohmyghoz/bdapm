CREATE TABLE [dbo].[log_monitoring_bda_slik_sum]
(
[row_id] [bigint] NOT NULL IDENTITY(1, 1),
[segmentasi] [varchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[bulan_laporan] [date] NOT NULL,
[kode_kondisi] [varchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[total_account] [decimal] (38, 6) NULL,
[baki_debet] [decimal] (38, 6) NULL,
[pfeed] [varchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[pyearmonth] [date] NULL
)
GO
ALTER TABLE [dbo].[log_monitoring_bda_slik_sum] ADD CONSTRAINT [PK_log_monitoring_bda_slik_sum] PRIMARY KEY CLUSTERED  ([row_id])
GO
