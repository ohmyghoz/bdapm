SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getGroupedRJPOtherLJK]
	@member_type_code VARCHAR(4) = NULL,
	@member_code VARCHAR(6) = NULL,
	@member_type_code2 VARCHAR(4) = NULL,
	@member_code2 VARCHAR(4) = NULL,
	@tipe_periode VARCHAR(20) = NULL,
	@dimensi1 VARCHAR(50) = NULL,
	@dimensi2 VARCHAR(50) = NULL,
	@periode_from DATE = NULL,
	@periode_until DATE = NULL,
	@level_alert VARCHAR(50)

--DECLARE @member_type_code VARCHAR(4)='0101'
--DECLARE @member_code VARCHAR(6)= '016'
--DECLARE @member_type_code2 VARCHAR(4) = '0101'
--DECLARE @member_code2 VARCHAR(4)= '008'
--DECLARE @tipe_periode VARCHAR(20)= 'Harian'
--DECLARE @dimensi1 VARCHAR(50)= 'Total Permintaan'
--DECLARE @dimensi2 VARCHAR(50)= 'Mingguan'
--DECLARE @periode_from DATE= '2019-01-01'
--DECLARE @periode_until DATE= '2019-03-31'

AS
BEGIN
DECLARE @temp_levelAlert AS TABLE(split VARCHAR(200))
	INSERT INTO @temp_levelAlert SELECT LTRIM(RTRIM(split)) FROM dbo.Split(',', @level_alert)

DECLARE @temp TABLE(
	rowid BIGINT,
	tgl DATE,
	NILAI1 DECIMAL(38,6),
	avg1 DECIMAL(38,6),
	avg2 DECIMAL(38,6),
	ratio  DECIMAL(38,6),
	LEVEL_ALERT VARCHAR(5)
)

INSERT @temp
(
	rowid,
    tgl,
    NILAI1,
    avg1,
    avg2,
    ratio,
    LEVEL_ALERT
)
EXEC dbo.usp_getRJPSelfVsOtherLJK @member_type_code = @member_type_code,       -- varchar(4)
                                  @member_code = @member_code,            -- varchar(6)
                                  @member_type_code2 = @member_type_code2,      -- varchar(4)
                                  @member_code2 = @member_code2,           -- varchar(4)
                                  @tipe_periode = @tipe_periode,           -- varchar(20)
                                  @dimensi1 = @dimensi1,               -- varchar(50)
                                  @dimensi2 = @dimensi2,               -- varchar(50)
                                  @periode_from = @periode_from, -- date
                                  @periode_until = @periode_until -- date


SELECT KODE_ALERT = 'RJP_Rata2_LJK_Lain', NAMA_ALERT = 'Rata-Rata Jumlah Permintaan - Versus LJK Lain', TIPE_PERIODE = @tipe_periode, MEMBER_TYPE_CODE = @member_type_code, JenisLJK = @member_type_code + ' - ' + lt.deskripsi_jenis_ljk, MEMBER_CODE = @member_code, LJK = @member_code + ' - ' + l.nama_ljk, Frekuensi = @dimensi2,
	SUM(CASE WHEN LEVEL_ALERT = '3' THEN 1 ELSE 0 END) AS RED,
	SUM(CASE WHEN LEVEL_ALERT = '2' THEN 1 ELSE 0 END) AS YELLOW,
	SUM(CASE WHEN LEVEL_ALERT = '1' THEN 1 ELSE 0 END) AS GREEN,
	SUM(CASE WHEN LEVEL_ALERT = '0' THEN 1 ELSE 0 END) AS GREY FROM @temp a
	LEFT JOIN dbo.master_ljk_type lt ON lt.kode_jenis_ljk = @member_type_code
	LEFT JOIN dbo.master_ljk l ON l.kode_jenis_ljk = lt.kode_jenis_ljk AND l.kode_ljk = @member_code
	WHERE a.LEVEL_ALERT IN (SELECT split FROM @temp_levelAlert)
	GROUP BY lt.deskripsi_jenis_ljk, l.nama_ljk

END
GO
