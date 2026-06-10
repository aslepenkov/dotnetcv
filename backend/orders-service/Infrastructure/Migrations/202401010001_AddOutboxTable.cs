using FluentMigrator;

namespace OrdersService.Infrastructure.Migrations;

[Migration(202401010001)]
public class AddOutboxTable : Migration
{
    public override void Up()
    {
        Create.Table("Outbox")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Type").AsString().NotNullable()
            .WithColumn("Data").AsString().NotNullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable()
            .WithColumn("ProcessedAt").AsDateTime().Nullable();
    }

    public override void Down()
    {
        Delete.Table("Outbox");
    }
}