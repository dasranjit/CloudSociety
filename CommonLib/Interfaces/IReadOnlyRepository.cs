using System;
using System.Collections.Generic;

namespace CommonLib.Interfaces
{
    public interface IReadOnlyRepository<TEntity>
    {
        TEntity GetById(Guid id);
        IEnumerable<TEntity> List();
    }
}