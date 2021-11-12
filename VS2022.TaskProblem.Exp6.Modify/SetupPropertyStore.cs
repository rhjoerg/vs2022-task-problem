using Microsoft.VisualStudio.Setup.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VS2022.TaskProblem.Exp6.Modify
{
    public class SetupPropertyStore : IReadOnlyDictionary<string, object>
    {
        private readonly Dictionary<string, object> properties = new();

        public int Count => properties.Count;
        public IEnumerable<string> Keys => properties.Keys;
        public IEnumerable<object> Values => properties.Values;

        public SetupPropertyStore(ISetupPropertyStore setupPropertyStore)
        {
            foreach (string key in setupPropertyStore.GetNames())
            {
                properties[key] = setupPropertyStore.GetValue(key);
            }
        }

        public object this[string key] => properties[key];
        public bool ContainsKey(string key) => properties.ContainsKey(key);
        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value) => properties.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => properties.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => properties.GetEnumerator();
    }
}
