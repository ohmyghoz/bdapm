CREATE TABLE [dim].[MEMBERS]
(
[Kode_Jenis_LJK] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Kode_LJK] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Nama_LJK] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Nama_Penanggung_Jawab_LJK] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Nomor_Telp_Penanggung_Jawab] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Alamat_Email_Penanggung_Jawab] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Kode_Kab_Kota_DATI_II] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Kecamatan] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Kelurahan] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Alamat] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Alamat_Website] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Tahun_bulan_data_Terakhir] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Status_Submission_Inisial] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Kode_Kantor_Cabang] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Status_Aktif] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Status_Delete] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Create_Date] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Update_Date] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
