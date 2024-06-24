CREATE TABLE [dbo].[debitur_anomali_duplikasi_nama_debitur]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_debitur] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_jenis_identitas_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_id_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_id_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tanggal_lahir] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_ibu_kandung] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_status_valid_id_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_similarity_score] [float] NULL,
[dm_similarity_result] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_identity_number_name_1] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tanggal_lahir_1] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_ibu_kandung_1] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_similarity_threshold] [float] NULL,
[dm_final_similarity_method] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_ljk_1] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk_1] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_cif_1] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
ALTER TABLE [dbo].[debitur_anomali_duplikasi_nama_debitur] ADD CONSTRAINT [PK_debitur_anomali_duplikasi_nama_debitur] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
