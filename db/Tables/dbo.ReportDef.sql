CREATE TABLE [dbo].[ReportDef]
(
[rdef_kode] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[rdef_def] [image] NOT NULL,
[rdef_nama] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ReportDef] ADD CONSTRAINT [PK_ReportDef_1] PRIMARY KEY CLUSTERED ([rdef_kode]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
