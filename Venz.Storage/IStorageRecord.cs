using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venz.Storage
{
    public interface IStorageRecord
    {
        UInt32 Id { get; }

        void Deserialize(DataReader reader);
        void Serialize(DataWriter writer);
    }
}
