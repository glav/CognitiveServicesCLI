using azCogSvc.CommandLine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace azCogSvc.Handlers.ComputerVision
{
    class ComputerVisionHandler
    {
        ComputerVisionOptions _options;

        public ComputerVisionHandler(ComputerVisionOptions options)
        {
            _options = options;
        }

        public async Task<bool> ExecuteAsync()
        {
            Console.WriteLine(_options);
            return await Task.FromResult(true);
        }
    }
}
