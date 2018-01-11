using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestGen.Lang.Typescript
{
    public abstract class TypescriptGenerator : Generator<TypescriptGenerateOptions>
    {
        public override string Generate(RestDefinition definition)
        {
            var code = new CodeBuilder();
            GenerateReferencePaths(code);
            GenerateInterfaces(code, definition);
            GenerateImplementations(code, definition);
            GenerateModels(code, definition.Models);
            return code.ToString();
        }

        private void GenerateReferencePaths(CodeBuilder code)
        {
            foreach (string referencePath in Options.ReferencePaths)
                code.Line(@"/// <reference path=""{0}"" />", referencePath);
            if (Options.ReferencePaths.Count > 0)
                code.Line();
        }

        protected abstract void GenerateInterfaces(CodeBuilder code, RestDefinition definition);

        protected abstract void GenerateImplementations(CodeBuilder code, RestDefinition definition);

        protected static void GenerateUrlBuilderFunction(CodeBuilder code)
        {
            using (code.Block("function buildServiceUrl(baseUrl: string, resourceUrl: string, queryParams?: any): string"))
            {
                code.Line("let url: string = baseUrl;");
                code.Line("let baseUrlSlash: boolean = url[url.length - 1] === '/';");
                code.Line("let resourceUrlSlash: boolean = resourceUrl[0] === '/';");
                using (code.Block("if (!baseUrlSlash && !resourceUrlSlash)"))
                    code.Line("url += '/';");
                using (code.Block("else if (baseUrlSlash && resourceUrlSlash)"))
                    code.Line("url = url.substr(0, url.length - 1);");
                code.Line("url += resourceUrl;");
                code.Line();
                using (code.Block("if (queryParams)"))
                {
                    code.Line("let isFirst: boolean = true;");
                    using (code.Block("for (let p in queryParams)"))
                    {
                        using (code.Block("if (queryParams.hasOwnProperty(p) && queryParams[p])"))
                        {
                            code.Line("let separator: string = isFirst ? '?' : '&';");
                            code.Line("url += `${separator}${p}=${encodeURI(queryParams[p])}`;");
                            code.Line("isFirst = false;");
                        }
                    }
                }
                code.Line("return url;");
            }
        }

        private void GenerateModels(CodeBuilder code, ModelDefinitions models)
        {
            Tuple<IDisposable, string> blockAndQualifier = GetBlockAndQualifier(code, Options.Ns.Models);

            using (blockAndQualifier.Item1)
            {
                foreach (ModelDefinition model in models.OrderBy(m => m.Name))
                {
                    using (code.Block("{0} interface {1}", blockAndQualifier.Item2, model.Name))
                    {
                        foreach (ModelPropertyDefinition property in model.Properties)
                        {
                            code.Code("{0}", property.Name);
                            //if (property.Requirement == Requirement.Optional)
                            //    code.Then("?");
                            code.Then(": {0};", GetTypeSignature(property.Type)).Line();
                        }
                    }
                }

                using (code.Block("{0} class ModelFactory", blockAndQualifier.Item2))
                    GenerateModelDefaults(code, models);
            }
        }

        private static void GenerateModelDefaults(CodeBuilder code, ModelDefinitions models)
        {
            foreach (ModelDefinition model in models)
            {
                using (code.Block("public static createEmpty{0}(initializer?: (model: {0}) => void): {0}", model.Name))
                {
                    using (code.BlockTerminated("let model: {0} = ", model.Name))
                    {
                        var count = model.Properties.Count();
                        foreach (ModelPropertyDefinition property in model.Properties)
                        {
                            code.Code("{0}: ", property.Name);
                            if (property.Type.IsCollection)
                                code.Then("[]");
                            else if (property.Type.IsComplex)
                                code.Then("ModelFactory.createEmpty{0}()", property.Type.ComplexType);
                            else
                                code.Then(GetDefaultForPrimitiveType(property.Type.PrimitiveType));
                            if (model.Properties.IndexOf(property) < count - 1)
                            {
                                code.Then(",");
                            }
                            code.Line();
                        }

                    }
                    using (code.Block("if (!!initializer)"))
                        code.Line("initializer(model);");
                    code.Line("return model;");
                }
            }
        }

        private static string GetDefaultForPrimitiveType(Type primitiveType)
        {
            string primitiveTypeString = GetPrimitiveType(primitiveType);
            switch (primitiveTypeString)
            {
                case "string": return "''";
                case "number": return "undefined";
                case "boolean": return "false";
                case "Date": return "undefined";
                case "any": return "{}";
                default:
                    throw new Exception(string.Format("Cannot get a default value for primitive type {0}", primitiveType));
            }
        }

        protected static Tuple<IDisposable, string> GetBlockAndQualifier(CodeBuilder code, string ns)
        {
            return string.IsNullOrEmpty(ns) ? Tuple.Create(code.DummyBlock(), "public") : Tuple.Create(code.Block("namespace {0}", ns), "export");
        }

        protected string GetMethodSignature(OperationDefinition operation)
        {
            var sb = new StringBuilder(operation.Name);
            sb.Append("(");
            for (int i = 0; i < operation.Parameters.Count; i++)
            {
                ParameterDefinition parameter = operation.Parameters[i];
                if (i > 0)
                    sb.Append(", ");
                sb.Append(parameter.Name);
                if (parameter.Requirement == Requirement.Optional)
                    sb.Append("?");
                sb.AppendFormat(": {0}", GetTypeSignature(parameter.Type, true));
                //TODO: Add defaults
            }
            sb.Append(")");
            sb.AppendFormat(": angular.IPromise<{0}>", GetTypeSignature(operation.SuccessResponseType, true));
            return sb.ToString();
        }

        protected string GetTypeSignature(DataType type, bool qualifyWithNs = false)
        {
            string signature;
            if (type.IsPrimitive)
                signature = GetPrimitiveType(type.PrimitiveType);
            else
                signature = qualifyWithNs ? QualifyWithNs(type.ComplexType, Options.Ns.Models) : type.ComplexType;
            if (type.IsCollection)
                signature += "[]";
            return signature;
        }

        protected static string QualifyWithNs(string str, string ns)
        {
            return string.IsNullOrEmpty(ns) ? str : ns + "." + str;
        }

        private static string GetPrimitiveType(Type type)
        {
            string result;
            return _typeMappings.TryGetValue(type, out result) ? result : "any";
        }

        private static readonly Dictionary<Type, string> _typeMappings = new Dictionary<Type, string> {
            { typeof(string), "string" },
            { typeof(char), "string" },
            { typeof(int), "number" },
            { typeof(uint), "number" },
            { typeof(long), "number" },
            { typeof(ulong), "number" },
            { typeof(short), "number" },
            { typeof(ushort), "number" },
            { typeof(byte), "number" },
            { typeof(sbyte), "number" },
            { typeof(float), "number" },
            { typeof(double), "number" },
            { typeof(decimal), "number" },
            { typeof(bool), "boolean" },
            { typeof(DateTime), "Date" },
            { typeof(object), "any" },
            { typeof(void), "void" },
        };
    }
}