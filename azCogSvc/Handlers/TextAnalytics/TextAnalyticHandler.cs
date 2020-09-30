using azCogSvc.CommandLine;
using Glav.CognitiveServices.FluentApi.Core;
using Glav.CognitiveServices.FluentApi.Core.Contracts;
using Glav.CognitiveServices.FluentApi.TextAnalytic;
using Glav.CognitiveServices.FluentApi.TextAnalytic.Domain.ApiResponses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace azCogSvc.Handlers.TextAnalytics
{
    class TextAnalyticHandler : IServiceHandlerOutput<TextAnalyticAnalysisResults>
    {
        TextAnalyticsOptions _options;
        TextAnalyticErrorProcessor _errorProcessor;

        public TextAnalyticHandler(TextAnalyticsOptions options)
        {
            _options = options;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (!_options.KeyPhraseAnalysis && !_options.SentimentAnalysis && !_options.LanguageDetection)
            {
                Console.Error.WriteLine("ERROR! At least one analysis option must be specified; Sentiment analysis, Keyphrase analysis or language detection ");
                return await Task.FromResult<bool>(false);
            }
            var location = (LocationKeyIdentifier)System.Enum.Parse(typeof(LocationKeyIdentifier), _options.Location, true);
            var txtToAnalyse = _options.Filename != null ? File.ReadAllText(_options.Filename.FullName) : _options.TextToAnalyse;

            if (string.IsNullOrWhiteSpace(txtToAnalyse))
            {
                Console.Error.WriteLine("ERROR! No text supplied for analysis.");
                return await Task.FromResult<bool>(false);
            }


            var analysis = TextAnalyticConfigurationSettings.CreateUsingConfigurationKeys(_options.ApiKey, location)
                    .AddConsoleDiagnosticLogging()
                    .UsingHttpCommunication()
                    .WithTextAnalyticAnalysisActions()
                    .AddSentimentAnalysisSplitIntoSentences(txtToAnalyse);

            if (_options.KeyPhraseAnalysis)
            {
                analysis = analysis.AddKeyPhraseAnalysis(txtToAnalyse);
            }
            if (_options.LanguageDetection)
            {
                analysis = analysis.AddLanguageAnalysis(txtToAnalyse);
            }
            var result = await analysis.AnalyseAllAsync();

            await ProcessResultsAsync(result); // Note: Not the optimal to return results as we want to output to multiple formats but will do for now.

            return await Task.FromResult<bool>(true);
        }

        public string ToJson(TextAnalyticAnalysisResults results)
        {
            var container = new TextAnalyticJsonResultContainer(_options, results);
            return Newtonsoft.Json.JsonConvert.SerializeObject(container, Newtonsoft.Json.Formatting.Indented);
        }

        public string ToTable(TextAnalyticAnalysisResults results)
        {
            var container = new TextAnalyticTableResultContainer(_options, results);
            return container.ToString();
        }

        private async Task ProcessResultsAsync(TextAnalyticAnalysisResults result)
        {
            _errorProcessor = new TextAnalyticErrorProcessor(_options, result);
            if (! await _errorProcessor.WasCallSuccessfulAsync())
            {
                return;
            }

            if (_options.ParseOutputFormat() == OutputFormat.Json)
            {
                Console.Out.WriteLine(ToJson(result));
            } else if (_options.ParseOutputFormat() == OutputFormat.Table)
            {
                Console.Out.WriteLine(ToTable(result));
            }

            return;
        }


    }
}
