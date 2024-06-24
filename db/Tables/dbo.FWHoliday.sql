CREATE TABLE [dbo].[FWHoliday]
(
[HolidayId] [bigint] NOT NULL IDENTITY(1, 1),
[HolidayDate] [datetime] NOT NULL,
[HolidayKeterangan] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWHoliday] ADD CONSTRAINT [PK_Holiday] PRIMARY KEY CLUSTERED ([HolidayId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
