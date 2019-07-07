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
            //JsonSchema schema;
            //if (objectType != null)
            //    schema = generator.Generate(objectType);

            if(objectType != null)
                JsonSchema = new JRaw(generator.Generate(objectType).ToString());
                
            //schema.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty);
        }
    }
}