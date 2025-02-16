using System.Linq.Expressions;
using FrameUp.OrderService.Infra.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FrameUp.OrderService.Infra.Abstractions;

public class BaseRepository<TModel, TDbContext> : IBaseRepository<TModel>
    where TModel : class
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    protected readonly DbSet<TModel> _dbSet;
    private readonly ILogger _logger;

    protected HashSet<string> _expandProperties = new();

    protected BaseRepository(TDbContext dbContext, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<BaseRepository<TModel, TDbContext>>();

        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TModel>();
    }

    public async Task InsertAsync(TModel model)
    {
        await _dbSet.AddAsync(model);
    }

    public async Task UpdateAsync(TModel model)
    {
        _dbSet.Update(model);
    }

    public async Task DeleteByPredicateAsync(Expression<Func<TModel, bool>> predicate)
    {
        var model = await _dbSet
            .AsNoTracking()
            .Where(predicate)
            .FirstOrDefaultAsync();

        if (model is null)
            return;

        _dbSet.Remove(model);
    }
    
    public async Task<IEnumerable<TModel>> ListByPredicateAsync(Expression<Func<TModel, bool>> predicate)
    {
        return await _dbSet
            .AsNoTracking()
            .Inflate(_expandProperties)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<TModel?> FindByPredicateAsync(Expression<Func<TModel, bool>> predicate)
    {
        return await _dbSet
            .AsNoTracking()
            .Inflate(_expandProperties)
            .Where(predicate)
            .FirstOrDefaultAsync();
    }

    public async Task<(int pageNumber, int pageSize, int lastPage, IEnumerable<TModel> items)> ListPaginateAsync(
        Expression<Func<TModel, bool>> predicate, int pageNumber, int pageSize)
    {
        var count = await _dbSet
            .AsNoTracking()
            .Where(predicate)
            .CountAsync();

        if (count == 0)
            return (pageNumber, pageSize, 1, new List<TModel>());

        var elements = await _dbSet
            .Inflate(_expandProperties)
            .Where(predicate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = count / pageSize;
        if (count % pageSize != 0) totalPages++;

        return (pageNumber, pageSize, totalPages, elements);
    }

    protected async Task SaveChangesAsync()
    {
        _logger.LogInformation("Storing context data!");
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();
    }
}