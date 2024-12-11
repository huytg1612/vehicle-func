using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleData;

namespace VehicleDynamoDB
{
    public class VehicleDB
    {
        private string _tableName = "Vehicle";
        public AmazonDynamoDBClient _client { get; set; }
        public VehicleDB()
        {
            _client = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.APSoutheast1,
                Profile = new Profile("VisualStudioProfile")
            });
        }

        public async Task<VehicleViewModel> FetchSingle(VehicleFilter filter)
        {
            VehicleViewModel result = null;

            var key = new Dictionary<string, AttributeValue>
            {
                ["Type"] = new AttributeValue { S = filter.Type },
                //["Name"] = new AttributeValue { S = filter.Name }
                ["Model"] = new AttributeValue { S = filter.Model },
            };

            var item = await _client.GetItemAsync(_tableName, key);
            if (item != null && item.Item != null && item.Item.Count > 0)
            {
                result = new VehicleViewModel
                {
                    Model = item.Item["Model"].S,
                    Type = item.Item["Type"].S,
                    Name = item.Item["Name"].S,
                };
            }

            return result;
        }

        public async Task<List<VehicleViewModel>> FetchList(VehicleFilter filter)
        {
            var result = new List<VehicleViewModel>();
            var movieTable = Table.LoadTable(_client, _tableName);

            var qFilter = new QueryFilter("Type", QueryOperator.Equal, filter.Type);
            if (!string.IsNullOrEmpty(filter.Model))
            {
                qFilter.AddCondition("Model", QueryOperator.Equal, filter.Model);
            }
            if (!string.IsNullOrEmpty(filter.Name)) 
            {
                qFilter.AddCondition("Name", QueryOperator.Equal, filter.Name);
            }
            var config = new QueryOperationConfig()
            {
                Limit = 10, // 10 items per page.
                Select = SelectValues.SpecificAttributes,
                AttributesToGet = new List<string>
                {
                  "Type",
                  "Model",
                  "Name"
                },
                ConsistentRead = true,
                Filter = qFilter,
            };

            Search search = movieTable.Query(config);
            VehicleViewModel model = null;
            do
            {
                var vehicleList = await search.GetNextSetAsync();
                foreach(var vehicle in vehicleList)
                {
                    model = new VehicleViewModel
                    {
                        Model = vehicle["Model"].AsString(),
                        Type = vehicle["Type"].AsString(),
                    };
                    if (vehicle.ContainsKey("Name"))
                        model.Name = vehicle["Name"].AsString();

                    result.Add(model);
                }
            }
            while (!search.IsDone);

            return result;
        }

        public async Task<bool> CreateItem(VehicleViewModel model)
        {
            var key = new Dictionary<string, AttributeValue>
            {
                ["Type"] = new AttributeValue { S = model.Type },
                ["Name"] = new AttributeValue { S = model.Name } ,
                ["Model"] = new AttributeValue { S = model.Model },
            };

            _client.PutItemAsync(_tableName, key).Wait();
            return true;
        }
    }
}
