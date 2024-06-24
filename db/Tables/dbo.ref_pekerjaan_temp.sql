CREATE TABLE [dbo].[ref_pekerjaan_temp]
(
[dm_kode_profesi_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[dm_profesi_debitur] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ref_pekerjaan_temp] ADD CONSTRAINT [PK_ref_pekerjaan_temp] PRIMARY KEY CLUSTERED ([dm_kode_profesi_debitur]) ON [PRIMARY]
GO
