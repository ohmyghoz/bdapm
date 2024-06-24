CREATE TABLE [dbo].[Alert_Summary]
(
[PERIODE] [date] NOT NULL,
[rowid] [bigint] NOT NULL IDENTITY(1, 1),
[KODE_ALERT] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[TIPE_PERIODE] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[MEMBER_TYPE_CODE] [varchar] (6) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[MEMBER_CODE] [varchar] (6) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[OFFICE_CODE] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DIMENSI1] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DIMENSI2] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DIMENSI3] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DIMENSI4] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DIMENSI5] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[LEVEL_ALERT] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[NILAI1] [decimal] (38, 6) NULL,
[NILAI_PEMBANDING1] [decimal] (38, 6) NULL,
[NILAI2] [decimal] (38, 6) NULL,
[NILAI_PEMBANDING2] [decimal] (38, 6) NULL,
[stsrc] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Alert_Summary_stsrc] DEFAULT ('A'),
[created_by] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_created] [datetime] NULL,
[modified_by] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[date_modified] [datetime] NULL
) ON [PartitionTable_ByYearMonthDateScheme_BDAP] ([PERIODE])
GO
ALTER TABLE [dbo].[Alert_Summary] ADD CONSTRAINT [PK_Alert_Summary2] PRIMARY KEY CLUSTERED ([PERIODE], [rowid]) WITH (FILLFACTOR=80) ON [PartitionTable_ByYearMonthDateScheme_BDAP] ([PERIODE])
GO
CREATE NONCLUSTERED INDEX [idx_Alert_Summary] ON [dbo].[Alert_Summary] ([PERIODE], [KODE_ALERT], [TIPE_PERIODE], [DIMENSI1], [MEMBER_TYPE_CODE], [MEMBER_CODE]) WITH (FILLFACTOR=80) ON [PartitionTable_ByYearMonthDateScheme_BDAP] ([PERIODE])
GO
