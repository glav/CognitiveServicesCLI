using azCogSvc.CommandLine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace azCogSvc.Handlers
{
    class TextAnalyticHandler
    {
        TextAnalyticsOptions _options;

        public TextAnalyticHandler(TextAnalyticsOptions options)
        {
            _options = options;
        }

        public async Task ExecuteAsync()
        {
            Console.WriteLine(_options);
            await Task.FromResult(0);
        }
    }
}
