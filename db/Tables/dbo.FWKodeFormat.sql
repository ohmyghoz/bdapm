CREATE TABLE [dbo].[FWKodeFormat]
(
[KofId] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[KofCounterLength] [tinyint] NOT NULL,
[KofIncrement] [tinyint] NOT NULL,
[KofStart] [int] NOT NULL,
[KofCatatan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[KofResetTp] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[KofResetInterval] [int] NOT NULL,
[KofResetTime] [datetime] NOT NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Kode_Format_New_stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWKodeFormat] ADD CONSTRAINT [PK_Kode_Format_New] PRIMARY KEY CLUSTERED ([KofId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
