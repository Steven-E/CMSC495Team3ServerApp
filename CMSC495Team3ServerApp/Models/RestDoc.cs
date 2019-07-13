using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace CMSC495Team3ServerApp.Models
{
    public class RestDoc
    {
        public RestDoc(string method, string route, string inputType, string outputType, Type objectType)
        {
            Method = method;
            Route = route;
            InputType = inputType;
            OutputType = outputType;

            var generator = new JsonSchemaGenerator();

            if (objectType != null)
                JsonSchema = new JRaw(generator.Generate(objectType).ToString());
        }

        public string Method { get; }

        public string Route { get; }

        public string InputType { get; }

        public string OutputType { get; }

        public JRaw JsonSchema { get; }
    }
}