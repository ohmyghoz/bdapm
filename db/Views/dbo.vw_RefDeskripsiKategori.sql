SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE VIEW [dbo].[vw_RefDeskripsiKategori]
AS
SELECT dm_deskripsi_jenis_penggunaan AS desk,'Penggunaan Kredit' AS tipe FROM dbo.ref_credit_usages
UNION
SELECT dm_deskripsi_kategori AS desk ,'Kategori Debitur' AS tipe FROM dbo.ref_kategori_debitur_temp
GO
