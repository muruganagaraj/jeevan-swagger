using System;

using WebApiGen.Definition;

namespace WebApiGen.CodeBlocks
{
    public sealed class TypeScriptCustomTypes : CodeBlock
    {
        public TypeScriptCustomTypes(WebApiDefinition definition, GenerateOptions options) : base(definition, options)
        {
        }

        public override void Generate(CodeBuilder code)
        {
            string qualifier = string.IsNullOrWhiteSpace(Options.CustomTypeNamespace) ? "public" : "export";
            foreach (CustomType customType in Definition.CustomTypes)
                GenerateCustomType(code, customType, qualifier);
            foreach (Type enumType in Definition.EnumTypes)
                GenerateEnumType(code, enumType, qualifier);
        }

        private static void GenerateCustomType(CodeBuilder code, CustomType customType, string qualifier)
        {
            IDisposable declaration;
            if (string.IsNullOrWhiteSpace(customType.BaseTypeName))
                declaration = code.Block("{0} interface {1}", qualifier, customType.Name);
            else
                declaration = code.Block("{0} interface {1} extends {2}", qualifier, customType.Name, customType.BaseTypeName);
            using (declaration)
            {
                foreach (CustomTypeProperty property in customType.Properties)
                    code.Line("{0}{1}: {2};", property.Name, property.Required ? "" : "?", property.Type);
            }
        }

        private void GenerateEnumType(CodeBuilder code, Type enumType, string qualifier)
        {
            //string[] enumNames = Enum.GetNames(enumType);
            Array enumValues = Enum.GetValues(enumType);
            using (code.Block("{0} enum {1}", qualifier, enumType.Name))
            {
                for (int i = 0; i < enumValues.Length; i++)
                {
                    //string enumName = enumNames[i];
                    if (Options.EnumAsStrings)
                        code.Line("{0} = <any>'{0}',", enumValues.GetValue(i));
                    else
                        code.Line("{0} = {1},", enumValues.GetValue(i), (int)enumValues.GetValue(i));
                }
            }
        }
    }
}