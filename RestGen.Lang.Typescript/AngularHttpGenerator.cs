using System;
using System.Collections.Generic;
using System.Linq;

namespace RestGen.Lang.Typescript
{
    public sealed class AngularHttpGenerator : TypescriptGenerator
    {
        protected override void GenerateInterfaces(CodeBuilder code, RestDefinition definition)
        {
            Tuple<IDisposable, string> blockAndQualifier = GetBlockAndQualifier(code, Options.Ns.Interfaces);

            using (blockAndQualifier.Item1)
            {
                foreach (ServiceDefinition service in definition.Services.OrderBy(sd => sd.Name))
                {
                    using (code.Block("{0} interface I{1}WebService", blockAndQualifier.Item2, service.Name))
                    {
                        foreach (OperationDefinition operation in service.Operations)
                            code.Line(GetMethodSignature(operation) + ";");
                    }
                }
            }
        }

        protected override void GenerateImplementations(CodeBuilder code, RestDefinition definition)
        {
            Tuple<IDisposable, string> blockAndQualifier = GetBlockAndQualifier(code, Options.Ns.Implementations);

            using (blockAndQualifier.Item1)
            {
                foreach (ServiceDefinition service in definition.Services.OrderBy(sd => sd.Name))
                {
                    string qualifiedInterfaceName = QualifyWithNs("I" + service.Name + "WebService", Options.Ns.Interfaces);
                    using (code.Block("{0} class {1}WebService implements {2}", blockAndQualifier.Item2, service.Name, qualifiedInterfaceName))
                    {
                        if (Options.InjectionApproach == InjectionApproach.Annotation)
                            code.Line("/* @ngInject */");
                        else
                            code.Line("public static $inject: string[] = ['$http', '$q', 'config'];");
                        using (code.Block("constructor (private $http: angular.IHttpService, private $q: angular.IQService, private config: common.config.IConfig)"))
                        {
                        }

                        foreach (OperationDefinition operation in service.Operations)
                        {
                            using (code.Block("public {0}", GetMethodSignature(operation)))
                            {
                                //Code to build the relative URL with the resource parameters.
                                code.Code("let resourceUrl: string = '{0}'", operation.Path);
                                foreach (ParameterDefinition parameter in operation.Parameters.Where(p => p.Location == ParameterLocation.Path))
                                    code.Then(".replace('{{{0}}}', {0}.toString())", parameter.Name);
                                code.Then(";").Line();

                                //Code to add any query string parameters to the end of the relative URL.
                                using (code.BlockTerminated("let queryParams: any ="))
                                {
                                    List<ParameterDefinition> queryParameters =
                                        operation.Parameters.Where(p => p.Location == ParameterLocation.Query).ToList();
                                    for (int i = 0; i < queryParameters.Count; i++)
                                    {
                                        ParameterDefinition parameter = queryParameters[i];
                                        code.Code(" {0}: {0}", parameter.Name);
                                        if (i < queryParameters.Count - 1)
                                            code.Then(",");
                                        code.Line();
                                    }
                                }

                                string successType = GetTypeSignature(operation.SuccessResponseType, true);
                                code.Line("return new this.$q<{0}>(", successType);
                                using (code.Indent())
                                {
                                    using (code.Block("(resolve: angular.IQResolveReject<{0}>, reject: angular.IQResolveReject<any>) =>", successType))
                                    {
                                        using (code.Block("this.$http<{0}>(", successType))
                                        {
                                            code.Line("method: '{0}',", operation.Verb.ToUpperInvariant());
                                            ParameterDefinition bodyParameter = operation.Parameters.FirstOrDefault(
                                                    p => p.Location == ParameterLocation.Body);
                                            if (bodyParameter != null)
                                                code.Line("data: {0},", bodyParameter.Name);
                                            code.Line("url: buildServiceUrl(this.config.apiBaseUrl, resourceUrl, queryParams)");
                                        }
                                        using (code.Block(").success((data: {0}, status: number, headers: angular.IHttpHeadersGetter, config: angular.IRequestConfig) =>", successType))
                                            code.Line("resolve(data);");
                                        using (code.Block(").error((data: any, status: number, headers: angular.IHttpHeadersGetter, config: angular.IRequestConfig) =>"))
                                            code.Line("reject(data);");
                                        code.Line(");");
                                    }
                                }
                                code.Line(");");
                            }
                        }
                    }

                    code.Line("angular.module('{0}').service('{1}WebService', {2}WebService);", Options.NgModule ?? "app", service.Name.ToCamelCase(), service.Name);
                }

                GenerateUrlBuilderFunction(code);
            }
        }
    }
}