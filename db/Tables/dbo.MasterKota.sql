CREATE TABLE [dbo].[MasterKota]
(
[RefKotaId] [bigint] NOT NULL IDENTITY(1, 1),
[RefKotaNama] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[RefKotaDomisili] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Ref_Kota_stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL,
[RefPropinsiId] [bigint] NULL,
[JtiCityid] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[MasterKota] ADD CONSTRAINT [PK_Ref_Kota] PRIMARY KEY CLUSTERED ([RefKotaId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[MasterKota] ADD CONSTRAINT [FK_Ref_Kota_Ref_Propinsi] FOREIGN KEY ([RefPropinsiId]) REFERENCES [dbo].[MasterPropinsi] ([RefPropinsiId])
GO
