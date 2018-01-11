using System;
using System.Collections.Generic;
using System.IO;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;
using ConsoleFx.Parser.Validators;
using ConsoleFx.Programs;
using ConsoleFx.Programs.UsageBuilders;

using RestGen;
using RestGen.Lang.Typescript;
using RestGen.Swagger;

namespace WebApiGen.SwaggerGen
{
    public sealed class Runner : SingleCommandProgram
    {
        public Uri SwaggerUrl { get; set; }
        public string OutputFileName { get; set; } = "swagger.ts";
        public string ModelNs { get; set; } = null;
        public string ServiceNs { get; set; } = null;
        public List<string> References { get; } = new List<string>();

        public Runner() : base(new WindowsParserStyle())
        {
            UsageBuilder = new MetadataUsageBuilder();
        }

        protected override int Handle()
        {
            var input = new SwaggerInput(SwaggerUrl);
            RestDefinition definition = input.GenerateDefinition();

            string c = definition.Generate<AngularHttpGenerator, TypescriptGenerateOptions>(
                o => {
                    o.Ns.Models = ModelNs;
                    o.Ns.Interfaces = o.Ns.Implementations = ServiceNs;
                    foreach (string reference in References)
                        o.ReferencePaths.Add(reference);
                });
            File.WriteAllText(OutputFileName, c);
            return 0;
        }

        protected override IEnumerable<Argument> GetArguments()
        {
            yield return CreateArgument()
                .Description("swagger url", "URL to the Swagger endpoint.")
                .ValidateWith(new UriValidator(UriKind.Absolute) {
                    Message = "The specified Swagger URL is invalid."
                })
                .AssignTo(() => SwaggerUrl, uri => new Uri(uri));
        }

        protected override IEnumerable<Option> GetOptions()
        {
            yield return CreateOption("out", "o")
                .Description("Path to the output file. If not specified, outputs to a file named swagger.ts in the current directory.")
                .Optional()
                .ExpectedParameters(1)
                .ValidateWith(new PathValidator(checkIfExists: false))
                .AssignTo(() => OutputFileName);
            yield return CreateOption("model-ns", "mns")
                .Description("Typescript namespace for the model interfaces.")
                .ExpectedParameters(1)
                .AssignTo(() => ModelNs);
            yield return CreateOption("service-ns", "sns")
                .Description("Typescript namespace for the service interfaces and classes.")
                .ExpectedParameters(1)
                .AssignTo(() => ServiceNs);
            yield return CreateOption("def", "d")
                .Description("Absolute or relative path to a Typescript definition file.")
                .Optional(int.MaxValue)
                .ExpectedParameters(int.MaxValue)
                .AddToList(() => References);
        }
    }
}