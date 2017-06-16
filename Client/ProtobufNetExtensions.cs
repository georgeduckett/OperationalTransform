using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class RuntimeTypeModelExt
    {
        public static MetaType Add<T>(this RuntimeTypeModel model)
        { // https://stackoverflow.com/a/18978142
            var publicFields = typeof(T).GetFields().Select(x => x.Name).ToArray();
            return model.Add(typeof(T), false).Add(publicFields);
        }
    }
}
