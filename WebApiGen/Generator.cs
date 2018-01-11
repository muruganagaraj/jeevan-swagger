using System;
using System.Diagnostics;

using WebApiGen.CodeBlocks;
using WebApiGen.Definition;

namespace WebApiGen
{
    public abstract class Generator
    {
        private readonly WebApiDefinition _definition;
        private readonly GenerateOptions _options;

        protected Generator(IDefinitionBuilder definitionBuilder, GenerateOptions options = null)
        {
            if (definitionBuilder == null)
                throw new ArgumentNullException("definitionBuilder");
            _definition = definitionBuilder.Build();
            _options = options ?? new GenerateOptions();
        }

        public abstract string Run();

        protected void InsertCodeBlock<TCodeBlock>(CodeBuilder code)
            where TCodeBlock : CodeBlock
        {
            var codeBlock = (TCodeBlock)Activator.CreateInstance(typeof(TCodeBlock), new object[] { Definition, Options });
            codeBlock.Generate(code);
        }

        protected WebApiDefinition Definition
        {
            [DebuggerStepThrough]
            get { return _definition; }
        }

        public GenerateOptions Options
        {
            [DebuggerStepThrough]
            get { return _options; }
        }
    }
}