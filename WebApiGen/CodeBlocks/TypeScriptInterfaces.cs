using WebApiGen.Definition;

namespace WebApiGen.CodeBlocks
{
    public sealed class TypeScriptInterfaces : CodeBlock
    {
        public TypeScriptInterfaces(WebApiDefinition definition, GenerateOptions options) : base(definition, options)
        {
        }

        public override void Generate(CodeBuilder code)
        {
            string qualifier = string.IsNullOrWhiteSpace(Options.ServiceClientNamespace) ? "public" : "export";
            foreach (ControllerDefinition controller in Definition.Controllers)
            {
                using (code.Block("{0} interface I{1}Service", qualifier, controller.Name))
                {
                    foreach (ActionDefinition action in controller.Actions)
                        code.Line(TypeScriptHelper.GetActionMethodSignature(action) + ";");
                }
            }
        }
    }
}