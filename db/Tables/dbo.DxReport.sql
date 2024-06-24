CREATE TABLE [dbo].[DxReport]
(
[DxRepKode] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[DxRepTipe] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[DxRepNama] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DxRepXml] [image] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DxReport] ADD CONSTRAINT [PK_DxReport_1] PRIMARY KEY CLUSTERED ([DxRepKode]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
