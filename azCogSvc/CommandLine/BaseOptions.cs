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

        public OutputFormat ParseOutputFormat()
        {
            var normalised = string.IsNullOrWhiteSpace(Output) ? string.Empty : Output.Trim().ToLowerInvariant();
            switch (normalised)
            {
                case "json":
                    return OutputFormat.Json;
                case "table":
                    return OutputFormat.Table;
            }
            return OutputFormat.None;
        }

        public override string ToString()
        {
            return $"Location: {Location}, Key: {IsApiKeySupplied()}, Output format: {Output}";
        }
    }

    public enum OutputFormat
    {
        None,
        Json,
        Table
    }
}