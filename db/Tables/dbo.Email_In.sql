CREATE TABLE [dbo].[Email_In]
(
[emin_id] [bigint] NOT NULL IDENTITY(1, 1),
[emin_to] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Table_1_emailq_to] DEFAULT (''),
[emin_from] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Table_1_emailq_from] DEFAULT (''),
[emin_cc] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Table_1_emailq_cc] DEFAULT (''),
[emin_reply_to] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Table_1_emailq_reply_to] DEFAULT (''),
[emin_subject] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Table_1_emailq_subject] DEFAULT (''),
[emin_body] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Table_1_emailq_body] DEFAULT (''),
[emin_header] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Table_1_emin_body1] DEFAULT (''),
[emin_date] [datetime] NULL,
[emin_receive_date] [datetime] NULL,
[emin_is_bounce] [bit] NULL,
[emin_is_read] [bit] NOT NULL CONSTRAINT [DF_Email_In_emin_is_read] DEFAULT ((0)),
[emin_message_id] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[emin_bounce_message_id] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[emin_bounce_from] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[emin_bounce_subject] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[emin_unique_id] [bigint] NULL,
[emin_server] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[emin_attach_file_count] [int] NULL CONSTRAINT [DF_Email_In_emin_attach_file_count] DEFAULT ((0)),
[emin_is_spam] [bit] NULL CONSTRAINT [DF_Email_In_emin_is_spam] DEFAULT ((0)),
[stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Email_In_stsrc] DEFAULT ('A'),
[created_by] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_created] [datetime] NULL,
[modified_by] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_modified] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Email_In] ADD CONSTRAINT [PK_Email_In] PRIMARY KEY CLUSTERED ([emin_id]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
