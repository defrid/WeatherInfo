namespace WeatherInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        Name_cityEn = c.String(),
                        Name_cityRus = c.String(),
                    })
                .PrimaryKey(t => t.CityId);
            
            CreateTable(
                "dbo.Places",
                c => new
                    {
                        PlaceId = c.Int(nullable: false, identity: true),
                        CountyId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        Country_CountryId = c.Int(),
                    })
                .PrimaryKey(t => t.PlaceId)
                .ForeignKey("dbo.Cities", t => t.CityId, cascadeDelete: true)
                .ForeignKey("dbo.Countries", t => t.Country_CountryId)
                .Index(t => t.CityId)
                .Index(t => t.Country_CountryId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryId = c.Int(nullable: false, identity: true),
                        Name_bukva = c.String(),
                        Name_country = c.String(),
                    })
                .PrimaryKey(t => t.CountryId);
            
            CreateTable(
                "dbo.Prognozs",
                c => new
                    {
                        PrognozId = c.Int(nullable: false, identity: true),
                        PlaceId = c.Int(nullable: false),
                        TempId = c.Int(nullable: false),
                        PictureId = c.Int(nullable: false),
                        Data_DataId = c.Int(),
                    })
                .PrimaryKey(t => t.PrognozId)
                .ForeignKey("dbo.Pictures", t => t.PictureId, cascadeDelete: true)
                .ForeignKey("dbo.Places", t => t.PlaceId, cascadeDelete: true)
                .ForeignKey("dbo.Data", t => t.Data_DataId)
                .ForeignKey("dbo.Temps", t => t.TempId, cascadeDelete: true)
                .Index(t => t.PlaceId)
                .Index(t => t.TempId)
                .Index(t => t.PictureId)
                .Index(t => t.Data_DataId);
            
            CreateTable(
                "dbo.Pictures",
                c => new
                    {
                        PictureId = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                    })
                .PrimaryKey(t => t.PictureId);
            
            CreateTable(
                "dbo.Temps",
                c => new
                    {
                        TempId = c.Int(nullable: false, identity: true),
                        Value = c.Int(nullable: false),
                        Pazmer = c.String(),
                        DataId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TempId)
                .ForeignKey("dbo.Data", t => t.DataId, cascadeDelete: true)
                .Index(t => t.DataId);
            
            CreateTable(
                "dbo.Data",
                c => new
                    {
                        DataId = c.Int(nullable: false, identity: true),
                        Year = c.Int(nullable: false),
                        Month = c.String(),
                        Day = c.String(),
                        SutkiId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DataId)
                .ForeignKey("dbo.Sutkis", t => t.SutkiId, cascadeDelete: true)
                .Index(t => t.SutkiId);
            
            CreateTable(
                "dbo.Sutkis",
                c => new
                    {
                        SutkiId = c.Int(nullable: false, identity: true),
                        Name_sutki = c.String(),
                    })
                .PrimaryKey(t => t.SutkiId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Prognozs", "TempId", "dbo.Temps");
            DropForeignKey("dbo.Temps", "DataId", "dbo.Data");
            DropForeignKey("dbo.Data", "SutkiId", "dbo.Sutkis");
            DropForeignKey("dbo.Prognozs", "Data_DataId", "dbo.Data");
            DropForeignKey("dbo.Prognozs", "PlaceId", "dbo.Places");
            DropForeignKey("dbo.Prognozs", "PictureId", "dbo.Pictures");
            DropForeignKey("dbo.Places", "Country_CountryId", "dbo.Countries");
            DropForeignKey("dbo.Places", "CityId", "dbo.Cities");
            DropIndex("dbo.Data", new[] { "SutkiId" });
            DropIndex("dbo.Temps", new[] { "DataId" });
            DropIndex("dbo.Prognozs", new[] { "Data_DataId" });
            DropIndex("dbo.Prognozs", new[] { "PictureId" });
            DropIndex("dbo.Prognozs", new[] { "TempId" });
            DropIndex("dbo.Prognozs", new[] { "PlaceId" });
            DropIndex("dbo.Places", new[] { "Country_CountryId" });
            DropIndex("dbo.Places", new[] { "CityId" });
            DropTable("dbo.Sutkis");
            DropTable("dbo.Data");
            DropTable("dbo.Temps");
            DropTable("dbo.Pictures");
            DropTable("dbo.Prognozs");
            DropTable("dbo.Countries");
            DropTable("dbo.Places");
            DropTable("dbo.Cities");
        }
    }
}
