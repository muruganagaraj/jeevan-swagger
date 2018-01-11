using System.Diagnostics;

using WebApiGen.Definition;

namespace WebApiGen.CodeBlocks
{
    public abstract class CodeBlock
    {
        private readonly WebApiDefinition _definition;
        private readonly GenerateOptions _options;

        protected CodeBlock(WebApiDefinition definition, GenerateOptions options)
        {
            _definition = definition;
            _options = options;
        }

        public abstract void Generate(CodeBuilder code);

        protected WebApiDefinition Definition
        {
            [DebuggerStepThrough]
            get { return _definition; }
        }

        protected GenerateOptions Options
        {
            [DebuggerStepThrough]
            get { return _options; }
        }
    }
}