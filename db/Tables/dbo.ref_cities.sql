CREATE TABLE [dbo].[ref_cities]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_kode_kota] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_nama_kota] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_pos] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_provinsi] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_negara] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_status_aktif] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_status_delete] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_create_date] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_update_date] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_reference_date] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ref_cities] ADD CONSTRAINT [PK_ref_cities] PRIMARY KEY CLUSTERED ([rowid]) ON [PRIMARY]
GO
