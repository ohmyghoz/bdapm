CREATE TABLE [dbo].[Master_Keterangan]
(
[mk_id] [bigint] NOT NULL IDENTITY(1, 1),
[mk_kode] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[mk_keterangan] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[mk_menu] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Master_Keterangan_Stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL,
[mk_deskripsi_export] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Master_Keterangan] ADD CONSTRAINT [PK_Master_Keterangan] PRIMARY KEY CLUSTERED ([mk_id]) ON [PRIMARY]
GO
