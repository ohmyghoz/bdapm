CREATE TABLE [dbo].[master_ljk]
(
[kode_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[kode_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[nama_ljk] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[nama_penanggung_jawab_ljk] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[nomor_telp_penanggung_jawab] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[alamat_email_penanggung_jawab] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[kode_kab] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[kecamatan] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[kelurahan] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[alamat] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[alamat_website] [varchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[tahun_bulan_data_terakhir] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[status_submission_inisial] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[kode_kantor_cabang] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[status_aktif] [varchar] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[status_delete] [varchar] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[create_date] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[update_date] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[p_date] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[sync_date] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[master_ljk] ADD CONSTRAINT [PK_master_ljk] PRIMARY KEY CLUSTERED ([kode_jenis_ljk], [kode_ljk]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
