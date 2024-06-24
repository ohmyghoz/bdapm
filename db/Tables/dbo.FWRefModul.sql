CREATE TABLE [dbo].[FWRefModul]
(
[ModId] [bigint] NOT NULL IDENTITY(1, 1),
[ParentModId] [bigint] NULL CONSTRAINT [DF_Table_1_parent_mod_id] DEFAULT ((0)),
[ModKode] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[ModNama] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ModCatatan] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_New_Ref_Modul_mod_catatan] DEFAULT (''),
[ModUrut] [bigint] NOT NULL,
[ModIconClass] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ModIsPublic] [bit] NULL,
[ModIsHidden] [bit] NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL,
[ModTooltip] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
ALTER TABLE [dbo].[FWRefModul] ADD CONSTRAINT [PK_FWRefModul] PRIMARY KEY CLUSTERED  ([ModId])
GO
