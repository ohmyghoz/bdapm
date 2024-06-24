CREATE TABLE [dbo].[Help]
(
[help_id] [bigint] NOT NULL IDENTITY(1, 1),
[help_urut] [int] NOT NULL,
[help_nama] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[help_catatan] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[help_last_update] [datetime] NULL,
[help_last_update_by] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Help_stsrc] DEFAULT ('A'),
[created_by] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_created] [datetime] NULL,
[modified_by] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_modified] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Help] ADD CONSTRAINT [PK_Help] PRIMARY KEY CLUSTERED ([help_id]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
