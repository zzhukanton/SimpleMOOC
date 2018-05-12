namespace GeoCourse.Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCustomFieldsToIntitialDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        AnswerId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        IsCorrect = c.Boolean(nullable: false),
                        QuestionId = c.Int(),
                    })
                .PrimaryKey(t => t.AnswerId)
                .ForeignKey("dbo.Questions", t => t.QuestionId)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        QuestionId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Points = c.Int(nullable: false),
                        TestId = c.Int(),
                    })
                .PrimaryKey(t => t.QuestionId)
                .ForeignKey("dbo.Tests", t => t.TestId)
                .Index(t => t.TestId);
            
            CreateTable(
                "dbo.Tests",
                c => new
                    {
                        TestId = c.Int(nullable: false, identity: true),
                        Chapter = c.String(),
                        DocumentPath = c.String(),
                        Description = c.String(),
                        CourseId = c.Int(),
                    })
                .PrimaryKey(t => t.TestId)
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .Index(t => t.CourseId);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        CourseId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        PicturePath = c.String(),
                        Description = c.String(),
                        Duration = c.String(),
                        MaxPoints = c.Int(nullable: false),
                        RequiredPoints = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CourseId);
            
            CreateTable(
                "dbo.UserCourses",
                c => new
                    {
                        UserCourseId = c.Int(nullable: false, identity: true),
                        UserId = c.Guid(),
                        CourseId = c.Int(),
                        IsFinished = c.Boolean(nullable: false),
                        CurrentPoints = c.Int(nullable: false),
                        DateCompleted = c.DateTime(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserCourseId)
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.CourseId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.TestResults",
                c => new
                    {
                        TestResultId = c.Int(nullable: false, identity: true),
                        TestId = c.Int(),
                        UserCourseId = c.Int(),
                        PointCount = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TestResultId)
                .ForeignKey("dbo.Tests", t => t.TestId)
                .ForeignKey("dbo.UserCourses", t => t.UserCourseId)
                .Index(t => t.TestId)
                .Index(t => t.UserCourseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Questions", "TestId", "dbo.Tests");
            DropForeignKey("dbo.UserCourses", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TestResults", "UserCourseId", "dbo.UserCourses");
            DropForeignKey("dbo.TestResults", "TestId", "dbo.Tests");
            DropForeignKey("dbo.UserCourses", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Tests", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Answers", "QuestionId", "dbo.Questions");
            DropIndex("dbo.TestResults", new[] { "UserCourseId" });
            DropIndex("dbo.TestResults", new[] { "TestId" });
            DropIndex("dbo.UserCourses", new[] { "User_Id" });
            DropIndex("dbo.UserCourses", new[] { "CourseId" });
            DropIndex("dbo.Tests", new[] { "CourseId" });
            DropIndex("dbo.Questions", new[] { "TestId" });
            DropIndex("dbo.Answers", new[] { "QuestionId" });
            DropTable("dbo.TestResults");
            DropTable("dbo.UserCourses");
            DropTable("dbo.Courses");
            DropTable("dbo.Tests");
            DropTable("dbo.Questions");
            DropTable("dbo.Answers");
        }
    }
}
