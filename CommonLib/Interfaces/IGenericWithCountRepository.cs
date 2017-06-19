using System;
using System.Collections.Generic;

namespace CommonLib.Interfaces
{
    public interface IGenericWithCountRepository<TEntity> : IReadOnlyRepository<TEntity>
    {
        bool Add(TEntity entity);
        bool Edit(TEntity entity);
        bool Delete(TEntity entity);
        int Count();
    }
}
