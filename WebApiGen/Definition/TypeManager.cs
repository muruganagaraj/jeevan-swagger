using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApiGen.Definition
{
    [DebuggerDisplay("Type Manager: {CustomTypes.Count} custom types")]
    public sealed class TypeManager
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DefinitionBuilderOptions _options;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<CustomType> _customTypes = new List<CustomType>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<Type> _enumTypes = new List<Type>();

        public TypeManager(DefinitionBuilderOptions options)
        {
            _options = options;
        }

        public string RegisterType(Type type)
        {
            return GetTypeName(type);
        }

        public IReadOnlyList<CustomType> CustomTypes
        {
            [DebuggerStepThrough]
            get { return _customTypes; }
        }

        public IReadOnlyList<Type> EnumTypes
        {
            [DebuggerStepThrough]
            get { return _enumTypes; }
        }

        public bool IsCustomType(string typeName)
        {
            return _customTypes.Any(type => type.Name == typeName);
        }

        private string GetTypeName(Type type)
        {
            if (type == null)
                return "void";

            if (type.IsEnum)
            {
                _enumTypes.Add(type);
                return type.Name;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
                return GetTypeName(type.GetGenericArguments()[0]);

            string typeName;
            if (PrimitiveTypes.TryGetValue(type, out typeName))
                return typeName;
            if (TryGetTypeAsCollection(type, out typeName))
                return typeName;

            CustomType customType = _customTypes.Find(ct => ct.Name.Equals(type.Name, StringComparison.Ordinal));
            if (customType != null)
                return customType.Name;
            if (type.IsClass)
                return ReadCustomType(type);

            return "any";
        }

        private string ReadCustomType(Type type)
        {
            string baseTypeName = null;
            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                ReadCustomType(type.BaseType);
                baseTypeName = type.BaseType.Name;
            }

            var customType = new CustomType(type.Name, baseTypeName);
            _customTypes.Add(customType);

            List<PropertyInfo> properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetIndexParameters().Length == 0).ToList();
            foreach (PropertyInfo property in properties)
            {
                Attribute requiredAttribute = Attribute.GetCustomAttribute(property, typeof(RequiredAttribute));
                customType.AddProperty(new CustomTypeProperty(_options.Casing.CustomTypeProperties.Case(property.Name),
                    GetTypeName(property.PropertyType), requiredAttribute != null));
            }

            return customType.Name;
        }

        private bool TryGetTypeAsCollection(Type type, out string typeName)
        {
            IEnumerable<Type> interfaces = type.GetInterfaces().Concat(new[] { type });

            Type genericEnumerable = interfaces.FirstOrDefault(intf => intf.IsGenericType && intf.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (genericEnumerable != null)
            {
                Type enumerableType = genericEnumerable.GetGenericArguments()[0];
                typeName = GetTypeName(enumerableType) + "[]";
                return true;
            }

            if (interfaces.Any(intf => intf == typeof(IEnumerable)))
            {
                typeName = "any[]";
                return true;
            }

            typeName = null;
            return false;
        }

        private static readonly Dictionary<Type, string> PrimitiveTypes = new Dictionary<Type, string> {
            { typeof(object), "any" },
            { typeof(string), "string" },
            { typeof(char), "string" },
            { typeof(bool), "boolean" },
            { typeof(sbyte), "number" },
            { typeof(byte), "number" },
            { typeof(short), "number" },
            { typeof(ushort), "number" },
            { typeof(int), "number" },
            { typeof(uint), "number" },
            { typeof(long), "number" },
            { typeof(ulong), "number" },
            { typeof(float), "number" },
            { typeof(double), "number" },
            { typeof(decimal), "number" },
            { typeof(void), "void" },
        };
    }
}