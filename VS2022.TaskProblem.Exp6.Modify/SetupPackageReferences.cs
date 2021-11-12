using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VS2022.TaskProblem.Exp6.Modify
{
    public class SetupPackageReferences : IEnumerable<SetupPackageReference>, IReadOnlyDictionary<string, SetupPackageReference?>
    {
        public const string MSBuild = "Microsoft.Component.MSBuild";

        private readonly Dictionary<string, SetupPackageReference> references = new();

        public int Count => references.Count;
        public IEnumerable<string> Keys => references.Keys;
        public IEnumerable<SetupPackageReference?> Values => references.Values;

        public SetupPackageReferences(SetupInstance setupInstance)
        {
            foreach (SetupPackageReference reference in setupInstance.Packages)
            {
                references[reference.Id] = reference;
            }
        }

        public SetupPackageReference? this[string key]
            => TryGetValue(key, out SetupPackageReference? value) ? value : null;

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out SetupPackageReference? value)
            => references.TryGetValue(key, out value);

        public bool ContainsKey(string key) => references.ContainsKey(key);

        public IEnumerator<SetupPackageReference> GetEnumerator() => references.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => references.Values.GetEnumerator();

        IEnumerator<KeyValuePair<string, SetupPackageReference?>> IEnumerable<KeyValuePair<string, SetupPackageReference?>>.GetEnumerator()
            => references.GetEnumerator();
    }
}
