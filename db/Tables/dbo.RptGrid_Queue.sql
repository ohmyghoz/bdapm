CREATE TABLE [dbo].[RptGrid_Queue]
(
[rgq_id] [bigint] NOT NULL IDENTITY(1, 1),
[rg_id] [bigint] NULL,
[rgq_query] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[rgq_params] [varchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[rgq_nama] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[rgq_requestor] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[rgq_date] [datetime] NOT NULL,
[rgq_start] [datetime] NULL,
[rgq_end] [datetime] NULL,
[rgq_status] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[rgq_priority] [tinyint] NOT NULL,
[rgq_urut] [int] NOT NULL CONSTRAINT [DF_RptGrid_Queue_rgq_urut] DEFAULT ((1000)),
[rgq_error_message] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[rgq_result_filesize] [int] NULL,
[rgq_result_filename] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[rgq_result_rowcount] [int] NULL,
[stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_RptGrid_Queue_stsrc] DEFAULT ('A'),
[created_by] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_created] [datetime] NULL,
[modified_by] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_modified] [datetime] NULL,
[rgq_tablename] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
ALTER TABLE [dbo].[RptGrid_Queue] ADD CONSTRAINT [PK_RptGrid_Queue] PRIMARY KEY CLUSTERED  ([rgq_id])
GO
ALTER TABLE [dbo].[RptGrid_Queue] ADD CONSTRAINT [FK_RptGrid_Queue_RptGrid] FOREIGN KEY ([rg_id]) REFERENCES [dbo].[RptGrid] ([rg_id])
GO
