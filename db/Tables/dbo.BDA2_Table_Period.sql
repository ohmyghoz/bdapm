CREATE TABLE [dbo].[BDA2_Table_Period]
(
[TableName] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Period] [date] NOT NULL,
[Rows] [int] NULL,
[LastProcess] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BDA2_Table_Period] ADD CONSTRAINT [PK_BDA2_Table_Period] PRIMARY KEY CLUSTERED ([TableName], [Period]) ON [PRIMARY]
GO
