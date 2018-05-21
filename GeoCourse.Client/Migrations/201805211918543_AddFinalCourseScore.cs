namespace GeoCourse.Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFinalCourseScore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserCourses", "FinalCourseScore", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserCourses", "FinalCourseScore");
        }
    }
}
