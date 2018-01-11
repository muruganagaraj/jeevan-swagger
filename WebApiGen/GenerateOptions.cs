namespace WebApiGen
{
    public sealed class GenerateOptions
    {
        public string CustomTypeNamespace { get; set; }
        public string ServiceClientNamespace { get; set; }
        public bool EnumAsStrings { get; set; }
    }
}