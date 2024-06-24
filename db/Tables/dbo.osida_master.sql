CREATE TABLE [dbo].[osida_master]
(
[kode] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[judul] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[skenario] [varchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[output] [varchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[output_empty] [varchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[tindaklanjut] [varchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[logic] [varchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_osida_master_Stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL,
[menu_nama] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
ALTER TABLE [dbo].[osida_master] ADD CONSTRAINT [PK_osida_master] PRIMARY KEY CLUSTERED  ([kode])
GO
