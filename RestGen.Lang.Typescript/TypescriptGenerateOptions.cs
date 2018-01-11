using System.Collections.Generic;
using System.Diagnostics;

namespace RestGen.Lang.Typescript
{
    public sealed class TypescriptGenerateOptions : GenerateOptions
    {
        private readonly NamespaceOptions _ns = new NamespaceOptions();
        private readonly List<string> _referencePaths = new List<string>();

        public string NgModule { get; set; }
        public InjectionApproach InjectionApproach { get; set; }

        public NamespaceOptions Ns
        {
            [DebuggerStepThrough]
            get { return _ns; }
        }

        public IList<string> ReferencePaths
        {
            [DebuggerStepThrough]
            get { return _referencePaths; }
        }
    }

    public enum InjectionApproach
    {
        InjectArray,
        Annotation,
    }
}
