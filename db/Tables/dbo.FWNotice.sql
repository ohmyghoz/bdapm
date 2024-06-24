CREATE TABLE [dbo].[FWNotice]
(
[NoticeId] [bigint] NOT NULL IDENTITY(1, 1),
[NoticeTitle] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Notice_notice_title] DEFAULT (''),
[NoticeSender] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Notice_notice_sender] DEFAULT (''),
[NoticeTo] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Notice_notice_to] DEFAULT (''),
[NoticeCc] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Notice_notice_cc] DEFAULT (''),
[NoticeBcc] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Notice_notice_bcc] DEFAULT (''),
[NoticeBatchStatus] [bit] NOT NULL CONSTRAINT [DF_Notice_notice_batch_status] DEFAULT ((0)),
[NoticeContent] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Notice_notice_content] DEFAULT (''),
[NoticeSmallContent] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Notice_notice_small_content] DEFAULT (''),
[NottId] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Notice_stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL,
[NoticeRefId] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Notice_notice_ref_id] DEFAULT (''),
[NoticeRefId2] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Notice_notice_ref_id2] DEFAULT (''),
[NoticeRefId3] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Notice_notice_ref_id3] DEFAULT (''),
[NoticeBatchUsers] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Notice_notice_batch_users] DEFAULT (''),
[ApprId] [bigint] NULL,
[NoticeCatatan] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Notice_notice_catatan] DEFAULT (''),
[NoticeAttachLink] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NoticeAttachLink2] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NoticeAttachLink3] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NoticeRdefParamCsv] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWNotice] ADD CONSTRAINT [PK_Notice] PRIMARY KEY CLUSTERED ([NoticeId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [idx_notice] ON [dbo].[FWNotice] ([Stsrc], [CreatedDatetime] DESC) INCLUDE ([NoticeCc], [NoticeTo]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWNotice] ADD CONSTRAINT [FK_FW_Notice_FW_Notice_Template] FOREIGN KEY ([NottId]) REFERENCES [dbo].[FWNoticeTemplate] ([NottId])
GO
