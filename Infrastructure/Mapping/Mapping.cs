using Domain.Base;
using Domain.Interface.Base;
using Domain.Permissions;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapping
{
    public class Mapping
    {
        public Mapping CreateIndex<T>(IElasticClient elasticClient, string Index) where T : class
        {
            // Crea el índice y aplica el mapping
            var createIndexResponse = elasticClient.Indices.Create(Index, c => c
                .Map<T>(m => m .AutoMap() )
            );
            return this;
        }

        public Mapping AddDefaultMappings<T>(ConnectionSettings settings, string Index) where T : Entity, IEntity
        {
            settings.
                DefaultMappingFor<T>(m => m
                .IdProperty(x => x.Id)
                .IndexName(Index)
            );
            return this;
        }
    }
}
