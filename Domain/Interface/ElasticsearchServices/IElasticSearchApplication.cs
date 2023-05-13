using Domain.Base;
using Domain.Interface.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.ElasticsearchServices
{
    public interface IElasticSearchApplication<T> where T : Entity, IEntity
    {
        Task SaveSingleAsync(T t);

        Task<T> GetAsync(int id);

        Task<List<T>> GetAll();

    }

}
