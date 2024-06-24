CREATE TABLE [dbo].[FWRefModulMenu]
(
[ModMenuId] [bigint] NOT NULL IDENTITY(1, 1),
[ModId] [bigint] NULL,
[ModMenuNama] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ModMenuAksi] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ModMenuUrl] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ModMenuIsHidden] [bit] NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWRefModulMenu] ADD CONSTRAINT [PK__FWRefMen__ADDAF00A78F67904] PRIMARY KEY CLUSTERED ([ModMenuId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWRefModulMenu] ADD CONSTRAINT [FK_FWRefModulMenu_FWRefModul] FOREIGN KEY ([ModId]) REFERENCES [dbo].[FWRefModul] ([ModId])
GO
