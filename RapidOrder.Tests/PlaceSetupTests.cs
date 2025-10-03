using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidOrder.Core.Entities;
using Xunit;

namespace RapidOrder.Tests.Entities;

public class PlaceSetupTests
{
    [Fact]
    public void Place_Defaults_AreInitialized()
    {
        var place = new Place();

        Assert.Equal(string.Empty, place.Description);
        Assert.Null(place.Icon);
        Assert.NotNull(place.CallButtons);
        Assert.Empty(place.CallButtons);
        Assert.NotNull(place.Missions);
        Assert.Empty(place.Missions);
        Assert.Equal(string.Empty, place.CreatedBy);
        Assert.Null(place.LastModifiedBy);
    }

    [Fact]
    public void Place_Allows_Associating_Setup_And_User()
    {
        var setup = new Setup { Id = 3, Name = "Summer" };
        var user = new User
        {
            Id = 7,
            Login = "user@example.com",
            Password = new string('x', 60)
        };

        var place = new Place
        {
            Number = 12,
            SetupId = setup.Id,
            Setup = setup,
            UserId = user.Id,
            User = user
        };

        Assert.Equal(12, place.Number);
        Assert.Same(setup, place.Setup);
        Assert.Same(user, place.User);
        Assert.Equal(setup.Id, place.SetupId);
        Assert.Equal(user.Id, place.UserId);
    }

    [Fact]
    public void Place_Validation_Fails_For_TooLongDescription()
    {
        var place = new Place
        {
            Number = 1,
            Description = new string('a', 200)
        };

        var context = new ValidationContext(place);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(place, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Place.Description)));
    }

    [Fact]
    public void Setup_Validation_Fails_For_TooLongIcon()
    {
        var setup = new Setup
        {
            Icon = new string('b', 60)
        };

        var context = new ValidationContext(setup);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(setup, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Setup.Icon)));
    }
}
