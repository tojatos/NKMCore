using System.Collections.Generic;
using System.Linq;

namespace NKMCore
{
    public class SelectableManager
    {
        private readonly List<SelectableProperties> _propertiesList = new List<SelectableProperties>();
        public int Register(SelectableProperties properties)
        {
            _propertiesList.Add(properties);
            return _propertiesList.IndexOf(properties);
        }

        public SelectableProperties Get(int index) => _propertiesList.ElementAtOrDefault(index);
    }
}