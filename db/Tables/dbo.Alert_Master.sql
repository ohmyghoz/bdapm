CREATE TABLE [dbo].[Alert_Master]
(
[KODE_ALERT] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[NAMA_ALERT] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[TIPE_PERIODE_CSV] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[DIMENSI1_NAME] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DIMENSI2_NAME] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DIMENSI3_NAME] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DIMENSI4_NAME] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DIMENSI5_NAME] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[created_by] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_created] [datetime] NULL,
[modified_by] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_modified] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Alert_Master] ADD CONSTRAINT [PK_Alert_Master] PRIMARY KEY CLUSTERED ([KODE_ALERT]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
