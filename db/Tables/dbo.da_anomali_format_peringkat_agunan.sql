CREATE TABLE [dbo].[da_anomali_format_peringkat_agunan]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_bulan_data] [date] NOT NULL,
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_kantor_cabang] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_kantor_cabang] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_rekening] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_id_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_reg_agunan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_status_agunan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_agunan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_peringkat_agunan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_lembaga_pemeringkat] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_anomali] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[da_anomali_format_peringkat_agunan] ADD CONSTRAINT [PK_da_anomali_format_peringkat_agunan] PRIMARY KEY CLUSTERED ([rowid], [dm_bulan_data]) ON [PRIMARY]
GO
