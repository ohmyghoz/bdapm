CREATE TABLE [dbo].[master_office_ljk]
(
[kode_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[kode_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[kode_kantor_cabang] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[kantor_cabang] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[parent_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[parent_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[parent_kantor_cabang] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[status_aktif] [varchar] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[status_delete] [varchar] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[create_date] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[update_date] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[p_date] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[sync_date] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[master_office_ljk] ADD CONSTRAINT [PK_master_office_ljk] PRIMARY KEY CLUSTERED ([kode_jenis_ljk], [kode_ljk], [kode_kantor_cabang]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
