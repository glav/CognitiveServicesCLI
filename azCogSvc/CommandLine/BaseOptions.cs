using System;
using System.Collections.Generic;
using System.Text;

namespace azCogSvc.CommandLine
{
    public abstract class BaseOptions
    {
        public string Location { get; set; }
        public string ApiKey { get; set; }

        protected string IsApiKeySupplied()
        {
            return string.IsNullOrWhiteSpace(ApiKey) ? "-NotSupplied-" : "**********";
        }

        public override string ToString()
        {
            return $"Location: {Location}, Key: {IsApiKeySupplied()}";
        }
    }
}