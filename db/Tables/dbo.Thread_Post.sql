CREATE TABLE [dbo].[Thread_Post]
(
[post_id] [bigint] NOT NULL IDENTITY(1, 1),
[thread_id] [bigint] NOT NULL,
[post_user] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[post_title] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[post_content] [varchar] (8000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[post_date] [datetime] NOT NULL,
[post_last_edit_date] [datetime] NULL,
[post_last_edit_by] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[post_urut] [int] NOT NULL CONSTRAINT [DF_Post_post_urut] DEFAULT ((1)),
[post_pujk_view] [bit] NOT NULL CONSTRAINT [DF_Post_post_is_support_only] DEFAULT ((1)),
[post_konsumen_view] [bit] NOT NULL CONSTRAINT [DF_Thread_Post_post_pujk_view1] DEFAULT ((1)),
[post_last_edit_reason] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[kb_id] [bigint] NULL,
[kb_rate] [int] NULL,
[stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Post_stsrc] DEFAULT ('A'),
[created_by] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_created] [datetime] NULL,
[modified_by] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_modified] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Thread_Post] ADD CONSTRAINT [PK_Post] PRIMARY KEY CLUSTERED ([post_id]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Thread_Post] ADD CONSTRAINT [FK_Post_Thread] FOREIGN KEY ([thread_id]) REFERENCES [dbo].[Thread] ([thread_id])
GO
ALTER TABLE [dbo].[Thread_Post] ADD CONSTRAINT [FK_Post_User_Master] FOREIGN KEY ([post_user]) REFERENCES [dbo].[UserMaster] ([UserId])
GO
