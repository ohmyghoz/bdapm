CREATE TABLE [dbo].[FWKodeDetail]
(
[KodId] [bigint] NOT NULL IDENTITY(1, 1),
[KofId] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[KodTipe] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[KodUrut] [int] NOT NULL,
[KodLength] [tinyint] NULL,
[KodCatatan] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[KodChar] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[KodParamKode] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[KodParamAsCounter] [bit] NOT NULL CONSTRAINT [DF_New_Kode_Detail_kod_param_as_counter] DEFAULT ((0))
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWKodeDetail] ADD CONSTRAINT [PK_New_Kode_Detail] PRIMARY KEY CLUSTERED ([KodId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWKodeDetail] ADD CONSTRAINT [FK_New_Kode_Detail_New_Kode_Format] FOREIGN KEY ([KofId]) REFERENCES [dbo].[FWKodeFormat] ([KofId])
GO
