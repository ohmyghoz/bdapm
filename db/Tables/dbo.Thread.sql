CREATE TABLE [dbo].[Thread]
(
[thread_id] [bigint] NOT NULL IDENTITY(1, 1),
[thread_title] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[thread_content] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[thread_owner] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[thread_start_date] [datetime] NOT NULL,
[thread_last_post] [datetime] NOT NULL,
[thread_last_replier] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Thread_stsrc] DEFAULT ('A'),
[created_by] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_created] [datetime] NULL,
[modified_by] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_modified] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Thread] ADD CONSTRAINT [PK_Thread] PRIMARY KEY CLUSTERED ([thread_id]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Thread] ADD CONSTRAINT [FK_Thread_User_Master] FOREIGN KEY ([thread_owner]) REFERENCES [dbo].[UserMaster] ([UserId])
GO
