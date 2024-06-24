CREATE TABLE [dbo].[RptGrid_RoleSetting]
(
[rgrole_id] [bigint] NOT NULL IDENTITY(1, 1),
[rg_id] [bigint] NOT NULL,
[role_id] [bigint] NULL,
[satker_id] [bigint] NULL,
[stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_RptGrid_RoleSetting_stsrc] DEFAULT ('A'),
[created_by] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_created] [datetime] NULL,
[modified_by] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_modified] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[RptGrid_RoleSetting] ADD CONSTRAINT [PK_RptGrid_RoleSetting] PRIMARY KEY CLUSTERED ([rgrole_id]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[RptGrid_RoleSetting] ADD CONSTRAINT [FK_RptGrid_RoleSetting_FW_Ref_Role] FOREIGN KEY ([role_id]) REFERENCES [dbo].[FWRefRole] ([RoleId])
GO
