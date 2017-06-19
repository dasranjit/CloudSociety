using System;
using System.Collections.Generic;

namespace CommonLib.Interfaces
{
    public interface IGenericChildRepository<TEntity>
    {
        TEntity GetById(Guid id);
        bool Add(TEntity entity);
        bool Edit(TEntity entity);
        bool Delete(TEntity entity);
        IEnumerable<TEntity> ListByParentId(Guid parentid);
    }
}