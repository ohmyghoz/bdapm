SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

CREATE PROCEDURE [dbo].[usp_GetDataAlert]
    @userID VARCHAR(150),
    @lvlAlert VARCHAR(50),
    @periode DATETIME,
    @tipePeriode VARCHAR(50),
    @uID VARCHAR(250)
AS

DECLARE @linkPrefix VARCHAR(1000);
SET @linkPrefix = '';
SELECT @linkPrefix
    = SetValue + '/Dashboard/SLIK?lvl=' + @lvlAlert + '&periode=' + CONVERT(VARCHAR, @periode, 112) + '&tperiode='
      + @tipePeriode + '&uid=' + @uID + '&tipe='
FROM dbo.FWRefSetting
WHERE SetName = 'Link_Prefix';

SELECT A.tipe,
       COUNT(*) AS jumlahAlert,
	   '<a href="' + @linkPrefix + A.tipe2 + '&jns=tp">'+ CONVERT(VARCHAR(50), SUM(A.tp)) + '</a>' AS tp,
	   '<a href="' + @linkPrefix + A.tipe2 + '&jns=ind">'+ CONVERT(VARCHAR(50), SUM(A.ind)) + '</a>' AS ind,
	   '<a href="' + @linkPrefix + A.tipe2 + '&jns=nind">'+ CONVERT(VARCHAR(50), SUM(A.nind)) + '</a>' AS nind,
	   '<a href="' + @linkPrefix + A.tipe2 + '&jns=bat">'+ CONVERT(VARCHAR(50), SUM(A.bat)) + '</a>' AS bat,
	   '<a href="' + @linkPrefix + A.tipe2 + '&jns=inte">'+ CONVERT(VARCHAR(50), SUM(A.inte)) + '</a>' AS inte,
       COUNT(DISTINCT (A.MEMBER_TYPE_CODE + A.MEMBER_CODE)) AS jumlahPelapor
FROM
(
    SELECT *,
           CASE
               WHEN KODE_ALERT LIKE 'SB%' THEN
                   'Size Bisnis'
               WHEN KODE_ALERT LIKE 'RJP%' THEN
                   'Rata-Rata Jumlah Permintaan'
               WHEN KODE_ALERT LIKE 'RWP%' THEN
                   'Rentang Waktu Permintaan'
               WHEN KODE_ALERT LIKE 'KC%' THEN
                   'Kantor Cabang'
               WHEN KODE_ALERT LIKE 'UP%' THEN
                   'User Permintaan'
           END AS tipe,
           CASE
               WHEN KODE_ALERT LIKE 'SB%' THEN
                   'SB'
               WHEN KODE_ALERT LIKE 'RJP%' THEN
                   'RJP'
               WHEN KODE_ALERT LIKE 'RWP%' THEN
                   'RWP'
               WHEN KODE_ALERT LIKE 'KC%' THEN
                   'KC'
               WHEN KODE_ALERT LIKE 'UP%' THEN
                   'UP'
           END AS tipe2,
		   CASE	WHEN DIMENSI1='Total Permintaan' THEN 1 ELSE 0 END tp,
		   CASE	WHEN DIMENSI1='Individu' THEN 1 ELSE 0 END ind,
		   CASE	WHEN DIMENSI1='NonIndividu' THEN 1 ELSE 0 END nind,
		   CASE	WHEN DIMENSI1='Interactive' THEN 1 ELSE 0 END inte,
		   CASE	WHEN DIMENSI1='Batch' THEN 1 ELSE 0 END bat
    FROM dbo.Alert_Summary
    WHERE (
              KODE_ALERT LIKE 'RJP%'
              OR KODE_ALERT LIKE 'RWP%'
              OR KODE_ALERT LIKE 'KC%'
              OR KODE_ALERT LIKE 'UP%'
              OR KODE_ALERT LIKE 'SB%'
          )
          AND LEVEL_ALERT = @lvlAlert
          AND PERIODE = dbo.GetDateWithoutTime(@periode)
          AND TIPE_PERIODE = @tipePeriode
          AND MEMBER_TYPE_CODE + MEMBER_CODE IN
              (
                  SELECT member_type_code + member_code
                  FROM dbo.pengawas_ljk
                  WHERE user_login_id = @userID
                        AND active_flag = 'Y'
              )
) A
GROUP BY A.tipe,
         A.tipe2;



GO
