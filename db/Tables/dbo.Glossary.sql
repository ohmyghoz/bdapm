CREATE TABLE [dbo].[Glossary]
(
[GloId] [bigint] NOT NULL IDENTITY(1, 1),
[GloTipe] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[GloPIdeb] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[GloDimensi1] [varchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[GloDimensi2] [varchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[GloKetNilai1] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[GloKetNilaiRata2] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[created_by] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_created] [datetime] NULL,
[modified_by] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_modified] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Glossary] ADD CONSTRAINT [PK_Glossary] PRIMARY KEY CLUSTERED ([GloId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
