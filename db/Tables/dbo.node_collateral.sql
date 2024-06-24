CREATE TABLE [dbo].[node_collateral]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_node_id] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_node_type] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_node_subtype] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_number_of_credit_facility_type] [int] NULL,
[dm_has_joint_credit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_number_of_ljk] [bigint] NULL,
[dm_number_of_cif] [bigint] NULL,
[dm_number_of_credit_acc] [bigint] NULL,
[dm_number_of_active_credit_acc] [bigint] NULL,
[dm_value_from] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_value] [decimal] (38, 6) NULL,
[dm_node_size_factor] [float] NULL,
[dm_node_color_type] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
ALTER TABLE [dbo].[node_collateral] ADD CONSTRAINT [PK_node_collateral] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
