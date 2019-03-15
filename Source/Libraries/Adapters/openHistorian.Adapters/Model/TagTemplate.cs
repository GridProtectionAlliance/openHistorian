// ReSharper disable CheckNamespace
#pragma warning disable 1591

namespace openHistorian.Adapters
{
    public class TagTemplate
    {
        public string TagName { get; set; }

        public string[] Inputs { get; set; } = new string[0];

        public string Equation { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }
    }
}
