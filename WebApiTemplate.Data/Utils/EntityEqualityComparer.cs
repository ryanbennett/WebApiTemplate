using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiTemplate.Data.Interfaces;

namespace WebApiTemplate.Data.Utils
{
    public class EntityEqualityComparer<T> : IEqualityComparer<T> where T : IEntity
    {
        public static readonly EntityEqualityComparer<T> Default = new EntityEqualityComparer<T>();

        private EntityEqualityComparer()
        {
        }

        public bool Equals(T x, T y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(T obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
