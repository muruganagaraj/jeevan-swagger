using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

using WebApiGen.Definition;

namespace WebApiGen.DefinitionBuilder
{
    public sealed class ApiDescriptionDefinitionBuilder : DefinitionBuilderBase
    {
        private readonly IReadOnlyList<ApiDescription> _apiDescriptions;
        private readonly Type[] _excludedControllers;

        public ApiDescriptionDefinitionBuilder(IReadOnlyList<ApiDescription> apiDescriptions, params Type[] excludedControllers)
        {
            if (apiDescriptions == null)
                throw new ArgumentNullException("apiDescriptions");
            _apiDescriptions = apiDescriptions;
            _excludedControllers = excludedControllers ?? new Type[0];
        }

        protected override WebApiDefinition BuildDefinition()
        {
            var definition = new WebApiDefinition();

            ILookup<HttpControllerDescriptor, ApiDescription> controllerGroups = _apiDescriptions.ToLookup(desc => desc.ActionDescriptor.ControllerDescriptor);
            foreach (IGrouping<HttpControllerDescriptor, ApiDescription> controllerGroup in controllerGroups)
            {
                if (_excludedControllers.Any(ec => controllerGroup.Key.ControllerType == ec))
                    continue;

                string controllerName = Options.Casing.ServiceClasses.Case(controllerGroup.Key.ControllerName);
                var controller = new ControllerDefinition(controllerName);
                definition.AddController(controller);

                foreach (ApiDescription description in controllerGroup)
                {
                    string actionName = Options.Casing.ServiceMethods.Case(description.ActionDescriptor.ActionName);
                    Type responseType = description.ActionDescriptor.ReturnType;
                    if (responseType == typeof(IHttpActionResult) || responseType == typeof(HttpResponseMessage))
                    {
                        Collection<ResponseTypeAttribute> responseTypeAttributes = description.ActionDescriptor.GetCustomAttributes<ResponseTypeAttribute>();
                        if (responseTypeAttributes == null || responseTypeAttributes.Count == 0)
                            throw new Exception("Web API operations that return IHttpActionResult or HttpResponseMessage should be decorated with a ResponseType attribute.");
                        responseType = responseTypeAttributes[0].ResponseType;
                    }
                    string returnType = TypeManager.RegisterType(responseType);
                    string method = description.HttpMethod.ToString();
                    string relativeUrl = description.RelativePath;
                    var action = new ActionDefinition(actionName, returnType, method, relativeUrl);
                    controller.AddAction(action);

                    Collection<ApiParameterDescription> parameters = description.ParameterDescriptions;
                    foreach (ApiParameterDescription parameter in parameters)
                    {
                        string parameterName = Options.Casing.ServiceMethodParameters.Case(parameter.Name);
                        string parameterType = TypeManager.RegisterType(parameter.ParameterDescriptor.ParameterType);
                        bool isComplexType = TypeManager.IsCustomType(parameterType);
                        ActionParameterKind parameterKind = parameter.Source == ApiParameterSource.FromUri ? ActionParameterKind.Uri : ActionParameterKind.Body;
                        action.AddParameter(new ActionParameterDefinition(parameterName, parameterType, parameterKind, isComplexType, parameter.ParameterDescriptor.ParameterType));
                    }
                }
            }

            //TODO: Can we get rid of these 2 collections and get direct access to the collections exposed by TypeManager?
            foreach (CustomType customType in TypeManager.CustomTypes)
                definition.CustomTypes.Add(customType);
            foreach (Type enumType in TypeManager.EnumTypes)
                definition.EnumTypes.Add(enumType);

            return definition;
        }
    }
}