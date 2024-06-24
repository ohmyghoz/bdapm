CREATE TABLE [dbo].[DimTime]
(
[PK_Date] [datetime] NOT NULL,
[Date_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Year] [datetime] NULL,
[Year_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Half_Year] [datetime] NULL,
[Half_Year_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Quarter] [datetime] NULL,
[Quarter_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Trimester] [datetime] NULL,
[Trimester_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Month] [datetime] NULL,
[Month_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Week] [datetime] NULL,
[Week_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Day_Of_Year] [int] NULL,
[Day_Of_Year_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Day_Of_Half_Year] [int] NULL,
[Day_Of_Half_Year_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Day_Of_Trimester] [int] NULL,
[Day_Of_Trimester_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Day_Of_Quarter] [int] NULL,
[Day_Of_Quarter_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Day_Of_Month] [int] NULL,
[Day_Of_Month_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Day_Of_Week] [int] NULL,
[Day_Of_Week_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Week_Of_Year] [int] NULL,
[Week_Of_Year_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Month_Of_Year] [int] NULL,
[Month_Of_Year_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Month_Of_Half_Year] [int] NULL,
[Month_Of_Half_Year_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Month_Of_Trimester] [int] NULL,
[Month_Of_Trimester_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Month_Of_Quarter] [int] NULL,
[Month_Of_Quarter_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Quarter_Of_Year] [int] NULL,
[Quarter_Of_Year_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Quarter_Of_Half_Year] [int] NULL,
[Quarter_Of_Half_Year_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Trimester_Of_Year] [int] NULL,
[Trimester_Of_Year_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Half_Year_Of_Year] [int] NULL,
[Half_Year_Of_Year_Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DimTime] ADD CONSTRAINT [PK_DimTime] PRIMARY KEY CLUSTERED ([PK_Date]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', NULL, NULL
GO
EXEC sp_addextendedproperty N'DSVTable', N'DimTime', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', NULL, NULL
GO
EXEC sp_addextendedproperty N'Project', N'23ade6a0-4a53-49d1-a2c4-7e61db69b999', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', NULL, NULL
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Date_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Date_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Date_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Half_Year'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Day_Of_Half_Year', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Half_Year'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Half_Year_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Day_Of_Half_Year_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Half_Year_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Month'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Day_Of_Month', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Month'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Month_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Day_Of_Month_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Month_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Quarter'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Day_Of_Quarter', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Quarter'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Quarter_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Day_Of_Quarter_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Quarter_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Trimester'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Day_Of_Trimester', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Trimester'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Trimester_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Day_Of_Trimester_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Trimester_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Week'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Day_Of_Week', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Week'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Week_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Day_Of_Week_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Week_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Year'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Day_Of_Year', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Year'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Year_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Day_Of_Year_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Day_Of_Year_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Half_Year'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Half_Year', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Half_Year'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Half_Year_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Half_Year_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Half_Year_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Half_Year_Of_Year'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Half_Year_Of_Year', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Half_Year_Of_Year'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Half_Year_Of_Year_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Half_Year_Of_Year_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Half_Year_Of_Year_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Month', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Month_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Half_Year'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Month_Of_Half_Year', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Half_Year'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Half_Year_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Month_Of_Half_Year_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Half_Year_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Quarter'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Month_Of_Quarter', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Quarter'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Quarter_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Month_Of_Quarter_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Quarter_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Trimester'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Month_Of_Trimester', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Trimester'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Trimester_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Month_Of_Trimester_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Trimester_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Year'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Month_Of_Year', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Year'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Year_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Month_Of_Year_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Month_Of_Year_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'PK_Date'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Date', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'PK_Date'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Quarter'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Quarter', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Quarter'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Quarter_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Quarter_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Quarter_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Quarter_Of_Half_Year'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Quarter_Of_Half_Year', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Quarter_Of_Half_Year'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Quarter_Of_Half_Year_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Quarter_Of_Half_Year_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Quarter_Of_Half_Year_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Quarter_Of_Year'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Quarter_Of_Year', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Quarter_Of_Year'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Quarter_Of_Year_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Quarter_Of_Year_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Quarter_Of_Year_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Trimester'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Trimester', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Trimester'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Trimester_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Trimester_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Trimester_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Trimester_Of_Year'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Trimester_Of_Year', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Trimester_Of_Year'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Trimester_Of_Year_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Trimester_Of_Year_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Trimester_Of_Year_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Week'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Week', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Week'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Week_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Week_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Week_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Week_Of_Year'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Week_Of_Year', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Week_Of_Year'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Week_Of_Year_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Week_Of_Year_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Week_Of_Year_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Year'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Year', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Year'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Year_Name'
GO
EXEC sp_addextendedproperty N'DSVColumn', N'Year_Name', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'COLUMN', N'Year_Name'
GO
EXEC sp_addextendedproperty N'AllowGen', N'True', 'SCHEMA', N'dbo', 'TABLE', N'DimTime', 'CONSTRAINT', N'PK_DimTime'
GO
