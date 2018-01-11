using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

using WebApiGen.Definition;

namespace WebApiGen.DefinitionBuilder
{
    public sealed class ApiDescriptionDefinitionBuilder : DefinitionBuilderBase
    {
        private readonly IReadOnlyList<ApiDescription> _apiDescriptions;

        public ApiDescriptionDefinitionBuilder(IReadOnlyList<ApiDescription> apiDescriptions)
        {
            if (apiDescriptions == null)
                throw new ArgumentNullException("apiDescriptions");
            _apiDescriptions = apiDescriptions;
        }

        protected override WebApiDefinition BuildDefinition(DefinitionBuilderOptions options)
        {
            var definition = new WebApiDefinition();

            ILookup<HttpControllerDescriptor, ApiDescription> controllerGroups = _apiDescriptions.ToLookup(desc => desc.ActionDescriptor.ControllerDescriptor);
            foreach (IGrouping<HttpControllerDescriptor, ApiDescription> controllerGroup in controllerGroups)
            {
                string controllerName = Case(controllerGroup.Key.ControllerName, options.Casing.ServiceClasses);
                var controller = new ControllerDefinition(controllerName);
                definition.AddController(controller);

                foreach (ApiDescription description in controllerGroup)
                {
                    string actionName = Case(description.ActionDescriptor.ActionName, options.Casing.ServiceMethods);
                    string returnType = TypeManager.RegisterType(description.ActionDescriptor.ReturnType);
                    var action = new ActionDefinition(actionName, returnType);
                    controller.AddAction(action);

                    Collection<HttpParameterDescriptor> parameters = description.ActionDescriptor.GetParameters();
                    foreach (HttpParameterDescriptor parameter in parameters)
                    {
                        string parameterName = Case(parameter.ParameterName, options.Casing.ServiceMethodParameters);
                        string parameterType = TypeManager.RegisterType(parameter.ParameterType);
                        action.AddParameter(new ActionParameterDefinition(parameterName, parameterType));
                    }
                }
            }

            foreach (CustomType customType in TypeManager.CustomTypes)
                definition.CustomTypes.Add(customType);
            return definition;
        }
    }
}