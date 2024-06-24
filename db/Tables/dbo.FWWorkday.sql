CREATE TABLE [dbo].[FWWorkday]
(
[WorkdayId] [bigint] NOT NULL IDENTITY(1, 1),
[WorkdayYear] [smallint] NOT NULL,
[WorkdayDayName] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[WorkdayStart] [datetime] NULL,
[WorkdayEnd] [datetime] NULL,
[Stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Workday_stsrc] DEFAULT ('A'),
[CreatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedDatetime] [datetime] NULL,
[UpdatedBy] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UpdatedDatetime] [datetime] NULL,
[WorkdayDay] [tinyint] NULL,
[CalendarType] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Workday_calendar_type] DEFAULT ('Standard')
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWWorkday] ADD CONSTRAINT [PK_Workday] PRIMARY KEY CLUSTERED ([WorkdayId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
