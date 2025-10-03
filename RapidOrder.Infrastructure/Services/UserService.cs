using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Services;

namespace RapidOrder.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly RapidOrderDbContext _dbContext;

    public UserService(RapidOrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.Missions)
            .Include(u => u.Authorities)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync(new object?[] { id }, cancellationToken);
        if (user == null)
        {
            return;
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignMissionAsync(long userId, Mission mission, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .Include(u => u.Missions)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new KeyNotFoundException($"User {userId} not found");
        }

        mission.AssignedUserId = userId;
        mission.AssignedUser = user;

        user.Missions.Add(mission);
        _dbContext.Missions.Add(mission);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
