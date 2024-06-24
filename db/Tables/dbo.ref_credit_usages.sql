CREATE TABLE [dbo].[ref_credit_usages]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_kode_jenis_penggunaan] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_deskripsi_jenis_penggunaan] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_status_aktif] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_status_delete] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_create_date] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_update_date] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_reference_date] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ref_credit_usages] ADD CONSTRAINT [PK_ref_credit_usages] PRIMARY KEY CLUSTERED ([rowid]) ON [PRIMARY]
GO
