using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using RapidOrder.Core.Enums;

namespace RapidOrder.Core.Entities
{
    public class User : AbstractAuditingEntity
    {
        private const string LoginPattern = "^(?>[a-zA-Z0-9!$&*+=?^_`{|}~.-]+@[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)*)|(?>[_.@A-Za-z0-9-]+)$";

        private string _login = string.Empty;

        public long Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [RegularExpression(LoginPattern)]
        public string Login
        {
            get => _login;
            set => _login = value?.ToLowerInvariant() ?? string.Empty;
        }

        [Required]
        [StringLength(60, MinimumLength = 60)]
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(100)]
        public string? UserName { get; set; }

        [EmailAddress]
        [StringLength(254, MinimumLength = 5)]
        public string? Email { get; set; }

        [Required]
        public bool Activated { get; set; }

        [StringLength(10, MinimumLength = 2)]
        public string? LangKey { get; set; }

        [StringLength(256)]
        public string? ImageUrl { get; set; }

        [JsonIgnore]
        [StringLength(20)]
        public string? ActivationKey { get; set; }

        [JsonIgnore]
        [StringLength(20)]
        public string? ResetKey { get; set; }

        public DateTime? ResetDate { get; set; }

        [StringLength(100)]
        public string? Schedule { get; set; }

        [JsonIgnore]
        public ICollection<Authority> Authorities { get; set; } = new HashSet<Authority>();

        private string? _displayName = string.Empty;

        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_displayName))
                {
                    return _displayName!;
                }

                var parts = new[] { FirstName, LastName }
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Select(name => name!.Trim())
                    .ToArray();

                if (parts.Length > 0)
                {
                    return string.Join(" ", parts);
                }

                if (!string.IsNullOrWhiteSpace(UserName))
                {
                    return UserName!;
                }

                return Login;
            }
            set => _displayName = value;
        }

        public UserRole Role { get; set; } = UserRole.WAITER;

        public ICollection<Mission> Missions { get; set; } = new List<Mission>();

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is User other && Id != 0 && Id == other.Id;
        }

        public override int GetHashCode()
            => Id.GetHashCode();

        public override string ToString()
            => $"User{{login='{Login}', firstName='{FirstName}', lastName='{LastName}', email='{Email}', imageUrl='{ImageUrl}', activated='{Activated}', langKey='{LangKey}', activationKey='{ActivationKey}'}}";
    }
}
