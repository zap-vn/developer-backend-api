using System.Text.Json;

namespace CRM.BuildingBlocks.Serialization
{
    public class ExceptionPascalCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            // Preserve _id as is
            if (name == "_id")
                return name;

            // Capitalize the first letter
            return char.ToUpperInvariant(name[0]) + name.Substring(1);
        }
    }
}
