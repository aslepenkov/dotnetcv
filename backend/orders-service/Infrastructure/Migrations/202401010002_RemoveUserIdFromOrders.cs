using FluentMigrator;

namespace OrdersService.Infrastructure.Migrations;

[Migration(202401010002)]
public class RemoveUserIdFromOrders : Migration
{
    public override void Up()
    {
        Delete.Column("UserId").FromTable("Orders");
    }

    public override void Down()
    {
        Alter.Table("Orders").AddColumn("UserId").AsGuid().NotNullable();
    }
}