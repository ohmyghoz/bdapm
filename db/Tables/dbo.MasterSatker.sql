CREATE TABLE [dbo].[MasterSatker]
(
[SatkerId] [bigint] NOT NULL IDENTITY(1, 1),
[SatkerKode] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[SatkerNama] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[SatkerTipe] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[SatkerParentId] [bigint] NULL,
[RefKotaId] [bigint] NULL,
[SatkerIsAudit] [bit] NULL,
[SatkerLevel] [smallint] NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Master_Satker_stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL,
[CamOrganizationCode] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CamOrganizationParentCode] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CamOrganizationLetterCode] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[MasterSatker] ADD CONSTRAINT [PK_Master_Satker] PRIMARY KEY CLUSTERED ([SatkerId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[MasterSatker] ADD CONSTRAINT [FK_Master_Satker_Ref_Kota] FOREIGN KEY ([RefKotaId]) REFERENCES [dbo].[MasterKota] ([RefKotaId])
GO
