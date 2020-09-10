using System;
using System.Collections.Generic;
using System.Text;

namespace azCogSvc.CommandLine
{
    public class Config
    {
        public Config(string location, string apiKey, SelectedService cogService)
        {
            SelectedService = cogService;
            Location = location;
            ApiKey = apiKey;
        }
        public SelectedService SelectedService { get; }
        public string Location { get; }
        public string ApiKey { get; }
        public ComputerVisionSettings ComputerVision;
        public TextAnalyticSettings TextAnalytics;

        public override string ToString()
        {
            return $"Selected Service: {this.SelectedService}\nLocation: {this.Location}\nApiKey: {ApiKey}";
        }
    }
}
