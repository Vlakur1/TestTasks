using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLesegaisRu.DbWork
{
    class DbCommand
    {
        public const string DbName = "LesEgais4";
        public const string AddDeclarationProcedureName = "AddDeclaration";
        public const string DealNumber = "DealNumber";
        public const string SellerName = "SellerName";
        public const string SellerINN = "SellerINN";
        public const string BuyerName = "BuyerName";
        public const string BuyerINN = "BuyerINN";
        public const string DealDate = "DealDate";
        public const string SellerVolume = "SellerVolume";
        public const string BuyerVolume = "BuyerVolume";


        //public const string VolumeByReportParam = "VolumeByReport";

        //      @DealNumber = N'0003002310126776010400625664',
        //@SellerName = N'ИП Мнацаканьян Карэн Петросович1',
        //@SellerINN = N'010400625660',
        //@Buyer = N'ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ < ФАБРИКА АЛЬВЕРО',
        //@BuyerINN = N'2310126776',
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
                    [" + DealNumber + @"][nchar](28) NOT NULL,
                    [" + DealDate + @"] [date] NOT NULL,
                    [" + SellerName + @"] [nvarchar] (500) NOT NULL,
                    [" + SellerINN + @"] [nchar] (13) NOT NULL,
                    [" + BuyerName + @"] [nvarchar] (500) NOT NULL,
                    [" + BuyerINN + @"] [nchar] (13) NULL,
                    [" + SellerVolume + @"] [nvarchar](50) NOT NULL,
                    [" + BuyerVolume + @"] [nvarchar](50) NOT NULL,                    
                    CONSTRAINT[PK_Declaration] PRIMARY KEY CLUSTERED([DealNumber]
                ))";


        //--IF OBJECT_ID(N'InsertNewDeclaration', N'P') IS NOT NULL 
        //--DROP PROCEDURE dbo.InsertNewDeclaration;


        public const string CreateAddDeclarationSoredProcedure =
            @"CREATE PROCEDURE[dbo]." + AddDeclarationProcedureName +
            " @" + DealNumber + @" nchar(28),
            @" + DealDate + @" date,
	        @" + SellerName+ @" nchar(500),
	        @" + SellerINN + @" nchar(13),
            @" + BuyerName + @" nvarchar(500),
	        @" + BuyerINN + @" nchar(13),
            @" + SellerVolume + @" nvarchar(50),
            @" + BuyerVolume + @" nvarchar(50)
            AS
            BEGIN
               IF NOT EXISTS(SELECT 1 FROM Declaration WHERE DealNumber = @DealNumber)
                INSERT INTO Declaration(DealNumber, DealDate, SellerName, SellerINN,  BuyerName, BuyerINN, SellerVolume, BuyerVolume)
                VALUES(@DealNumber, @DealDate, @SellerName, @SellerINN, @BuyerName, @BuyerINN, @SellerVolume, @BuyerVolume)
            END";
    }
}
