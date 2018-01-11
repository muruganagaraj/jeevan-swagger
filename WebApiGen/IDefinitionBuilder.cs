using System;

using WebApiGen.Definition;

namespace WebApiGen
{
    public interface IDefinitionBuilder
    {
        WebApiDefinition Build();
    }

    public enum CasingOption
    {
        CamelCase,
        PascalCase,
    }

    public static class CasingExtension
    {
        public static string Case(this CasingOption casingOption, string identifier)
        {
            Func<char, char> caseFunc = casingOption == CasingOption.PascalCase ? (Func<char, char>)char.ToUpper : char.ToLower;
            return caseFunc(identifier[0]) + identifier.Substring(1);
        }
    }
}