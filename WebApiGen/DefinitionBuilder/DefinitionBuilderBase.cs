using System.Diagnostics;

using WebApiGen.Definition;

namespace WebApiGen.DefinitionBuilder
{
    public abstract class DefinitionBuilderBase : IDefinitionBuilder
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DefinitionBuilderOptions _options;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TypeManager _typeManager;

        protected DefinitionBuilderBase(DefinitionBuilderOptions options = null)
        {
            _options = options ?? new DefinitionBuilderOptions();
            _typeManager = new TypeManager(_options);
        }

        public WebApiDefinition Build()
        {
            return BuildDefinition();
        }

        protected abstract WebApiDefinition BuildDefinition();

        public DefinitionBuilderOptions Options
        {
            [DebuggerStepThrough]
            get { return _options; }
        }

        protected TypeManager TypeManager
        {
            [DebuggerStepThrough]
            get { return _typeManager; }
        }
    }
}