using System.Text.Json;
using Newtonsoft.Json.Serialization;

namespace Web_Api.Dependencies
{
    public class SnakeCaseJsonNamingPolicy : JsonNamingPolicy
    {
        private SnakeCaseNamingStrategy scns = new SnakeCaseNamingStrategy();

        public override string ConvertName(string name)
        {
            return scns.GetPropertyName(name, false);
        }
    }
}