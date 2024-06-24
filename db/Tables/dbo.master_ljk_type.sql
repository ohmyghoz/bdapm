CREATE TABLE [dbo].[master_ljk_type]
(
[kode_jenis_ljk] [varchar] (4) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[deskripsi_jenis_ljk] [varchar] (37) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[status_aktif] [varchar] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[status_delete] [varchar] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[create_date] [varchar] (14) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[update_date] [varchar] (14) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[p_date] [varchar] (6) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[sync_date] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[master_ljk_type] ADD CONSTRAINT [PK_master_ljk_type] PRIMARY KEY CLUSTERED ([kode_jenis_ljk]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
