using Domain.Base;
using Domain.Interface.Base;
using Domain.Interface.ElasticsearchServices;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ElasticsearchServices
{
    public class ElasticSearchApplication<T> : IElasticSearchApplication<T> where T : Entity, IEntity
    {
        private readonly IElasticClient _elasticClient;
        public ElasticSearchApplication(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<List<T>> GetAll()
        {
            var t = (await _elasticClient.SearchAsync<T>(s =>
                s.Query(q => q
                    .Bool(b => b)
                    ).Size(10000)
            )).Hits.Select(x => x.Source);
            return t.ToList();
        }

        public async Task<T> GetAsync(int id)
        {
            var t = await _elasticClient.GetAsync<T>(id);
            return t.Source;
        }

        public async Task SaveSingleAsync(T t)
        {
            var product = await _elasticClient.GetAsync<T>(t.Id);
            if (product.Found)
            {
                var result = await _elasticClient.UpdateAsync<T>(t, u => u.Doc(t));
            }
            else
            {
                var result = await _elasticClient.IndexDocumentAsync(t);
            }
        }
    }
}
