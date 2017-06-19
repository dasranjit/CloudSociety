using System;
using System.Collections.Generic;

namespace CommonLib.Interfaces
{
    public interface IReadOnlyCodeTableRepository<TEntity>
    {
        TEntity GetByCode(String code);
        IEnumerable<TEntity> List();
    }
}