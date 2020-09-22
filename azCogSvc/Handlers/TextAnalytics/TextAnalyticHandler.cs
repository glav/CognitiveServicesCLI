using azCogSvc.CommandLine;
using Glav.CognitiveServices.FluentApi.Core;
using Glav.CognitiveServices.FluentApi.Core.Contracts;
using Glav.CognitiveServices.FluentApi.TextAnalytic;
using Glav.CognitiveServices.FluentApi.TextAnalytic.Domain.ApiResponses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azCogSvc.Handlers.TextAnalytics
{
    class TextAnalyticHandler : IServiceHandlerOutput
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

        public string ToJson()
        {
            throw new NotImplementedException();
        }

        public string ToTable()
        {
            throw new NotImplementedException();
        }

        private async Task ProcessResultsAsync(TextAnalyticAnalysisResults result)
        {
            _errorProcessor = new TextAnalyticErrorProcessor(_options, result);
            if (! await _errorProcessor.WasCallSuccessfulAsync())
            {
                return;
            }
            if (_options.SentimentAnalysis && result.SentimentAnalysis.AnalysisResult.ActionSubmittedSuccessfully)
            {
                // To be refactored as output in JSON or Table
                Console.WriteLine("Sentiment Analysis: Action submitted successfully:");
                var allResultItems = result.SentimentAnalysis.GetResults();
                foreach (var resultItem in allResultItems)
                {
                    Console.WriteLine(" {0}: {1} ({2})", resultItem.id, resultItem.score, result.SentimentAnalysis.ScoringEngine.EvaluateScore(resultItem.score).Name);
                }
            }

            if (_options.KeyPhraseAnalysis && result.KeyPhraseAnalysis.AnalysisResult.ActionSubmittedSuccessfully)
            {
                // To be refactored as output in JSON or Table
                Console.WriteLine("Keyphrase Analysis: Action submitted successfully:");
                var allResultItems = result.KeyPhraseAnalysis.AnalysisResults.Select(r => r.ResponseData);
                foreach (var resultItem in allResultItems)
                {
                    var keyphraseList = resultItem.documents.SelectMany(k => k.keyPhrases);
                    Console.WriteLine(" {0}: {1}", resultItem.id, string.Join(",", keyphraseList));
                }
            }

            if (_options.LanguageDetection && result.LanguageAnalysis.AnalysisResult.ActionSubmittedSuccessfully)
            {
                // To be refactored as output in JSON or Table
                Console.WriteLine("Language Analysis: Action submitted successfully:");
                var allResultItems = result.LanguageAnalysis.AnalysisResults.Select(r => r.ResponseData);
                foreach (var resultItem in allResultItems)
                {
                    var languageList = resultItem.documents.SelectMany(k => k.detectedLanguages);
                    Console.WriteLine(" {0}: {1}", resultItem.id, string.Join(",", languageList));
                }
            }

        }


    }
}
