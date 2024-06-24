CREATE TABLE [dbo].[FWKodeCounter]
(
[KdcnId] [bigint] NOT NULL IDENTITY(1, 1),
[KofId] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[KdcnPrefix] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[KdcnCounter] [int] NOT NULL CONSTRAINT [DF_New_Kode_Counter_kdcn_counter] DEFAULT ((1)),
[KdcnLast] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[KdcnLastUpdate] [datetime] NOT NULL,
[KdcnLastReset] [datetime] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWKodeCounter] ADD CONSTRAINT [PK_New_Kode_Counter] PRIMARY KEY CLUSTERED ([KdcnId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWKodeCounter] ADD CONSTRAINT [FK_New_Kode_Counter_New_Kode_Format] FOREIGN KEY ([KofId]) REFERENCES [dbo].[FWKodeFormat] ([KofId])
GO
