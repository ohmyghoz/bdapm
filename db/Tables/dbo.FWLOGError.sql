CREATE TABLE [dbo].[FWLOGError]
(
[ErrId] [bigint] NOT NULL IDENTITY(1, 1),
[ErrIpAddress] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ErrUserId] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ErrMessage] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ErrDescription] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ErrDate] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWLOGError] ADD CONSTRAINT [PK_LOG_Error] PRIMARY KEY CLUSTERED ([ErrId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
