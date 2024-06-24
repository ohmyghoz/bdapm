CREATE TABLE [dbo].[ms_pinjaman_baru_bersamaan_sum]
(
[dm_kode_ljk_filter] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_total_account] [int] NULL,
[dm_total_plafon] [decimal] (38, 6) NULL,
[dm_total_outstanding] [decimal] (38, 6) NULL,
[dm_list_ljk] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kolektibilitas] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ms_pinjaman_baru_bersamaan_sum] ADD CONSTRAINT [PK_ms_pinjaman_baru_bersamaan_sum] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) ON [PRIMARY]
GO
