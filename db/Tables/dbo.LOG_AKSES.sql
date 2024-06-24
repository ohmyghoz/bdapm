CREATE TABLE [dbo].[LOG_AKSES]
(
[log_id] [bigint] NOT NULL IDENTITY(1, 1),
[RemoteHost] [varchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AuthUser] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[StartDate] [datetime] NULL,
[RequestLine] [varchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[StatusCode] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[LOG_AKSES] ADD CONSTRAINT [PK_LOG_AKSES] PRIMARY KEY CLUSTERED ([log_id]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
