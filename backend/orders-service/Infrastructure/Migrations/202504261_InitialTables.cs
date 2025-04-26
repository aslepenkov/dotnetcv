using FluentMigrator;

namespace app.Infrastructure.Migrations;

[Migration(202504261)]
public class InitialTables : Migration
{
    public override void Up()
    {
        Create.Table("Orders")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("UserId").AsGuid().NotNullable()
            .WithColumn("Total").AsDecimal().NotNullable()
            .WithColumn("Status").AsString(50).NotNullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
    }

    public override void Down()
    {
        Delete.Table("Orders");
    }
}