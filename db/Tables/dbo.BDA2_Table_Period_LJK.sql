CREATE TABLE [dbo].[BDA2_Table_Period_LJK]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[TableName] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Period] [date] NOT NULL,
[JenisLJK] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[KodeLJK] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Rows] [int] NULL,
[LastProcess] [datetime] NULL
)
GO
ALTER TABLE [dbo].[BDA2_Table_Period_LJK] ADD CONSTRAINT [PK_BDA2_Table_Period_LJK] PRIMARY KEY CLUSTERED  ([rowid])
GO
