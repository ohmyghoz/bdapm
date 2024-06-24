CREATE TABLE [dbo].[BDA_A01_Cluster]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[kode_report] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[periode] [date] NOT NULL,
[member_type_code] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[member_code] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[jenis_agunan] [varchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[status] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[cluster] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[sum_collateral_value_member] [decimal] (38, 6) NULL,
[cnt] [bigint] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BDA_A01_Cluster] ADD CONSTRAINT [PK_BDA_A01_Cluster] PRIMARY KEY CLUSTERED ([rowid]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
