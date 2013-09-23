namespace MvcBootstrap.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_v2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Student", "FirstName", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Student", "FirstName", c => c.String(nullable: false));
        }
    }
}
