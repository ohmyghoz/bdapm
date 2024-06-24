CREATE TABLE [dbo].[pengawas_ljk]
(
[user_login_id] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[orgn_name] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[employee_id] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[phone_number] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[group_name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[active_flag] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[member_type_code] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[member_code] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[created_datetime] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[created_by] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[updated_datetime] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[updated_by] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[p_date] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[sync_date] [datetime] NULL
) ON [PRIMARY]
GO
