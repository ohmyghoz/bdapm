CREATE TABLE [dbo].[FWNoticeTemplate]
(
[NottId] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[NottTitle] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[NottGroup] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NottTo] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NottCc] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NottBcc] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NottBatch] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NottSender] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NottContent] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[NottCatatan] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NottOneEmailPerUser] [bit] NOT NULL CONSTRAINT [DF_Notice_Template_nott_one_email_per_user] DEFAULT ((0)),
[NottRefId] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NottRefId2] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NottRefId3] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NottModelType] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NottKeyType] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NottLastTestId] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NottSmallContent] [varchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[RdefKode] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[NottRdefParamCsv] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_FW_Notice_Template_stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWNoticeTemplate] ADD CONSTRAINT [PK_Notice_Template] PRIMARY KEY CLUSTERED ([NottId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWNoticeTemplate] ADD CONSTRAINT [FK_FW_Notice_Template_ReportDef] FOREIGN KEY ([RdefKode]) REFERENCES [dbo].[ReportDef] ([rdef_kode])
GO
