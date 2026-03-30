using System.Text.Json;

namespace CRM.BuildingBlocks.Serialization
{
    public class CrmSnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            // Handle Id or _id to be _id for MongoDB/Frontend consistency
            if (name == "Id" || name == "_id")
                return "_id";

            // Use the built-in SnakeCaseLower for all other fields (e.g., FirstName -> first_name)
            return JsonNamingPolicy.SnakeCaseLower.ConvertName(name);
        }
    }
}
