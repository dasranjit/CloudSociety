using System;
using System.Collections.Generic;

namespace CommonLib.Interfaces
{
    public interface IGenericChildChildRepository<TEntity> : IGenericChildRepository<TEntity>
    {
        IEnumerable<TEntity> ListByParentParentId(Guid parentparentid);
    }
}