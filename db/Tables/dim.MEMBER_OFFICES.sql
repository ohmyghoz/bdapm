CREATE TABLE [dim].[MEMBER_OFFICES]
(
[Kode_Jenis_LJK] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Kode_LJK] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Kode_Kantor_Cabang] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Kantor_Cabang] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Parent_LJK] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Parent_Jenis_LJK] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Parent_Kantor_Cabang] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Status_Aktif] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Status_Delete] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Create_Date] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Update_Date] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
