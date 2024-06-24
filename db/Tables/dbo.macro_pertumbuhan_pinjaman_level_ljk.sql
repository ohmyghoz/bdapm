CREATE TABLE [dbo].[macro_pertumbuhan_pinjaman_level_ljk]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_outstanding_sekarang] [decimal] (38, 6) NULL,
[dm_outstanding_sebelumnya] [decimal] (38, 6) NULL,
[dm_persen_pertumbuhan_outstanding] [decimal] (38, 6) NULL,
[dm_jenis_pertumbuhan] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_current_period] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_prev_period] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[macro_pertumbuhan_pinjaman_level_ljk] ADD CONSTRAINT [PK_macro_pertumbuhan_pinjaman_level_ljk] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
