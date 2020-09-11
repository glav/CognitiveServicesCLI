using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace azCogSvc.CommandLine
{
    public class ParsingConfiguration
    {
        private string[] _args;
        private RootCommand _rootCmd = new RootCommand("A Command line interface (CLI) way of interacting with Azure Cognitive Services");
        private const string cvName = "computer-vision";
        private const string taName = "text-analytics";

        public ParsingConfiguration(string[] args)
        {
            _args = args;
        }
        public async Task SetupAsync()
        {
            var options = BuildOptions();
            options.ForEach(o => _rootCmd.AddOption(o));

            var cmds = BuildCommands();
            cmds.ForEach(c => _rootCmd.AddCommand(c));

            SetupHandlers();

            await _rootCmd.InvokeAsync(_args);
        }

        private void SetupHandlers()
        {
            _rootCmd.Handler = CommandHandler.Create<string, string, bool, bool>((l, k, cv, ta) =>
            {
                var parsedConfig = new Config(l, k,
                                                  cv && ta ? SelectedService.Multiple :
                                                    !cv && !ta ? SelectedService.None :
                                                    cv && !ta ? SelectedService.ComputerVision : SelectedService.TextAnalytics);


                Console.WriteLine($"Config values:\n {parsedConfig}");

            });

        }

        private List<Command> BuildCommands()
        {
            var cmds = new List<Command>();
            // Add and retain the supported cognitive service options so we can build out the specific options for each one
            var cvCmd = new Command("-cv", "Use the Computer Vision Cognitive Service");
            cvCmd.AddAlias($"--{cvName}");
            cmds.Add(cvCmd);
            
            var taCmd = new Command("-ta", "Use the Text Analytics Cognitive Service");
            taCmd.AddAlias($"--{taName}");
            taCmd.AddOption(new Option<bool>(new string[] { "-sa", "--sentiment-analysis" },() => { return true; }, "Perform sentiment analysis"));
            taCmd.AddOption(new Option<bool>(new string[] { "-ka", "--keyphrase-analysis" }, () => { return false; }, "Perform keyphrase analysis"));
            taCmd.AddOption(new Option<bool>(new string[] { "-ld", "--language-detection" }, () => { return false; }, "Perform language detection"));
            taCmd.AddOption(new Option<string>(new string[] { "-txt", "--text-to-analyse" }, "The text to analyse"));
            taCmd.AddOption(new Option<string>(new string[] { "-f", "--filename" }, "File to read as input for the operation"));
            cmds.Add(taCmd);

            return cmds;

        }

        private List<Option> BuildOptions()
        {
            var options = new List<Option>();

            // Add the required elements
            options.Add(new Option<string>(new string[] { "-l", "--location" }, "Location of the Azure Cognitive Resource eg. Australia SouthEast"));
            options[0].IsRequired = true;
            options.Add(new Option<string>(new string[] { "-k", "--api-key" }, "API Key generated for the Azure Cognitive resource"));
            options[1].IsRequired = true;

            return options;
        }
    }
}
