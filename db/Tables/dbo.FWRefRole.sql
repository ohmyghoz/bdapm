CREATE TABLE [dbo].[FWRefRole]
(
[RoleId] [bigint] NOT NULL IDENTITY(1, 1),
[RoleName] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[RoleCatatan] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Urut] [int] NULL,
[RoleScope] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_FW_Ref_Role_stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWRefRole] ADD CONSTRAINT [PK_New_Ref_Role] PRIMARY KEY CLUSTERED ([RoleId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
