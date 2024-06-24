CREATE TABLE [dbo].[FWCalendar]
(
[CalendarId] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[CalendarDate] [datetime] NOT NULL,
[CalendarDayName] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[CalendarMonthName] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[CalendarYear] [smallint] NOT NULL,
[CalendarDay] [tinyint] NOT NULL,
[CalendarMonth] [tinyint] NOT NULL,
[CalendarQuarter] [tinyint] NOT NULL,
[CalendarHoliday] [bit] NOT NULL CONSTRAINT [DF_Calendar_calendar_holiday] DEFAULT ((0)),
[CalendarHolidayKeterangan] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CalendarType] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Calendar_calendar_type] DEFAULT ('Standard')
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FWCalendar] ADD CONSTRAINT [PK_Calendar] PRIMARY KEY CLUSTERED ([CalendarId]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
