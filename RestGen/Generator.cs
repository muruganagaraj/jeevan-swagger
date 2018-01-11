namespace RestGen
{
    public abstract class Generator
    {
        public abstract string Generate(RestDefinition definition);
    }

    public abstract class Generator<TOptions> : Generator
        where TOptions : GenerateOptions, new()
    {
        private TOptions _options = new TOptions();

        public TOptions Options
        {
            get { return _options ?? (_options = new TOptions()); }
            internal set { _options = value; }
        }
    }
}