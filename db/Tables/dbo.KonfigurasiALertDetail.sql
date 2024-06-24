CREATE TABLE [dbo].[KonfigurasiALertDetail]
(
[KadId] [bigint] NOT NULL IDENTITY(1, 1),
[KaId] [bigint] NOT NULL,
[UserId] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_KonfigurasiALertDetail_Stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL,
[KadIsRed] [bit] NOT NULL CONSTRAINT [DF_KonfigurasiALertDetail_KaIsRed] DEFAULT ((0)),
[KadIsYellow] [bit] NOT NULL CONSTRAINT [DF_KonfigurasiALertDetail_KaIsYellow] DEFAULT ((0))
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[KonfigurasiALertDetail] ADD CONSTRAINT [PK_KonfigurasiALertDetail] PRIMARY KEY CLUSTERED ([KadId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[KonfigurasiALertDetail] ADD CONSTRAINT [FK_KonfigurasiALertDetail_KonfigurasiAlert] FOREIGN KEY ([KaId]) REFERENCES [dbo].[KonfigurasiAlert] ([KaId])
GO
ALTER TABLE [dbo].[KonfigurasiALertDetail] ADD CONSTRAINT [FK_KonfigurasiALertDetail_UserMaster] FOREIGN KEY ([UserId]) REFERENCES [dbo].[UserMaster] ([UserId])
GO
