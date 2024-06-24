CREATE TABLE [dbo].[BDA_F01_Predictive]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[kode_report] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[periode] [date] NOT NULL,
[member_type_code] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[member_code] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[status] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[collectibility_type_code] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dugaan_collectability] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[sum_outstanding] [decimal] (38, 6) NULL,
[cnt_row] [bigint] NULL,
[cnt_distinct_cif] [bigint] NULL,
[cnt_acc] [bigint] NULL,
[min_overdue_days] [bigint] NULL,
[max_overdue_days] [bigint] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BDA_F01_Predictive] ADD CONSTRAINT [PK_BDA_F01_Predictive] PRIMARY KEY CLUSTERED ([rowid]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
