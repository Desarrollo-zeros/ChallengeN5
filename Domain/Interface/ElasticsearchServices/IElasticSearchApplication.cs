using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.ElasticsearchServices
{
    public interface IElasticSearchApplication<T> where T : class
    {
        Task SaveSingleAsync(T t, int Id);

        Task<T> GetAsync(Guid id);

        Task<List<T>> GetAll();

    }

}
