namespace CommonLib.Interfaces
{
    public interface IGenericRepository<TEntity> : IReadOnlyRepository<TEntity>
    {
        //TEntity GetById(Guid id);
        bool Add(TEntity entity);
        bool Edit(TEntity entity);
        bool Delete(TEntity entity);
        //IEnumerable<TEntity> List();
    }
}