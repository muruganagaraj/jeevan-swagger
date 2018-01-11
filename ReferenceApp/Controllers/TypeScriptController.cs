using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

using WebApiGen;
using WebApiGen.DefinitionBuilder;

namespace ReferenceApp.Controllers
{
    public sealed class TypeScriptController : ApiController
    {
        public HttpResponseMessage Get(string ns, bool enumAsStrings = true)
        {
            var definitionBuilder = new ApiDescriptionDefinitionBuilder(GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions, GetType());
            var generator = new NgTypeScriptGenerator(definitionBuilder, new GenerateOptions
            {
                CustomTypeNamespace = ns,
                ServiceClientNamespace = ns,
                EnumAsStrings = enumAsStrings,
            });
            string code = generator.Run();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(code, Encoding.UTF8, "text/plain")
            };
        }
    }
}