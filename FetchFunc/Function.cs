using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Reflection;
using System.Text.Json;
using VehicleData;
using VehicleDynamoDB;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FetchFunc;

public class Function
{
    VehicleDB _db;

    public Function()
    {
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    /// 
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        var filter = new VehicleFilter
        {
            Name = string.Empty,
            Type = string.Empty,
            Model = string.Empty,
        };

        if (string.IsNullOrEmpty(request.Body))
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = "Body is empty"
            };
        }

        try
        {
            filter = JsonSerializer.Deserialize<VehicleFilter>(request.Body);
        }
        catch (JsonException ex)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = "Body is wrong format\n" + ex.Message
            };
        }

        VehicleDB db = new VehicleDB();
        var result = await db?.FetchList(filter);

        return new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = System.Text.Json.JsonSerializer.Serialize(result),
        };
    }    
}
