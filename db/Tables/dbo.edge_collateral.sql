CREATE TABLE [dbo].[edge_collateral]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_relationship] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_first_node_id] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_second_node_id] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_relationship_status] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_number_of_facility_type] [int] NULL,
[dm_has_joint_credit] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_number_of_associated_rel] [bigint] NULL,
[dm_number_of_active_associated_rel] [bigint] NULL,
[dm_estimated_subtotal_active_collateral_value] [decimal] (38, 6) NULL,
[dm_estimated_subtotal_active_joint_collateral_value] [decimal] (38, 6) NULL,
[dm_active_outstanding] [decimal] (38, 6) NULL,
[dm_active_joint_outstanding] [decimal] (38, 6) NULL,
[dm_edge_width_factor] [float] NULL,
[dm_edge_color_factor] [float] NULL
)
GO
ALTER TABLE [dbo].[edge_collateral] ADD CONSTRAINT [PK_edge_collateral] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
