CREATE TABLE [dbo].[ma_analisis_pencurian_identitas]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_rekening] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_debitur] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_kode_jenis_id_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_id_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_id_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_ibu_kandung] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_tanggal_lahir] [date] NULL,
[dm_status_id_debitur_valid] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_id_debitur_2] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_similarity_score] [float] NULL,
[dm_similarity_result] [int] NULL,
[dm_kategori_asesmen] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ma_analisis_pencurian_identitas] ADD CONSTRAINT [PK_ma_analisis_pencurian_identitas] PRIMARY KEY CLUSTERED ([dm_periode], [rowid]) ON [PRIMARY]
GO
