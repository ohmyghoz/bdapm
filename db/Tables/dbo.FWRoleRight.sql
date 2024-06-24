CREATE TABLE [dbo].[FWRoleRight]
(
[RightId] [bigint] NOT NULL IDENTITY(1, 1),
[RoleId] [bigint] NOT NULL,
[ModId] [bigint] NOT NULL,
[IsView] [bit] NOT NULL,
[IsAdd] [bit] NOT NULL,
[IsEdit] [bit] NOT NULL,
[IsDelete] [bit] NOT NULL,
[IsReview] [bit] NOT NULL,
[IsApprove] [bit] NOT NULL,
[IsOpen] [bit] NOT NULL,
[IsPrint] [bit] NOT NULL,
[IsExport] [bit] NOT NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_FW_Role_Right_stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWRoleRight] ADD CONSTRAINT [PK_New_Role_Right] PRIMARY KEY CLUSTERED ([RightId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWRoleRight] ADD CONSTRAINT [FK_FWRoleRight_FWRoleRight] FOREIGN KEY ([RightId]) REFERENCES [dbo].[FWRoleRight] ([RightId])
GO
