USE [LesEgais2]
GO
/****** Object:  StoredProcedure [dbo].[AddDeclaration]    Script Date: 20.07.2022 18:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE[dbo].[AddDeclaration]
            @DealNumber nchar(28),
	        @SellerName nchar(500),
	        @SellerINN nchar(13),
	        @CustomerName nvarchar(500),
	        @CustomerINN nchar(13),
            @DealDate date,
            @VolumeByReport nvarchar(50)
            AS
            BEGIN
				SET NOCOUNT ON;
				DECLARE @resultStr VARCHAR(1000);
				SET  @resultStr =N'';
               IF NOT EXISTS(SELECT 1 FROM Declaration WHERE DealNumber = @DealNumber)
			   BEGIN
				   BEGIN TRY  
						INSERT INTO Declaration(DealNumber, SellerName, SellerINN, CustomerName, CustomerINN, DealDate, VolumeByReport)
						VALUES(@DealNumber, @SellerName, @SellerINN, @CustomerName, @CustomerINN, @DealDate, @VolumeByReport);
						SET  @resultStr =N'Запись добавлена';
				   END TRY
				   BEGIN CATCH  
					  SET @resultStr = 'не предвиденная ошибка: ' + ERROR_MESSAGE();  
				   END CATCH;
				END;
				ELSE
				BEGIN
					SET @resultStr=N'Запись уже присутсвует в таблице';
				END;
				Select @resultStr as 'resultStr';
            END



USE [LesEgais2]
GO

DECLARE	@return_value int
SET DATEFORMAT dmy;


EXEC	@return_value = [dbo].[AddDeclaration]
		@DealNumber = N'000300231012677601040054634',
		@SellerName = N'ИП Мнацаканьян Карэн Петросович1',
		@SellerINN = N'010400625660',
		@CustomerName = N'ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ < ФАБРИКА АЛЬВЕРО',
		@CustomerINN = N'2310126776',
		@DealDate = '18.07.2022',
		@VolumeByReport = N'Пр: 0 / Пк: 0'

--SELECT	'Return Value' = @return_value

SELECT COUNT(*)
  FROM [LesEgais2].[dbo].[Declaration]

DELETE [dbo].[Declaration];



GO
