CREATE TABLE [dbo].[FWTempTable]
(
[TempId] [uniqueidentifier] NOT NULL,
[TempContent2] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[TempContent] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[UserId] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedDatetime] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWTempTable] ADD CONSTRAINT [PK_Temp_Table] PRIMARY KEY CLUSTERED ([TempId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
