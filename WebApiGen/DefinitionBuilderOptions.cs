namespace WebApiGen
{
    public sealed class DefinitionBuilderOptions
    {
        private readonly CasingOptions _casing = new CasingOptions();

        public CasingOptions Casing
        {
            get { return _casing; }
        }

        public sealed class CasingOptions
        {
            internal CasingOptions()
            {
                CustomTypes = CasingOption.PascalCase;
                CustomTypeProperties = CasingOption.CamelCase;
                ServiceClasses = CasingOption.PascalCase;
                ServiceMethods = CasingOption.CamelCase;
                ServiceMethodParameters = CasingOption.CamelCase;
            }

            public CasingOption CustomTypes { get; set; }
            public CasingOption CustomTypeProperties { get; set; }
            public CasingOption ServiceClasses { get; set; }
            public CasingOption ServiceMethods { get; set; }
            public CasingOption ServiceMethodParameters { get; set; }
        }
    }
}
