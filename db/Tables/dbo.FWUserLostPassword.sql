CREATE TABLE [dbo].[FWUserLostPassword]
(
[UlpId] [bigint] NOT NULL IDENTITY(1, 1),
[UlpDate] [datetime] NOT NULL,
[UserId] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[UlpExpireDate] [datetime] NOT NULL,
[UlpEmail] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[UlpCode] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[UlpStatus] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[UlpResetDate] [datetime] NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_User_Lost_Password_stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWUserLostPassword] ADD CONSTRAINT [PK_User_Lost_Password] PRIMARY KEY CLUSTERED ([UlpId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWUserLostPassword] ADD CONSTRAINT [FK_User_Lost_Password_User_Master] FOREIGN KEY ([UserId]) REFERENCES [dbo].[UserMaster] ([UserId])
GO
