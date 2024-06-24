CREATE TABLE [dbo].[BDA_HML_MCDFA]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[kode_report] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dimensi1] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[periode] [date] NOT NULL,
[member_type_code] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[member_code] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[mcdfa] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[hml_pareto] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[mcdfa_bytaxid] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[hml_pareto_bytaxid] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[sum_credit_limit] [decimal] (38, 6) NULL,
[sum_outstanding] [decimal] (38, 6) NULL,
[sum_qty_distinct] [bigint] NULL,
[sum_qty_account] [bigint] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BDA_HML_MCDFA] ADD CONSTRAINT [PK_BDA_HML_MCDFA] PRIMARY KEY CLUSTERED ([rowid]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
