using System;
using System.Collections.Generic;

namespace CommonLib.Interfaces
{
    public interface IGenericIDCodeComboKeyRepository<TEntity>
    {
        TEntity GetByIdCode(Guid parentid,String code);
        bool Add(TEntity entity);
        bool Edit(TEntity entity);
        bool Delete(TEntity entity);
        IEnumerable<TEntity> ListByParentId(Guid parentid);
    }
}