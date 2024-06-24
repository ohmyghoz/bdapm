SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO






CREATE VIEW [dbo].[vw_getMasterOfficeLJK] AS
SELECT ol.kode_jenis_ljk + ' - ' + ol.kode_ljk + ' - ' + ol.kode_kantor_cabang AS CompositeKey, ol.kode_ljk + ' - ' + ol.kode_kantor_cabang + ' - ' + ol.kantor_cabang AS Display,lt.deskripsi_jenis_ljk,l.nama_ljk, ol.* FROM dbo.master_office_ljk ol
LEFT JOIN dbo.master_ljk_type lt ON lt.kode_jenis_ljk = ol.kode_jenis_ljk
LEFT JOIN dbo.master_ljk l ON l.kode_ljk = ol.kode_ljk AND l.kode_jenis_ljk = lt.kode_jenis_ljk


GO
