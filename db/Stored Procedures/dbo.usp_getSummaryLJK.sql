SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO





CREATE PROCEDURE [dbo].[usp_getSummaryLJK]
    @JenisLJK NVARCHAR(MAX),
    @KodeLJK NVARCHAR(MAX),
    @Period NVARCHAR(MAX)
AS
BEGIN

    DECLARE @DynamicSQL NVARCHAR(MAX);
    SET @DynamicSQL = '
    SELECT 
        CASE 
            WHEN TableName = ''debitur_anomali_format_ktp'' THEN ''DQ 01''
			WHEN TableName = ''debitur_anomali_gender'' THEN ''DQ 02''
			WHEN TableName = ''debitur_anomali_duplikasi_nama_debitur'' THEN ''DQ 03''
			WHEN TableName = ''agunan_id_anomali'' THEN ''DQ 04''
			WHEN TableName = ''agunan_dokumen_anomali'' THEN ''DQ 05''
			WHEN TableName = ''da_anomali_nilai_agunan_deb'' THEN ''DQ 06''
			WHEN TableName = ''da_anomali_nik_deb'' THEN ''DQ 07''
			WHEN TableName = ''da_anomali_gelar_nama_deb'' THEN ''DQ 08''
			WHEN TableName = ''da_anomali_alamat_deb'' THEN ''DQ 09''
			WHEN TableName = ''da_analisis_debtor_nom_identitas_sama'' THEN ''DQ 10''
			WHEN TableName = ''da_analisis_debtor_nom_identitas_beda'' THEN ''DQ 11''
			WHEN TableName = ''da_anomali_nama_ibu_kandung'' THEN ''DQ 12''
			WHEN TableName = ''da_anomali_bentuk_badan_usaha'' THEN ''DQ 13''
			WHEN TableName = ''da_anomali_nilai_njop_agunan'' THEN ''DQ 14''
			WHEN TableName = ''da_anomali_penghasilan_per_tahun'' THEN ''DQ 15''
			WHEN TableName = ''da_anomali_nik_lahir_debitur'' THEN ''DQ 16''
			WHEN TableName = ''da_anomali_format_npwp'' THEN ''DQ 17''
			WHEN TableName = ''da_anomali_tempat_lahir_debitur'' THEN ''DQ 18''
			WHEN TableName = ''da_anomali_baki_debet_tidak_wajar'' THEN ''DQ 19''
			WHEN TableName = ''da_anomali_format_telepon_debitur'' THEN ''DQ 20''
			WHEN TableName = ''da_anomali_alamat_email_debitur'' THEN ''DQ 21''
			WHEN TableName = ''da_anomali_tempat_bekerja_debitur'' THEN ''DQ 22''
			WHEN TableName = ''da_anomali_alamat_bekerja_debitur'' THEN ''DQ 23''
			WHEN TableName = ''da_anomali_tempat_badan_usaha'' THEN ''DQ 24''
			WHEN TableName = ''da_anomali_nomor_akta_badan_usaha'' THEN ''DQ 25''
			WHEN TableName = ''da_anomali_format_peringkat_agunan'' THEN ''DQ 26''
			WHEN TableName = ''da_anomali_tingkat_suku_bunga'' THEN ''DQ 27''
			WHEN TableName = ''da_anomali_bukti_kepemilikan_agunan'' THEN ''DQ 28''
            ELSE TableName
        END AS TableName,
        JenisLJK, 
        KodeLJK + '' - '' + c.nama_ljk AS NamaLJK,
		KodeLJK,
        Period, 
        Rows 
    FROM dbo.BDA2_Table_Period_LJK a
	LEFT JOIN dbo.master_ljk_type b ON a.JenisLJK = b.deskripsi_jenis_ljk
	LEFT JOIN dbo.master_ljk c ON a.KodeLJK = c.kode_ljk AND b.kode_jenis_ljk = c.kode_jenis_ljk
    WHERE (a.TableName LIKE ''da%'' OR a.TableName LIKE ''debitur%'' OR a.TableName LIKE ''agunan%'')
    ';

    IF @JenisLJK IS NOT NULL
    BEGIN
        SET @DynamicSQL = @DynamicSQL + ' AND a.JenisLJK IN (' + @JenisLJK + ')'
    END

    IF @KodeLJK IS NOT NULL
    BEGIN
        SET @DynamicSQL = @DynamicSQL + ' AND a.KodeLJK IN (' + @KodeLJK + ')'
    END

    IF @Period IS NOT NULL
    BEGIN
        SET @DynamicSQL = @DynamicSQL + ' AND a.Period IN (' + @Period + ')'
    END

    SET @DynamicSQL = @DynamicSQL + ' 
    ORDER BY a.TableName ASC '
    
    -- PRINT @DynamicSQL
    EXEC sp_executesql @DynamicSQL;
END;
GO
