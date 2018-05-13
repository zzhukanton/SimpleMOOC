namespace GeoCourse.Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTestTryCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestResults", "CurrentTryCount", c => c.Int(nullable: false));
            AddColumn("dbo.TestResults", "MaxTryCount", c => c.Int(nullable: false));
            DropColumn("dbo.Questions", "Points");
            DropColumn("dbo.TestResults", "IsLocked");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TestResults", "IsLocked", c => c.Boolean(nullable: false));
            AddColumn("dbo.Questions", "Points", c => c.Int(nullable: false));
            DropColumn("dbo.TestResults", "MaxTryCount");
            DropColumn("dbo.TestResults", "CurrentTryCount");
        }
    }
}
