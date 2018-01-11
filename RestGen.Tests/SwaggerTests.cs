using System.IO;

using RestGen.Lang.Typescript;
using RestGen.Swagger;

using Xunit;

namespace RestGen.Tests
{
    public sealed class SwaggerTests
    {
        [Fact]
        public void TestSwagger()
        {
            string swaggerJson = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "swagger.json"));
            var input = new SwaggerInput(swaggerJson);

            //var input = new SwaggerInput(new Uri("http://localhost:11111/swagger/docs/v1"));

            RestDefinition definition = input.GenerateDefinition();
            Assert.NotNull(definition);

            string c = definition.Generate<AngularHttpGenerator, TypescriptGenerateOptions>(
                o => {
                    o.Ns.Models = "my.model";
                    o.Ns.Interfaces = "my.service";
                    o.Ns.Implementations = "my.service";
                    o.ReferencePaths.Add("../../../typings/lib.d.ts");
                    o.ReferencePaths.Add("../../../typings/app.d.ts");
                });
            Assert.NotNull(c);
            Assert.NotEmpty(c);

            var generator = new AngularHttpGenerator();
            generator.Options.Ns.SetAll("My.Services");
            string code = generator.Generate(definition);
            Assert.NotNull(code);
            Assert.NotEmpty(code);
        }
    }
}
