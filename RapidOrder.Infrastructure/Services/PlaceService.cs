using Microsoft.EntityFrameworkCore;
using System.Linq;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Services;

namespace RapidOrder.Infrastructure.Services;

public class PlaceService : IPlaceService
{
    private readonly RapidOrderDbContext _dbContext;

    public PlaceService(RapidOrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Place> CreateAsync(Place place, CancellationToken cancellationToken = default)
    {
        _dbContext.Places.Add(place);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return place;
    }

    public async Task<Place?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Places
            .Include(p => p.PlaceGroup)
            .Include(p => p.Setup)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Place>> GetByUserAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Places
            .Where(p => p.UserId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Place>> GetBySetupAsync(int setupId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Places
            .Where(p => p.SetupId == setupId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Place place, CancellationToken cancellationToken = default)
    {
        _dbContext.Places.Update(place);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var place = await _dbContext.Places.FindAsync(new object?[] { id }, cancellationToken);
        if (place == null)
        {
            return;
        }

        _dbContext.Places.Remove(place);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
