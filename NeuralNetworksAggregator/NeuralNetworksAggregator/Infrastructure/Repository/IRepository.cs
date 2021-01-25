namespace NeuralNetworksAggregator.Infrastructure.Repository
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        public void CreateOrUpdate(string id, TEntity entity);
        TEntity Get(string id);
    }
}
