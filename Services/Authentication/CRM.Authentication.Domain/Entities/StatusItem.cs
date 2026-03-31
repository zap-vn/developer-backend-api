using System;

namespace CRM.Authentication.Domain.Entities
{
    public class StatusItem
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? code { get; set; }
        public string? domain { get; set; }
    }
}
