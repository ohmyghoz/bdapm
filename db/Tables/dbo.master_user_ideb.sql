CREATE TABLE [dbo].[master_user_ideb]
(
[user_id] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[user_login_id] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[user_name] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[phone_number] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[user_type_flag] [varchar] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[member_type_code] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[member_code] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[office_code] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[employee_id] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[password_latest_changed_date] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[password_lock_count] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[user_latest_login_date] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[active_flag] [varchar] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[created_by] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[created_datetime] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[updated_by] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[updated_datetime] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[user_status_flag] [varchar] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[p_date] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[sync_date] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[master_user_ideb] ADD CONSTRAINT [PK_master_user_ideb] PRIMARY KEY CLUSTERED ([user_id]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20211106-021406] ON [dbo].[master_user_ideb] ([user_id], [member_type_code], [member_code]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
