using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLesegaisRu.DbWork
{
    class DbCommand
    {
        public const string DbName = "LesEgais2";
        public const string AddDeclarationProcedureName = "AddDeclaration";
        public const string DealNumberParam = "DealNumber";
        public const string SellerNameParam = "SellerName";
        public const string SellerINNParam = "SellerINN";
        public const string CustomerNameParam = "CustomerName";
        public const string CustomerINNParam = "CustomerINN";
        public const string DealDateParam = "DealDate";
        public const string VolumeByReportParam = "VolumeByReport";

        //      @DealNumber = N'0003002310126776010400625664',
        //@SellerName = N'ИП Мнацаканьян Карэн Петросович1',
        //@SellerINN = N'010400625660',
        //@Customer = N'ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ < ФАБРИКА АЛЬВЕРО',
        //@CustomerINN = N'2310126776',
        //@DealDate = '18.07.2022',
        //@VolumeByReport = N'Пр: 0 / Пк: 0'

        public const string CreateDataBaseQuery = "IF DB_ID (N'" + DbName + "') IS NULL CREATE DATABASE " + DbName + ";";

        public const string CheckIsStoredProcedureExist =
        @"DECLARE @IsExist BIT;
                SET @IsExist = 0;
                IF OBJECT_ID(N'" + AddDeclarationProcedureName + @"', N'P') IS NOT NULL
                SET @IsExist = 1;
                select @IsExist;";


        public const string CreateDeclarationTableQuery =
            @"
            IF OBJECT_ID(N'Declaration', N'U') IS NULL
                CREATE TABLE[dbo].[Declaration]
                (
                    [DealNumber][nchar](28) NOT NULL,
                    [SellerName] [nvarchar] (500) NOT NULL,
                    [SellerINN] [nchar] (13) NOT NULL,
                    [CustomerName] [nvarchar] (500) NOT NULL,
                    [CustomerINN] [nchar] (13) NULL,
                    [DealDate] [date] NOT NULL,
                    [VolumeByReport] [nvarchar](50) NOT NULL,
                    CONSTRAINT[PK_Declaration] PRIMARY KEY CLUSTERED([DealNumber]
                ))";


//--IF OBJECT_ID(N'InsertNewDeclaration', N'P') IS NOT NULL 
//--DROP PROCEDURE dbo.InsertNewDeclaration;


        public const string CreateAddDeclarationSoredProcedure =
            @"CREATE PROCEDURE[dbo]."+ AddDeclarationProcedureName + @"
            @DealNumber nchar(28),
	        @SellerName nchar(500),
	        @SellerINN nchar(13),
	        @CustomerName nvarchar(500),
	        @CustomerINN nchar(13),
            @DealDate date,
            @VolumeByReport nvarchar(50)
            AS
            BEGIN
               IF NOT EXISTS(SELECT 1 FROM Declaration WHERE DealNumber = @DealNumber)
                INSERT INTO Declaration(DealNumber, SellerName, SellerINN, CustomerName, CustomerINN, DealDate, VolumeByReport)

                VALUES(@DealNumber, @SellerName, @SellerINN, @CustomerName, @CustomerINN, @DealDate, @VolumeByReport)
            END";
    }
}
