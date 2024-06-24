CREATE TABLE [dbo].[BDA2_Table]
(
[TableName] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[StorageType] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[SqlServerPeriodCount] [int] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BDA2_Table] ADD CONSTRAINT [PK_BDA2_Tables] PRIMARY KEY CLUSTERED ([TableName]) ON [PRIMARY]
GO
