namespace WeatherInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class City : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cities", "Name_cityEn", c => c.String());
            AddColumn("dbo.Cities", "Name_cityRus", c => c.String());
            DropColumn("dbo.Cities", "Name_city");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cities", "Name_city", c => c.String());
            DropColumn("dbo.Cities", "Name_cityRus");
            DropColumn("dbo.Cities", "Name_cityEn");
        }
    }
}
