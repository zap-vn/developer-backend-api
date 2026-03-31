using System;

namespace CRM.Authentication.Domain.Entities
{
    public class SystemConfig
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public string key { get; set; } = string.Empty;
        public string value { get; set; } = string.Empty;
    }
}
