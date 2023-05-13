using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Domain.Interface.Base;
using Nest;

namespace Domain.Base
{
    public abstract class Entity<TPrimaryKey> : Entity, IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
        public virtual TPrimaryKey Id { get; set; }

        public object[] GetKeys()
        {
            return new object[] { Id };
        }
    }


    public abstract class Entity : IEntity
    {
        [Number(Name = "id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        public object[] GetKeys()
        {
            return new object[] { Id };
        }
    }
}
