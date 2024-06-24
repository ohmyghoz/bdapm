SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO



CREATE VIEW [dbo].[vw_getMasterLJK] AS
SELECT l.kode_jenis_ljk + ' - ' + kode_ljk AS CompositeKey, kode_ljk + ' - ' + nama_ljk AS Display,lt.deskripsi_jenis_ljk, l.* FROM master_ljk l
LEFT JOIN dbo.master_ljk_type lt ON lt.kode_jenis_ljk = l.kode_jenis_ljk


GO
