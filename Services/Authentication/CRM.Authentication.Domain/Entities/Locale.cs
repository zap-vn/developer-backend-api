using System;

namespace CRM.Authentication.Domain.Entities
{
    public class Locale
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
    }
}
