using System;
using System.Collections.Generic;

namespace CommonLib.Interfaces
{
    public interface IGenericAppInfoRepository<TEntity>
    {
        TEntity Get();
        bool Edit(TEntity entity);
    }
}