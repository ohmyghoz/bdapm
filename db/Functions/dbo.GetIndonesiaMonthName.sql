SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE FUNCTION [dbo].[GetIndonesiaMonthName]
(
	@int INTEGER
)
RETURNS VARCHAR(50)
AS
BEGIN
	DECLARE @result VARCHAR(50)

	IF @int=1
	BEGIN	
	SET @result='Januari'
	END 
	ELSE IF @int=2
	BEGIN	
	SET @result='Februari'
	END 
	ELSE IF @int=3
	BEGIN	
	SET @result='Maret'
	END 
	ELSE IF @int=4
	BEGIN	
	SET @result='April'
	END 
	ELSE IF @int=5
	BEGIN	
	SET @result='Mei'
	END 
	ELSE IF @int=6
	BEGIN	
	SET @result='Juni'
	END 
	ELSE IF @int=7
	BEGIN	
	SET @result='Juli'
	END 
	ELSE IF @int=8
	BEGIN	
	SET @result='Agustus'
	END 
	ELSE IF @int=9
	BEGIN	
	SET @result='September'
	END 
	ELSE IF @int=10
	BEGIN	
	SET @result='Oktober'
	END 
	ELSE IF @int=11
	BEGIN	
	SET @result='November'
	END 
	ELSE IF @int=12
	BEGIN	
	SET @result='Desember'
	END 


	RETURN @result

END
GO
