using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Text.Json;
using System.Text.Json.Serialization;
using VehicleData;
using VehicleDynamoDB;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CreateFunc;

public class Function
{
    VehicleDB _db;
    public Function()
    {
        _db = new VehicleDB();
    }


    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        if (string.IsNullOrEmpty(request.Body))
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 400,
                Body = "Body is empty"
            };
        }
        var model = JsonSerializer.Deserialize<VehicleViewModel>(request.Body);
        
        _db.CreateItem(model).Wait();

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = request.Body,
        };
    }
}
