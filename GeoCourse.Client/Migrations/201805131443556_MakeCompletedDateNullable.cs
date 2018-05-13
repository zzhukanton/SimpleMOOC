namespace GeoCourse.Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeCompletedDateNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserCourses", "DateCompleted", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserCourses", "DateCompleted", c => c.DateTime(nullable: false));
        }
    }
}
