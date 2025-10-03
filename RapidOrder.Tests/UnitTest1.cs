using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Enums;

namespace RapidOrder.Tests.Entities
{
    public class UserTests
    {
        [Fact]
        public void CanCreateUser_WithValidProperties()
        {
            var user = new User
            {
                Id = 1,
                Login = "andreas@example.com",
                UserName = "andreas",
                FirstName = "Andreas",
                LastName = "Ramos",
                Email = "andreas@example.com",
                Activated = true,
                Role = UserRole.ADMIN,
                Missions = new List<Mission> { new(), new() },
                CreatedBy = "tester",
                LastModifiedBy = "tester"
            };

            Assert.Equal("Andreas Ramos", user.DisplayName);
            Assert.True(user.Activated);
            Assert.Equal(UserRole.ADMIN, user.Role);
            Assert.Equal(2, user.Missions.Count);
            Assert.Equal("tester", user.CreatedBy);
            Assert.Equal("tester", user.LastModifiedBy);
        }

        [Fact]
        public void Authority_EqualsAndHashCode_WorkCorrectly()
        {
            var auth1 = new Authority { Name = "ADMIN" };
            var auth2 = new Authority { Name = "ADMIN" };
            var auth3 = new Authority { Name = "USER" };

            Assert.True(auth1.Equals(auth2));
            Assert.False(auth1.Equals(auth3));
            Assert.Equal(auth1.GetHashCode(), auth2.GetHashCode());
            Assert.NotEqual(auth1.GetHashCode(), auth3.GetHashCode());
        }

        [Fact]
        public void User_ValidationAttributes_WorkCorrectly()
        {
            var user = new User
            {
                Login = string.Empty,
                UserName = "",
                Email = "invalid-email"
            };

            var context = new ValidationContext(user);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(user, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(User.Login)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(User.Email)));
        }

        [Fact]
        public void User_DisplayName_ReturnsFallbackIfEmpty()
        {
            var user = new User
            {
                Login = "andreas",
                UserName = "andreas"
            };

            Assert.Equal("andreas", user.DisplayName);
        }
    }
}
