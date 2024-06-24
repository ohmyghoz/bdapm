CREATE TABLE [dbo].[BDA_LogOutliers_Avg]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[member_type_code] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[member_code] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[periode] [date] NOT NULL,
[status_avg_activity] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[user_id] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[mean_diff] [float] NULL,
[above_threshold] [float] NULL,
[below_threshold] [float] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BDA_LogOutliers_Avg] ADD CONSTRAINT [PK_BDA_LogOutliers_Avg] PRIMARY KEY CLUSTERED ([periode], [rowid]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [idx_BDA_LogOutliers_Avg] ON [dbo].[BDA_LogOutliers_Avg] ([periode], [member_type_code], [member_code], [user_id]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
