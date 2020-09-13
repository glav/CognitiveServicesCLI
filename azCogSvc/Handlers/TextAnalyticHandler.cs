using azCogSvc.CommandLine;
using Glav.CognitiveServices.FluentApi.Core;
using Glav.CognitiveServices.FluentApi.TextAnalytic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public async Task<bool> ExecuteAsync()
        {
            if (!_options.KeyPhraseAnalysis && !_options.SentimentAnalysis && !_options.LanguageDetection)
            {
                Console.WriteLine("ERROR! At least one analysis option must be specified; Sentiment analysis, Keyphrase analysis or language detection ");
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

            //Console.WriteLine(_options);
            ProcessResults(result); // Note: Not the optimal to return results as we want to output to multiple formats but will do for now.

            return await Task.FromResult<bool>(true);
        }

        private void ProcessResults(TextAnalyticAnalysisResults result)
        {
            if (_options.SentimentAnalysis && result.SentimentAnalysis.AnalysisResult.ActionSubmittedSuccessfully)
            {
                var errors = result.SentimentAnalysis.AnalysisResults
                    .Where(r => r.ResponseData.errors != null && r.ResponseData.errors.Length > 0)
                    .SelectMany(s => s.ResponseData.errors)
                    .ToList();
                if (errors.Count > 0)
                {
                    Console.WriteLine("Sentiment Analysis: Action submitted but contained some errors:");
                    foreach (var err in errors)
                    {
                        Console.WriteLine(" {0}: {1} -> {2}", err.code, err.message, err.InnerError != null ? $"-> {err.InnerError.code}:{err.InnerError.message}" : string.Empty);
                    }
                }
                else
                {
                    Console.WriteLine("Sentiment Analysis: Action submitted successfully:");
                    var allResultItems = result.SentimentAnalysis.GetResults();
                    foreach (var resultItem in allResultItems)
                    {
                        Console.WriteLine(" {0}: {1} ({2})", resultItem.id, resultItem.score, result.SentimentAnalysis.ScoringEngine.EvaluateScore(resultItem.score).Name);
                    }
                }
            }
            else if (_options.SentimentAnalysis)
            {
                var firstError = result.SentimentAnalysis.AnalysisResult.ResponseData.errors.First();
                Console.WriteLine("Sentiment Analysis: Unsuccessful. Reason: {0}:{1}", firstError.code, firstError.message, firstError.InnerError != null ? $" -> {firstError.InnerError.code}:{firstError.InnerError.message}" : string.Empty);
            }

            if (_options.KeyPhraseAnalysis && result.KeyPhraseAnalysis.AnalysisResult.ActionSubmittedSuccessfully)
            {
                var errors = result.KeyPhraseAnalysis.AnalysisResults
                    .Where(r => r.ResponseData.errors != null && r.ResponseData.errors.Length > 0)
                    .SelectMany(s => s.ResponseData.errors)
                    .ToList();
                if (errors.Count > 0)
                {
                    Console.WriteLine("Keyphrase Analysis: Action submitted but contained some errors:");
                    foreach (var err in errors)
                    {
                        Console.WriteLine(" {0}: {1} -> {2}", err.code, err.message, err.InnerError != null ? $"-> {err.InnerError.code}:{err.InnerError.message}" : string.Empty);
                    }
                }
                else
                {
                    Console.WriteLine("Keyphrase Analysis: Action submitted successfully:");
                    var allResultItems = result.KeyPhraseAnalysis.AnalysisResults.Select(r => r.ResponseData);
                    foreach (var resultItem in allResultItems)
                    {
                        var keyphraseList = resultItem.documents.SelectMany(k => k.keyPhrases);
                        Console.WriteLine(" {0}: {1}", resultItem.id, string.Join(",", keyphraseList));
                    }
                }
            }
            else if (_options.KeyPhraseAnalysis)
            {
                var firstError = result.SentimentAnalysis.AnalysisResult.ResponseData.errors.First();
                Console.WriteLine("Keyphrase Analysis: Unsuccessful. Reason: {0}:{1}", firstError.code, firstError.message, firstError.InnerError != null ? $" -> {firstError.InnerError.code}:{firstError.InnerError.message}" : string.Empty);
            }
        }

    }
}
