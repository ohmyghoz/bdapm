CREATE TABLE [dbo].[da_anomali_baki_debet_tidak_wajar]
(
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_bulan_data] [date] NOT NULL,
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_kantor_cabang] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_kantor_cabang] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_baki_debet] [decimal] (38, 6) NULL,
[dm_baki_debet_m1] [decimal] (38, 6) NULL,
[dm_selisih_baki_debet_m1] [decimal] (38, 6) NULL,
[dm_%selisih_baki_debet_m1] [decimal] (38, 6) NULL,
[dm_baki_debet_m3] [decimal] (38, 6) NULL,
[dm_selisih_baki_debet_m3] [decimal] (38, 6) NULL,
[dm_%selisih_baki_debet_m3] [decimal] (38, 6) NULL,
[dm_baki_debet_m6] [decimal] (38, 6) NULL,
[dm_selisih_baki_debet_m6] [decimal] (38, 6) NULL,
[dm_%selisih_baki_debet_m6] [decimal] (38, 6) NULL,
[dm_baki_debet_m12] [decimal] (38, 6) NULL,
[dm_selisih_baki_debet_m12] [decimal] (38, 6) NULL,
[dm_%selisih_baki_debet_m12] [decimal] (38, 6) NULL,
[dm_segmen] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_anomali] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[da_anomali_baki_debet_tidak_wajar] ADD CONSTRAINT [PK_da_anomali_baki_debet_tidak_wajar] PRIMARY KEY CLUSTERED ([rowid], [dm_bulan_data]) ON [PRIMARY]
GO
