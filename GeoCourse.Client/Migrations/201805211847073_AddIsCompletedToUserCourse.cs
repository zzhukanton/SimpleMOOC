namespace GeoCourse.Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsCompletedToUserCourse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserCourses", "IsCompleted", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserCourses", "IsCompleted");
        }
    }
}
