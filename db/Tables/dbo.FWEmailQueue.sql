CREATE TABLE [dbo].[FWEmailQueue]
(
[EmailqId] [bigint] NOT NULL IDENTITY(1, 1),
[EmailqTo] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Email_Queue_emailq_to] DEFAULT (''),
[EmailqFrom] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Email_Queue_emailq_from] DEFAULT (''),
[EmailqCc] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Email_Queue_emailq_cc] DEFAULT (''),
[EmailqBcc] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Email_Queue_emailq_bcc] DEFAULT (''),
[EmailqReplyTo] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Email_Queue_emailq_reply_to] DEFAULT (''),
[EmailqSubject] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Email_Queue_emailq_subject] DEFAULT (''),
[EmailqBody] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Email_Queue_emailq_body] DEFAULT (''),
[EmailqAttchName1] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[EmailqAttchFile1] [image] NULL,
[EmailqAttchName2] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[EmailqAttchFile2] [image] NULL,
[EmailqAttchName3] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[EmailqAttchFile3] [image] NULL,
[EmailqStatus] [tinyint] NULL,
[EmailqQueueDate] [smalldatetime] NOT NULL,
[EmailqScheduledSent] [smalldatetime] NOT NULL,
[EmailqSentDate] [smalldatetime] NULL,
[EmailqSentTry] [int] NULL CONSTRAINT [DF_Email_Queue_emailq_sent_try] DEFAULT ((0)),
[EmailqProcess] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Email_Queue_emailq_process] DEFAULT (''),
[EmailqProcessId] [bigint] NULL,
[EmailqErrorText] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Email_Queue_stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL,
[NoticeId] [bigint] NULL,
[EmailqMessageId] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWEmailQueue] ADD CONSTRAINT [PK_Email_Queue] PRIMARY KEY CLUSTERED ([EmailqId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWEmailQueue] ADD CONSTRAINT [FK_FW_Email_Queue_FW_Notice] FOREIGN KEY ([NoticeId]) REFERENCES [dbo].[FWNotice] ([NoticeId])
GO
