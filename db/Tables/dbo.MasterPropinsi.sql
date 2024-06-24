CREATE TABLE [dbo].[MasterPropinsi]
(
[RefPropinsiId] [bigint] NOT NULL IDENTITY(1, 1),
[RefPropinsiNama] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Entrier] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Ref_Propinsi_stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL,
[RefPropinsiKode] [varchar] (2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[JtiProvinceid] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[MasterPropinsi] ADD CONSTRAINT [PK_Ref_Propinsi] PRIMARY KEY CLUSTERED ([RefPropinsiId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
