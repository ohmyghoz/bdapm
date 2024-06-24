CREATE TABLE [dbo].[agunan_id_anomali]
(
[dm_periode] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[dm_jenis_ljk] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_kode_ljk] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_cif] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_nama_debitur] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_no_agunan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_jenis_agunan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_dokumen_kepemilikan_agunan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_lokasi_agunan] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_status_paripasu] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[dm_persen_paripasu] [float] NULL,
[dm_similarity_score] [float] NULL,
[dm_similarity_result] [int] NULL
)
GO
ALTER TABLE [dbo].[agunan_id_anomali] ADD CONSTRAINT [PK_agunan_id_anomali] PRIMARY KEY CLUSTERED  ([dm_periode], [rowid])
GO
