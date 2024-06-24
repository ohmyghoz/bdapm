CREATE TABLE [dbo].[BDA_LogOutliers_Act]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[member_type_code] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[member_code] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[periode] [date] NOT NULL,
[status_count_activity] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[user_id] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[act_count] [bigint] NULL,
[above_threshold] [float] NULL,
[below_threshold] [float] NULL
) ON [PartitionTable_ByYearMonthDateScheme_BDAP] ([periode])
GO
ALTER TABLE [dbo].[BDA_LogOutliers_Act] ADD CONSTRAINT [PK_BDA_LogOutliers_Act] PRIMARY KEY CLUSTERED ([periode], [rowid]) WITH (FILLFACTOR=80) ON [PartitionTable_ByYearMonthDateScheme_BDAP] ([periode])
GO
CREATE NONCLUSTERED INDEX [idx_BDA_LogOutleirs_Act] ON [dbo].[BDA_LogOutliers_Act] ([periode], [member_type_code], [member_code], [user_id]) WITH (FILLFACTOR=80) ON [PartitionTable_ByYearMonthDateScheme_BDAP] ([periode])
GO
