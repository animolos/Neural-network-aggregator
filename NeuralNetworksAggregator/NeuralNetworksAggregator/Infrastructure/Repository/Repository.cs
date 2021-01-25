using System.Collections.Concurrent;

namespace NeuralNetworksAggregator.Infrastructure.Repository
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private readonly ConcurrentDictionary<string, TEntity> entities
            = new ConcurrentDictionary<string, TEntity>();

        public void CreateOrUpdate(string id, TEntity entity)
        {
            if (!entities.TryAdd(id, entity))
                entities[id] = entity;
        }

        public TEntity Get(string id)
            => entities.TryGetValue(id, out var value) ? value : null;
    }
}
