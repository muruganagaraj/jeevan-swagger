using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

using WebApiGen.Definition;

namespace WebApiGen.DefinitionBuilder
{
    public sealed class ReflectionDefinitionBuilder : DefinitionBuilderBase
    {
        private readonly List<Type> _controllerTypes;

        public ReflectionDefinitionBuilder(IEnumerable<Type> controllerTypes)
        {
            if (controllerTypes == null)
                throw new ArgumentNullException("controllerTypes");
            _controllerTypes = new List<Type>(controllerTypes.Where(type => type.IsSubclassOf(typeof(ApiController))));
        }

        public ReflectionDefinitionBuilder(params Type[] controllerTypes) : this((IEnumerable<Type>)controllerTypes)
        {
        }

        public ReflectionDefinitionBuilder(IEnumerable<Assembly> controllerAssemblies)
        {
            _controllerTypes = new List<Type>(controllerAssemblies
                .SelectMany(asm => asm.GetExportedTypes())
                .Where(type => type.IsSubclassOf(typeof(ApiController))));
        }

        public ReflectionDefinitionBuilder(params Assembly[] controllerAssemblies)
            : this((IEnumerable<Assembly>)controllerAssemblies)
        {
        }

        protected override WebApiDefinition BuildDefinition()
        {
            var definition = new WebApiDefinition();
            foreach (Type controllerType in _controllerTypes)
                definition.AddController(BuildControllerDefinition(controllerType));
            foreach (CustomType customType in TypeManager.CustomTypes)
                definition.CustomTypes.Add(customType);
            return definition;
        }

        private ControllerDefinition BuildControllerDefinition(Type controllerType)
        {
            var controller = new ControllerDefinition(controllerType.Name);
            MethodInfo[] methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo method in methods)
                controller.AddAction(BuildActionDefinition(method));
            return controller;
        }

        private ActionDefinition BuildActionDefinition(MethodInfo method)
        {
            string returnType = TypeManager.RegisterType(GetReturnType(method));
            var action = new ActionDefinition(method.Name, returnType, "GET", null);
            ParameterInfo[] parameters = method.GetParameters();
            foreach (ParameterInfo parameter in parameters)
                action.AddParameter(BuildParameterDefinition(parameter));
            return action;
        }

        private Type GetReturnType(MethodInfo method)
        {
            Type returnType = method.ReturnType;

            if (returnType == typeof(IHttpActionResult) || returnType == typeof(HttpResponseMessage))
            {
                var responseTypeAttribute = method.GetCustomAttribute<ResponseTypeAttribute>();
                if (responseTypeAttribute == null)
                    throw new Exception("Web API operations that return IHttpActionResult or HttpResponseMessage should be decorated with a ResponseType attribute to specify the expected return type.");
                return responseTypeAttribute.ResponseType;
            }

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                return returnType.GetGenericArguments()[0];

            return returnType;
        }

        private ActionParameterDefinition BuildParameterDefinition(ParameterInfo parameter)
        {
            string parameterType = TypeManager.RegisterType(parameter.ParameterType);
            return new ActionParameterDefinition(parameter.Name, parameterType, ActionParameterKind.Uri, TypeManager.IsCustomType(parameterType), parameter.ParameterType);
        }
    }
}