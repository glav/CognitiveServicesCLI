using System;
using System.Collections.Generic;
using System.Text;

namespace azCogSvc.CommandLine
{
    public abstract class BaseOptions
    {
        public const string OutputTable = "table";
        public const string OutputJson = "json";

        public BaseOptions()
        {
            Output = OutputTable;
        }
        public string Location { get; set; }
        public string ApiKey { get; set; }

        public string Output { get; set; }

        protected string IsApiKeySupplied()
        {
            return string.IsNullOrWhiteSpace(ApiKey) ? "-NotSupplied-" : "**********";
        }

        public override string ToString()
        {
            return $"Location: {Location}, Key: {IsApiKeySupplied()}, Output format: {Output}";
        }
    }
}