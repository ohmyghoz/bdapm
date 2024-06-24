CREATE TABLE [dbo].[Log_ETL]
(
[log_id] [bigint] NOT NULL IDENTITY(1, 1),
[log_date] [datetime2] (0) NOT NULL,
[log_tipe] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[log_periode] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[log_delete_cnt] [bigint] NULL,
[log_insert_cnt] [bigint] NULL,
[log_start] [datetime] NULL,
[log_end] [datetime] NULL,
[log_status] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[log_errmessage] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Log_ETL] ADD CONSTRAINT [PK_Log_ETL2] PRIMARY KEY CLUSTERED ([log_id]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
