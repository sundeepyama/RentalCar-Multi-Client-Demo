using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Contracts
{
    public interface IIdentifiableEntity
    {
        public int EntityId { get; set; }
    }
}
