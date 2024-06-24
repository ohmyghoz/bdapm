CREATE TABLE [dbo].[RptGrid]
(
[rg_id] [bigint] NOT NULL IDENTITY(1, 1),
[parent_id] [bigint] NULL,
[rg_nama] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[rg_catatan] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[rg_kode] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[rg_tipe] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_RptGrid_rg_tipe] DEFAULT ('(''Predefined'',''Report'')'),
[rg_db_name] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[rg_query] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[rg_entrier] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[rg_rolesetting_inherited] [bit] NOT NULL CONSTRAINT [DF_RptGrid_rg_rolesetting_inherited] DEFAULT ((1)),
[rg_timeout] [int] NOT NULL CONSTRAINT [DF_RptGrid_rg_timeout] DEFAULT ((10)),
[stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_RptGrid_stsrc] DEFAULT ('A'),
[created_by] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_created] [datetime] NULL,
[modified_by] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_modified] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[RptGrid] ADD CONSTRAINT [PK_RptGrid] PRIMARY KEY CLUSTERED ([rg_id]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[RptGrid] ADD CONSTRAINT [FK_RptGrid_RptGrid] FOREIGN KEY ([parent_id]) REFERENCES [dbo].[RptGrid] ([rg_id])
GO
