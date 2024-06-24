CREATE TABLE [dbo].[RptGrid_Param]
(
[rgpr_id] [bigint] NOT NULL IDENTITY(1, 1),
[rg_id] [bigint] NOT NULL,
[rgpr_kode] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[rgpr_nama] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[rgpr_catatan] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[rgpr_datatype] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[rgpr_controltype] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[rgpr_allow_null] [bit] NULL,
[rgpr_null_text] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[rgpr_value_default] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[rgpr_value_csv] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[rgpr_value_query] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_RptGrid_Param_stsrc] DEFAULT ('A'),
[created_by] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_created] [datetime] NULL,
[modified_by] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_modified] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[RptGrid_Param] ADD CONSTRAINT [PK_RptGrid_Param] PRIMARY KEY CLUSTERED ([rgpr_id]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[RptGrid_Param] ADD CONSTRAINT [FK_RptGrid_Param_RptGrid] FOREIGN KEY ([rg_id]) REFERENCES [dbo].[RptGrid] ([rg_id])
GO
