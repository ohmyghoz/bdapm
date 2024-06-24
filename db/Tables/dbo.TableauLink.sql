CREATE TABLE [dbo].[TableauLink]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[name] [varchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[type] [varchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[link] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[created_by] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[modified_by] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_created] [datetime] NULL,
[date_modified] [datetime] NULL,
[date_update] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TableauLink] ADD CONSTRAINT [PK__TableauL__3213E83F1A7CA89F] PRIMARY KEY CLUSTERED ([id]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
