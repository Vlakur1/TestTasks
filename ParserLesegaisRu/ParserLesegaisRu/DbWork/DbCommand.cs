namespace ParserLesegaisRu.DbWork
{
    class DbCommand
    {
        public const string DbName = "LesEgais1";
        public const string AddDeclarationProcedureName = "AddDeclaration";
        public const string DealNumber = "DealNumber";
        public const string SellerName = "SellerName";
        public const string SellerINN = "SellerINN";
        public const string BuyerName = "BuyerName";
        public const string BuyerINN = "BuyerINN";
        public const string DealDate = "DealDate";
        public const string SellerVolume = "SellerVolume";
        public const string BuyerVolume = "BuyerVolume";

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
                    [" + DealDate + @"] [date] NULL,
                    [" + SellerName + @"] [nvarchar] (500) NOT NULL,
                    [" + SellerINN + @"] [nchar] (13) NOT NULL,
                    [" + BuyerName + @"] [nvarchar] (500) NOT NULL,
                    [" + BuyerINN + @"] [nchar] (13) NOT NULL,
                    [" + SellerVolume + @"] [decimal](18, 4) NULL,
                    [" + BuyerVolume + @"] [decimal](18, 4) NULL,                    
                    CONSTRAINT[PK_Declaration] PRIMARY KEY CLUSTERED([DealNumber]
                ))";


        public const string CreateAddDeclarationSoredProcedure =
            @"CREATE PROCEDURE[dbo]." + AddDeclarationProcedureName +
            " @" + DealNumber + @" nchar(28),
            @" + DealDate + @" date,
	        @" + SellerName + @" nchar(500),
	        @" + SellerINN + @" nchar(13),
            @" + BuyerName + @" nvarchar(500),
	        @" + BuyerINN + @" nchar(13),
            @" + SellerVolume + @" decimal (18, 4),
            @" + BuyerVolume + @" decimal (18, 4)
            AS
            BEGIN
               IF NOT EXISTS(SELECT 1 FROM Declaration WHERE DealNumber = @DealNumber)
                INSERT INTO Declaration(DealNumber, DealDate, SellerName, SellerINN,  BuyerName, BuyerINN, SellerVolume, BuyerVolume)
                VALUES(@DealNumber, @DealDate, @SellerName, @SellerINN, @BuyerName, @BuyerINN, @SellerVolume, @BuyerVolume)
            END";
    }
}
