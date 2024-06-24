CREATE TABLE [dbo].[FWRefSetting]
(
[SetName] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[SetType] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[SetValue] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[SetCatatan] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[SetGroup] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[SetForuser] [bit] NOT NULL CONSTRAINT [DF_Ref_Setting_set_foruser] DEFAULT ((0))
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWRefSetting] ADD CONSTRAINT [PK_Ref_Setting_1] PRIMARY KEY CLUSTERED ([SetName]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
