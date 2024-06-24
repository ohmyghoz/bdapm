CREATE TABLE [dbo].[Master_Agunan]
(
[jenis_agunan] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[nama_agunan] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[sync_date] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Master_Agunan] ADD CONSTRAINT [PK_Master_Agunan] PRIMARY KEY CLUSTERED ([jenis_agunan]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
