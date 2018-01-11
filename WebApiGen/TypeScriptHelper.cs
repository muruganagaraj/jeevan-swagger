using System;
using System.Linq;
using System.Text;

using WebApiGen.Definition;

namespace WebApiGen
{
    internal static class TypeScriptHelper
    {
        internal static string GetActionMethodSignature(ActionDefinition action, Func<string, string> returnTypeDecorator = null)
        {
            string parameters = action.Parameters
                .Aggregate(new StringBuilder(), (sb, parameter) => sb.AppendFormat("{0}: {1}, ", parameter.Name, parameter.Type))
                .ToString().Trim().TrimEnd(',');
            string returnType = returnTypeDecorator != null ? returnTypeDecorator(action.ReturnType) : action.ReturnType;
            return string.Format("{0}({1}): {2}", action.Name, parameters, returnType);
        }
    }
}