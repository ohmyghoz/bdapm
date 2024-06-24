CREATE TABLE [dbo].[FWAttachment]
(
[AttachId] [bigint] NOT NULL IDENTITY(1, 1),
[AttachObjId] [uniqueidentifier] NOT NULL,
[AttachTipe] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[AttachKode] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AttachThumb] [image] NULL,
[AttachFileNama] [varchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[AttachFileSize] [int] NOT NULL,
[AttachFileLink] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AttachFilePwd] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Attachment_stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL,
[HelpId] [bigint] NULL,
[PostId] [bigint] NULL,
[EminId] [bigint] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWAttachment] ADD CONSTRAINT [PK_Attachment] PRIMARY KEY CLUSTERED ([AttachId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [idx_att_stsrc_pplandok] ON [dbo].[FWAttachment] ([Stsrc]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [idx_att_stsrc2] ON [dbo].[FWAttachment] ([Stsrc]) INCLUDE ([AttachId], [AttachObjId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWAttachment] ADD CONSTRAINT [FK_FW_Attachment_Email_In] FOREIGN KEY ([EminId]) REFERENCES [dbo].[Email_In] ([emin_id])
GO
ALTER TABLE [dbo].[FWAttachment] ADD CONSTRAINT [FK_FW_Attachment_Help] FOREIGN KEY ([HelpId]) REFERENCES [dbo].[Help] ([help_id])
GO
ALTER TABLE [dbo].[FWAttachment] ADD CONSTRAINT [FK_FW_Attachment_Thread_Post] FOREIGN KEY ([PostId]) REFERENCES [dbo].[Thread_Post] ([post_id])
GO
