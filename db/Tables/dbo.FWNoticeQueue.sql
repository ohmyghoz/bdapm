CREATE TABLE [dbo].[FWNoticeQueue]
(
[NotqId] [bigint] NOT NULL IDENTITY(1, 1),
[NottId] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[NottDate] [datetime] NOT NULL,
[NotqTitle] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NottSentDate] [datetime] NULL,
[NotqUser] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NotqContent] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NotqStatus] [tinyint] NOT NULL,
[EmailqId] [bigint] NULL,
[NotqEmail] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Notice_Queue_stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWNoticeQueue] ADD CONSTRAINT [PK_Notice_Queue] PRIMARY KEY CLUSTERED ([NotqId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWNoticeQueue] ADD CONSTRAINT [FK_Notice_Queue_Email_Queue] FOREIGN KEY ([EmailqId]) REFERENCES [dbo].[FWEmailQueue] ([EmailqId])
GO
ALTER TABLE [dbo].[FWNoticeQueue] ADD CONSTRAINT [FK_Notice_Queue_Notice_Template] FOREIGN KEY ([NottId]) REFERENCES [dbo].[FWNoticeTemplate] ([NottId])
GO
ALTER TABLE [dbo].[FWNoticeQueue] ADD CONSTRAINT [FK_Notice_Queue_User_Master] FOREIGN KEY ([NotqUser]) REFERENCES [dbo].[UserMaster] ([UserId])
GO
