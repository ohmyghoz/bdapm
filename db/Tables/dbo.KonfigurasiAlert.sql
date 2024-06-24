CREATE TABLE [dbo].[KonfigurasiAlert]
(
[KaId] [bigint] NOT NULL IDENTITY(1, 1),
[JenisKategori] [varchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[KaPeriode] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_KonfigurasiAlert_Stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[KonfigurasiAlert] ADD CONSTRAINT [PK_KonfigurasiAlert] PRIMARY KEY CLUSTERED ([KaId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
