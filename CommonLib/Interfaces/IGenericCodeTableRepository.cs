using System;
using System.Collections.Generic;

namespace CommonLib.Interfaces
{
    public interface IGenericCodeTableRepository<TEntity>
    {
        TEntity GetByCode(String code);
        bool Add(TEntity entity);
        bool Edit(TEntity entity);
        bool Delete(TEntity entity);
        IEnumerable<TEntity> List();
    }
}