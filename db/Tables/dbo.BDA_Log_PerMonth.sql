CREATE TABLE [dbo].[BDA_Log_PerMonth]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[member_type_code] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[member_code] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[periode] [date] NULL,
[user_id] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[max_act_count] [bigint] NULL,
[min_act_count] [bigint] NULL,
[avg_act_count] [float] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BDA_Log_PerMonth] ADD CONSTRAINT [PK_BDA_Log_PerMonth] PRIMARY KEY CLUSTERED ([rowid]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
