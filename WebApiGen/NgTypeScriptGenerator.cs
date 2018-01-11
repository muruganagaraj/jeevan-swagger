using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using WebApiGen.CodeBlocks;
using WebApiGen.Definition;

namespace WebApiGen
{
    public sealed class NgTypeScriptGenerator : Generator
    {
        private CodeBuilder _code;

        public NgTypeScriptGenerator(IDefinitionBuilder definitionBuilder, GenerateOptions options = null) : base(definitionBuilder, options)
        {
        }

        public override string Run()
        {
            _code = new CodeBuilder();

            bool namespaceSpecified = !string.IsNullOrWhiteSpace(Options.ServiceClientNamespace);
            IDisposable serviceClientNs = namespaceSpecified ? _code.Block("module {0}", Options.ServiceClientNamespace) : _code.DummyBlock();
            using (serviceClientNs)
            {
                InsertCodeBlock<TypeScriptCustomTypes>(_code);
                InsertCodeBlock<TypeScriptInterfaces>(_code);

                //string qualifier = namespaceSpecified ? "export" : "public";
                //foreach (ControllerDefinition controller in Definition.Controllers)
                //    GenerateControllerClass(controller, qualifier);
            }

            return _code.ToString();
        }

        private void GenerateControllerClass(ControllerDefinition controller, string qualifier)
        {
            using (_code.Block("{0} class {1}Service implements I{1}Service", qualifier, controller.Name))
            {
                _code.Line("static $inject = ['$http', '$q'];");
                using (_code.Block("constructor(private http: ng.IHttpService, private q: ng.IQService)"))
                {
                }
                foreach (ActionDefinition action in controller.Actions)
                {
                    using (_code.Block("public {0}", TypeScriptHelper.GetActionMethodSignature(action, s => string.Format("ng.IPromise<{0}>", s))))
                    {
                        _code.Line("var deferred = this.q.defer<{0}>();", action.ReturnType);
                        _code.Line("this.http<{0}>({{", action.ReturnType);
                        using (_code.Indent())
                        {
                            _code.Line("method: '{0}',", action.Method);
                            _code.Line("url: '{0}',", FormatRouteUrl(action.RouteUrl, action.Parameters));
                            ActionParameterDefinition fromBodyParameter = action.Parameters.FirstOrDefault(p => p.Kind == ActionParameterKind.Body);
                            if (fromBodyParameter != null)
                                _code.Line("data: {0},", fromBodyParameter.Name);
                        }
                        _code.Line("})");
                        _code.Line(".success((data, status, headers, config) => deferred.resolve(data))");
                        _code.Line(".error((data, status, headers, config) => deferred.reject(data));");
                        _code.Line("return deferred.promise;");
                    }
                }
            }
        }

        private static string FormatRouteUrl(string routeUrl, IEnumerable<ActionParameterDefinition> parameters)
        {
            foreach (ActionParameterDefinition parameter in parameters)
            {
                if (!parameter.IsComplexType)
                    routeUrl = routeUrl.Replace("{" + parameter.Name + "}", "' + " + parameter.Name + " + '");
            }
            return routeUrl;
        }
    }
}