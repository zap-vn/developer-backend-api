using System.Text.Json;

namespace CRM.BuildingBlocks.Serialization
{
    public class ExceptionPascalCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            // Force Id or _id to be _id in JSON
            if (name == "Id" || name == "_id")
                return "_id";

            // Capitalize the first letter
            return char.ToUpperInvariant(name[0]) + name.Substring(1);
        }
    }
}
