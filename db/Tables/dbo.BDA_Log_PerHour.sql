CREATE TABLE [dbo].[BDA_Log_PerHour]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[member_type_code] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[member_code] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[periode] [date] NULL,
[hour] [int] NULL,
[cnt_user_id] [bigint] NULL,
[cnt_act] [bigint] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BDA_Log_PerHour] ADD CONSTRAINT [PK_BDA_Log_PerHour] PRIMARY KEY CLUSTERED ([rowid]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
