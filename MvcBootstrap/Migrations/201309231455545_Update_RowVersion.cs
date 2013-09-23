namespace MvcBootstrap.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_RowVersion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Department", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Department", "RowVersion");
        }
    }
}
