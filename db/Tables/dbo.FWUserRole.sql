CREATE TABLE [dbo].[FWUserRole]
(
[UroleId] [bigint] NOT NULL IDENTITY(1, 1),
[UserId] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[RoleId] [bigint] NOT NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_FW_User_Role_stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL
) ON [PRIMARY]
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author:		Yulius
-- Create date: 2013-10-02
-- Description:	Isi Roles CSV setiap kali perubahan
-- =============================================
CREATE TRIGGER [dbo].[trig_userRolesCsv]
   ON  [dbo].[FWUserRole]
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE dbo.UserMaster SET UserRolesCsv = roles
    FROM (SELECT UserId, roles = 
		STUFF((SELECT DISTINCT CASE WHEN RTRIM(COALESCE(b.RoleId,'')) = '' THEN '' ELSE ', ' + COALESCE(RoleId,'') END 
			   FROM dbo.FWUserRole b
			   WHERE b.Stsrc = 'A' AND a.UserId = b.UserId 
			  FOR XML PATH('')), 1, 2, '')
		FROM dbo.UserMaster a
	) x
	INNER JOIN dbo.UserMaster y ON x.UserId = y.UserId
	AND (y.UserId IN (SELECT UserId FROM INSERTED)
	     OR y.UserId IN (SELECT UserId FROM DELETED))

END
GO
DISABLE TRIGGER [dbo].[trig_userRolesCsv] ON [dbo].[FWUserRole]
GO
ALTER TABLE [dbo].[FWUserRole] ADD CONSTRAINT [PK_New_User_Role] PRIMARY KEY CLUSTERED ([UroleId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWUserRole] ADD CONSTRAINT [FK_FW_User_Role_User_Master] FOREIGN KEY ([UserId]) REFERENCES [dbo].[UserMaster] ([UserId])
GO
