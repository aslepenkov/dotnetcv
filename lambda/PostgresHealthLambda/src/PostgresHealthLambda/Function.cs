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

        try
        {
            var connectionString = await GetConnectionStringFromSSM(context);
            return await CheckDatabaseConnection(connectionString, context);
        }
        catch (Exception ex)
        {
            context.Logger.Log($"Error: {ex.Message}");
            return CreateErrorResponse("An unexpected error occurred.", ex.Message);
        }
    }

    private async Task<string> GetConnectionStringFromSSM(ILambdaContext context)
    {
        try
        {
            context.Logger.Log("Fetching SSM parameter...");
            var request = new GetParameterRequest
            {
                Name = "/config/postgres/connection-string",
                WithDecryption = true
            };
            var response = await _ssmClient.GetParameterAsync(request);
            context.Logger.Log("Successfully retrieved SSM parameter.");
            return response.Parameter.Value;
        }
        catch (Exception ex)
        {
            context.Logger.Log($"SSM error: {ex.Message}");
            throw new Exception("Failed to get SSM parameter", ex);
        }
    }

    private async Task<Stream> CheckDatabaseConnection(string connectionString, ILambdaContext context)
    {
        try
        {
            context.Logger.Log("Connecting to PostgreSQL...");
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            context.Logger.Log("Database connection successful.");
            return CreateSuccessResponse("Database connected successfully.");
        }
        catch (Exception ex)
        {
            context.Logger.Log($"Database connection error: {ex.Message}");
            return CreateErrorResponse("Database connection failed.", ex.Message);
        }
    }

    private Stream CreateSuccessResponse(string message)
    {
        return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { status = "ok", message })));
    }

    private Stream CreateErrorResponse(string error, string details)
    {
        return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { status = "error", error, details })));
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
