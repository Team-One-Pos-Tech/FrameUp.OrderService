using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SnackHub.Production.Infra.Repositories.Abstractions;

public interface IBaseRepository<TModel> where TModel : class
{
    
    Task InsertAsync(TModel model);
    Task UpdateAsync(TModel model);
    Task DeleteByPredicateAsync(Expression<Func<TModel, bool>> predicate);
    
    
    Task<IEnumerable<TModel>> ListByPredicateAsync(Expression<Func<TModel, bool>> predicate);
    Task<TModel?> FindByPredicateAsync(Expression<Func<TModel, bool>> predicate);

    Task<(int pageNumber, int pageSize, int lastPage, IEnumerable<TModel> items)> ListPaginateAsync(
        Expression<Func<TModel, bool>> predicate, int pageNumber, int pageSize);
}