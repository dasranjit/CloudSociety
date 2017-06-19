using System;
using System.Collections.Generic;

namespace CommonLib.Interfaces
{
    public interface IGenericComboKeyRepository<TEntity>
    {
        TEntity GetByIds(Guid parentid,Guid id);
        bool Add(TEntity entity);
        bool Edit(TEntity entity);
        bool Delete(TEntity entity);
        IEnumerable<TEntity> ListByParentId(Guid parentid);
        IEnumerable<TEntity> ListById(Guid id);
    }
}