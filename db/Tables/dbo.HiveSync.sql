CREATE TABLE [dbo].[HiveSync]
(
[sync_id] [bigint] NOT NULL IDENTITY(1, 1),
[pprocess] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[pperiode] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[sync_status] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[HiveSync] ADD CONSTRAINT [PK_HiveSync] PRIMARY KEY CLUSTERED ([sync_id]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
