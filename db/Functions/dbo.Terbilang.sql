SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE FUNCTION [dbo].[Terbilang](@number NUMERIC(19,6))
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE
    @position INT, 
    @length INT, 
    @words NVARCHAR(MAX), 
    @ends NVARCHAR(MAX), 
    @numStr NVARCHAR(MAX), 
    @foreStr NVARCHAR(MAX), 
    @backStr NVARCHAR(MAX), 
    @char NVARCHAR(1),
    @charafter NVARCHAR(1),
    @charprev NVARCHAR(1),
    @charprev2 NVARCHAR(1)
  
SELECT @numStr = STR(@number, 19, 2)
SELECT @numStr = LTRIM(RTRIM(@numStr))
SELECT @foreStr = SUBSTRING(@numStr, 0, (SELECT CHARINDEX('.', @numStr, 1)))
SELECT @backStr = SUBSTRING(@numStr, (SELECT CHARINDEX('.', @numStr, 1)+1), LEN(@numStr))
SELECT @length = LEN(@foreStr)
SELECT @position = @length
SELECT @words =''
  
    --Memproses "angka di depan koma" 
    WHILE(@position > 0)
    BEGIN   
             
            SELECT @char = SUBSTRING(@numStr, @length+1 - @position, 1)
            SELECT @charafter = SUBSTRING(@numStr, @length+2 - @position, 1)
            SELECT @charprev = SUBSTRING(@numStr, @length - @position, 1)
            SELECT @charprev2 = SUBSTRING(@numStr, @length - @position - 1, 1)
             
            IF ((@char = '1') AND ((SELECT(@position-1)/3.0) = 1) AND
                (@charafter != '' ) AND ((SELECT CAST(@charprev as INT)) = 0)) 
                SELECT @words = @words + 'se'
            ELSE
            IF ((@char = '1') AND ((SELECT @position % 3) = 1)) 
                SELECT @words = @words + 'satu '
            ELSE
            IF ((@char = '1') AND ((SELECT CAST(@charafter as INT)) > 1) AND
                ((SELECT @position % 3) = 2))
                BEGIN
                    IF (@charafter = '1') SELECT @words = @words + 'se' ELSE
                    IF (@charafter = '2') SELECT @words = @words + 'dua ' ELSE
                    IF (@charafter = '3') SELECT @words = @words + 'tiga ' ELSE   
                    IF (@charafter = '4') SELECT @words = @words + 'empat ' ELSE   
                    IF (@charafter = '5') SELECT @words = @words + 'lima ' ELSE   
                    IF (@charafter = '6') SELECT @words = @words + 'enam ' ELSE   
                    IF (@charafter = '7') SELECT @words = @words + 'tujuh ' ELSE   
                    IF (@charafter = '8') SELECT @words = @words + 'delapan ' ELSE   
                    IF (@charafter = '9') SELECT @words = @words + 'sembilan '
                END
            ELSE
            IF (@char = '1') SELECT @words = @words + 'se' ELSE
            IF (@char = '2') SELECT @words = @words + 'dua ' ELSE
            IF (@char = '3') SELECT @words = @words + 'tiga ' ELSE   
            IF (@char = '4') SELECT @words = @words + 'empat ' ELSE   
            IF (@char = '5') SELECT @words = @words + 'lima ' ELSE   
            IF (@char = '6') SELECT @words = @words + 'enam ' ELSE   
            IF (@char = '7') SELECT @words = @words + 'tujuh ' ELSE   
            IF (@char = '8') SELECT @words = @words + 'delapan ' ELSE   
            IF (@char = '9') SELECT @words = @words + 'sembilan ' ELSE
            IF ((@char = '0') AND ((SELECT CAST(@charprev as INT)) > 1) AND ((SELECT @position % 3) = 1))
                SELECT @words = @words 
            ELSE
            IF ((@char = '0') AND ((SELECT @charprev) = '0') AND ((SELECT CAST(@charprev2 as INT)) > 0) AND ((SELECT @position % 3) = 1))
                SELECT @words = @words 
            ELSE
            IF (@char = '0') 
            BEGIN
                SELECT @position = @position - 1
                CONTINUE
            END    
             
            IF ((SELECT @position % 3) = 0) SELECT @words = @words + 'ratus ' ELSE   
            IF (((SELECT @position % 3) = 2) AND ((SELECT CAST(@char as INT)) > 1)) 
                SELECT @words = @words + 'puluh '
            ELSE   
            IF (((SELECT @position % 3) = 2) AND ((SELECT CAST(@char as INT)) = 1)
                AND ((SELECT CAST(@charafter as INT)) > 0))
            BEGIN
                SELECT @words = @words + 'belas '
                SELECT @position = @position - 1
            END
            ELSE
            IF (((SELECT @position % 3) = 2) AND ((SELECT CAST(@char as INT)) = 1)
                AND ((SELECT CAST(@charafter as INT)) = 0))
            BEGIN
                SELECT @words = @words + 'puluh '
                SELECT @position = @position - 1
            END
             
            IF ((SELECT (@position-1)/3.0) = 1) SELECT @words = @words +'ribu ' ELSE
            IF ((SELECT (@position-1)/3.0) = 2) SELECT @words = @words +'juta ' ELSE
            IF ((SELECT (@position-1)/3.0) = 3) SELECT @words = @words +'milyar ' ELSE
            IF ((SELECT (@position-1)/3.0) = 4) SELECT @words = @words +'triliun '
  
        SELECT @position = @position - 1
    END
     
    --Memproses "koma" dan "angka di belakang koma"
    IF((SELECT CAST(@backStr AS INT)) > 0)
    BEGIN
        --Menambahkan "koma" pada terbilang
         
        SELECT @words = @words + 'koma '
         
        --Menambahkan "Angka di belakang koma" pada terbilang
     
        SELECT @length = LEN(@backStr)
        SELECT @position = @length
         
        WHILE( @position > 0)
        BEGIN
         
            SELECT @char = SUBSTRING(@backStr, @length+1 - @position, 1)
            SELECT @words = @words +
            (CASE @char
                WHEN '0'THEN 'nol '
                WHEN '1'THEN 'satu '
                WHEN '2'THEN 'dua '
                WHEN '3'THEN 'tiga '
                WHEN '4'THEN 'empat '
                WHEN '5'THEN 'lima '
                WHEN '6'THEN 'enam '
                WHEN '7'THEN 'tujuh '
                WHEN '8'THEN 'delapan '
                WHEN '9'THEN 'sembilan '
                ELSE ''
             END
            )    
            SELECT @position = @position - 1
        END
    END
     
    SELECT @words = LTRIM(RTRIM(@words))
     
    -- Huruf pertama huruf besar
    IF LEN(@words) > 0 
    BEGIN
        SET @words = UPPER(left(@words,1)) + RIGHT(@words, LEN(@words)-1)
    END
  
    /* FINAL RETURN */
RETURN (SELECT @words)
END
GO
