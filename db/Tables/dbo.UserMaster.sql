CREATE TABLE [dbo].[UserMaster]
(
[UserId] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[UserKode] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UserLdap] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UserNama] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[UserAlamat] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UserTelp] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UserPassword] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[UserLastLogin] [datetime] NULL,
[UserEmail] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UserMainRole] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UserDivisi] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UserPasswordLastchange] [datetime] NULL,
[UserFailedLoginCount] [int] NOT NULL CONSTRAINT [DF_User_Master_user_failed_login] DEFAULT ((0)),
[UserBlockedDate] [datetime] NULL,
[UserStatus] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_User_Master_user_status] DEFAULT ('Aktif'),
[UserLdapDepartment] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UserLdapOffice] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UserLdapDescription] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UserRolesCsv] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_User_Master2_stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL,
[CamUserId] [bigint] NULL,
[SatkerId] [bigint] NULL,
[IpAddress] [varchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UserNoticeLastRead] [datetime] NULL,
[UserAgent] [varchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[LastTimeCookies] [datetime] NULL,
[user_is_notifredalert] [bit] NULL CONSTRAINT [DF_UserMaster_user_is_notifredalert] DEFAULT ((0)),
[user_is_notifyellowalert] [bit] NULL CONSTRAINT [DF_UserMaster_user_is_notifredalert1] DEFAULT ((0))
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[UserMaster] ADD CONSTRAINT [PK_User_Master2] PRIMARY KEY CLUSTERED ([UserId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[UserMaster] ADD CONSTRAINT [FK_User_Master_Master_Satker] FOREIGN KEY ([SatkerId]) REFERENCES [dbo].[MasterSatker] ([SatkerId])
GO
