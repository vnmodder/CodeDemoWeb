using Microsoft.AspNetCore.Identity;

namespace DemoWeb.Infrastructure.Databases.DemoWebDB.Entities
{
    public class User : IdentityUser<int>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public int? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? DeleteUserId { get; set; } = null;
        public DateTime? DeleteDate { get; set; } = null;
    }
}
