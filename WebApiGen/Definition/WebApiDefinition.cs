using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WebApiGen.Definition
{
    /// <summary>
    ///     Represents the object model of the Web API.
    /// </summary>
    [DebuggerDisplay("Web API Definition: {Controllers.Count} controllers, {CustomTypes.Count} custom types")]
    public sealed class WebApiDefinition
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<ControllerDefinition> _controllers = new List<ControllerDefinition>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IList<CustomType> _customTypes = new List<CustomType>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IList<Type> _enumTypes = new List<Type>();

        public void AddController(ControllerDefinition controller)
        {
            _controllers.Add(controller);
        }

        /// <summary>
        ///     List of controller definitions.
        /// </summary>
        public IReadOnlyList<ControllerDefinition> Controllers
        {
            [DebuggerStepThrough]
            get { return _controllers; }
        }

        public IList<CustomType> CustomTypes
        {
            [DebuggerStepThrough]
            get { return _customTypes; }
        }

        public IList<Type> EnumTypes
        {
            [DebuggerStepThrough]
            get { return _enumTypes; }
        }
    }

    [DebuggerDisplay("Controller {Name} ({Actions.Count} actions)")]
    public sealed class ControllerDefinition
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _name;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<ActionDefinition> _actions = new List<ActionDefinition>();

        public ControllerDefinition(string name)
        {
            _name = name;
        }

        public void AddAction(ActionDefinition action)
        {
            _actions.Add(action);
        }

        public string Name
        {
            [DebuggerStepThrough]
            get { return _name; }
        }

        public IReadOnlyList<ActionDefinition> Actions
        {
            [DebuggerStepThrough]
            get { return _actions; }
        }
    }

    [DebuggerDisplay("Action {Name} ({Parameters.Count} parameters, returns {ReturnType}")]
    public sealed class ActionDefinition
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _name;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<ActionParameterDefinition> _parameters = new List<ActionParameterDefinition>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _returnType;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _method;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _routeUrl;

        public ActionDefinition(string name, string returnType, string method, string routeUrl)
        {
            _name = name;
            _returnType = returnType;
            _method = method;
            _routeUrl = routeUrl;
        }

        public string Name
        {
            [DebuggerStepThrough]
            get { return _name; }
        }

        public IReadOnlyList<ActionParameterDefinition> Parameters
        {
            [DebuggerStepThrough]
            get { return _parameters; }
        }

        public string ReturnType
        {
            [DebuggerStepThrough]
            get { return _returnType; }
        }

        public string Method
        {
            [DebuggerStepThrough]
            get { return _method; }
        }

        public string RouteUrl
        {
            [DebuggerStepThrough]
            get { return _routeUrl; }
        }

        public void AddParameter(ActionParameterDefinition parameter)
        {
            _parameters.Add(parameter);
        }
    }

    [DebuggerDisplay("{Name}: {Type}")]
    public sealed class ActionParameterDefinition
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _name;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _type;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ActionParameterKind _kind;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly bool _isComplexType;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Type _reflectedType;

        public ActionParameterDefinition(string name, string type, ActionParameterKind kind, bool isComplexType, Type reflectedType)
        {
            _name = name;
            _type = type;
            _kind = kind;
            _isComplexType = isComplexType;
            _reflectedType = reflectedType;
        }

        public string Name
        {
            [DebuggerStepThrough]
            get { return _name; }
        }

        public string Type
        {
            [DebuggerStepThrough]
            get { return _type; }
        }

        public ActionParameterKind Kind
        {
            [DebuggerStepThrough]
            get { return _kind; }
        }

        public bool IsComplexType
        {
            [DebuggerStepThrough]
            get { return _isComplexType; }
        }

        public Type ReflectedType
        {
            [DebuggerStepThrough]
            get { return _reflectedType; }
        }
    }

    public enum ActionParameterKind
    {
        Uri,
        Body,
    }

    [DebuggerDisplay("Custom Type: {Name} ({Properties.Count} properties)")]
    public sealed class CustomType
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _name;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _baseTypeName;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<CustomTypeProperty> _properties = new List<CustomTypeProperty>();

        public CustomType(string name, string baseTypeName = null)
        {
            _name = name;
            _baseTypeName = baseTypeName;
        }

        public string Name
        {
            [DebuggerStepThrough]
            get { return _name; }
        }

        public string BaseTypeName
        {
            [DebuggerStepThrough]
            get { return _baseTypeName; }
        }

        public IReadOnlyList<CustomTypeProperty> Properties
        {
            [DebuggerStepThrough]
            get { return _properties; }
        }

        internal void AddProperty(CustomTypeProperty property)
        {
            _properties.Add(property);
        }
    }

    [DebuggerDisplay("{Name}: {Type}")]
    public sealed class CustomTypeProperty
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _name;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _type;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly bool _required;

        public CustomTypeProperty(string name, string type, bool required)
        {
            _name = name;
            _type = type;
            _required = required;
        }

        public string Name
        {
            [DebuggerStepThrough]
            get { return _name; }
        }

        public string Type
        {
            [DebuggerStepThrough]
            get { return _type; }
        }

        public bool Required
        {
            [DebuggerStepThrough]
            get { return _required; }
        }
    }
}
