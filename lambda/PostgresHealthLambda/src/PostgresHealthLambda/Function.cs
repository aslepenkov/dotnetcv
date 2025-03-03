using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Npgsql;
using Amazon.Runtime;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PostgresHealthLambda;

public class Function
{
    private readonly AmazonSimpleSystemsManagementClient _ssmClient;

    public Function()
    {
        _ssmClient = new AmazonSimpleSystemsManagementClient(new BasicAWSCredentials("test", "test"), 
                                                             new AmazonSimpleSystemsManagementConfig { ServiceURL = "http://localhost:4566" });
    }

    public async Task<Stream> FunctionHandler(Stream input, ILambdaContext context)
    {
        context.Logger.Log("Lambda execution started.");

        string connectionString;
        try
        {
            context.Logger.Log("Fetching SSM parameter...");
            var request = new GetParameterRequest
            {
                Name = "/config/postgres/connection-string",
                WithDecryption = true
            };
            var response = await _ssmClient.GetParameterAsync(request);
            connectionString = response.Parameter.Value;
            context.Logger.Log("Successfully retrieved SSM parameter.");

            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { 
                status = "Successfully retrieved SSM parameter.", 
                db = "connected" 
            })));

        }
        catch (Exception ex)
        {
            context.Logger.Log($"SSM error: {ex.Message}");
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { 
                status = "error", 
                error = "Failed to get SSM parameter", 
                details = ex.Message 
            })));
        }

        try
        {
            context.Logger.Log("Connecting to PostgreSQL...");
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            context.Logger.Log("Database connection successful.");
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { 
                status = "ok", 
                db = "connected" 
            })));
        }
        catch (Exception ex)
        {
            context.Logger.Log($"Database connection error: {ex.Message}");
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { 
                status = "error", 
                db = "unreachable", 
                error = ex.Message 
            })));
        }
    }

    public async Task<Stream> FunctionHandler0(Stream input, ILambdaContext context)
    {
        string connectionString;
        try
        {
            // Get connection string from SSM
            var request = new GetParameterRequest
            {
                Name = "/config/postgres/connection-string",
                WithDecryption = true
            };
            var response = await _ssmClient.GetParameterAsync(request);
            connectionString = response.Parameter.Value;
        }
        catch (Exception ex)
        {
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { status = "error", error = "Failed to get SSM parameter", details = ex.Message })));
        }

        try
        {
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { status = "ok", db = "connected" })));
        }
        catch (Exception ex)
        {
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { status = "error", db = "unreachable", error = ex.Message })));
        }
    }
}
