CREATE TABLE [dbo].[AuditTrail]
(
[AuditId] [bigint] NOT NULL IDENTITY(1, 1),
[AuditTipe] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[AuditCause] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[AuditMenu] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AuditDate] [datetime] NOT NULL,
[AuditIpAddress] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AuditUser] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AuditDebtorName] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AuditPrevUrl] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AuditUrl] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[AuditObjType] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AuditObjId] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AuditObjCode] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AuditXml] [xml] NULL,
[AuditJson] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AuditErrMsg] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AuditTrail] ADD CONSTRAINT [PK_Audit_Trail] PRIMARY KEY CLUSTERED ([AuditId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
