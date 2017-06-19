using System;
using System.Collections.Generic;

namespace CommonLib.Interfaces
{
    public interface IReadOnlyChildRepository<TEntity>
    {
        IEnumerable<TEntity> ListByParentId(Guid parentid);
    }
}