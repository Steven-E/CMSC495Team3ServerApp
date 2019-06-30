using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace CMSC495Team3ServerApp.Models
{
    public class RestDoc
    {
        public string Method { get; }

        public string Route { get; }

        public string InputType { get; }

        public string OutputType { get; }

        public JRaw JsonSchema { get; }

        public RestDoc(string method, string route, string inputType, string outputType, Type objectType )
        {
            Method = method;
            Route = route;
            InputType = inputType;
            OutputType = outputType;

            var generator = new JsonSchemaGenerator();
            var schema = generator.Generate(objectType);
            JsonSchema = new JRaw(schema.ToString());
                
            //schema.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty);
        }
    }
}