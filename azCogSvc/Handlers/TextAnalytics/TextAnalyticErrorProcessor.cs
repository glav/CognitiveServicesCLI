using azCogSvc.CommandLine;
using Glav.CognitiveServices.FluentApi.TextAnalytic;
using Glav.CognitiveServices.FluentApi.TextAnalytic.Domain.ApiResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace azCogSvc.Handlers.TextAnalytics
{
    class TextAnalyticErrorProcessor
    {
        TextAnalyticsOptions _options;
        TextAnalyticAnalysisResults _results;

        public TextAnalyticErrorProcessor(TextAnalyticsOptions options, TextAnalyticAnalysisResults results)
        {
            _options = options;
            _results = results;
        }

        public async Task<bool> WasCallSuccessfulAsync()
        {

            var isOk = await HandleUnsuccessfulCall();
            if (!isOk)
            {
                return await Task.FromResult<bool>(false);
            }

            return await HandleProcessingErrors();

        }

        private async Task<bool> HandleProcessingErrors()
        {
            var errorList = new List<string>();
            if (_options.SentimentAnalysis && _results.SentimentAnalysis.AnalysisResult.ActionSubmittedSuccessfully)
            {
                AddErrorsToList(_results.SentimentAnalysis.GetAllErrors(), errorList, "Sentiment");
            }

            if (_options.KeyPhraseAnalysis && _results.KeyPhraseAnalysis.AnalysisResult.ActionSubmittedSuccessfully)
            {
                AddErrorsToList(_results.KeyPhraseAnalysis.GetAllErrors(), errorList, "Keyphrase");
            }

            if (_options.LanguageDetection && _results.LanguageAnalysis.AnalysisResult.ActionSubmittedSuccessfully)
            {
                AddErrorsToList(_results.LanguageAnalysis.GetAllErrors(), errorList, "Language");
            }

            if (errorList.Count > 0)
            {
                errorList.ForEach(e => Console.Error.WriteLine(e));
                return await Task.FromResult<bool>(false);
            }
            return await Task.FromResult<bool>(true);
        }

        private void AddErrorsToList(IEnumerable<ApiErrorResponse> apiErrors, List<string> errorList, string analysisType)
        {
            var errors = apiErrors.ToList();
            if (errors.Count > 0)
            {
                errorList.Add($"{analysisType} Analysis: Action submitted but contained some errors:");
                errors.ForEach(err => errorList.Add(string.Format(" {0}: {1} -> {2}", err.code, err.message, err.InnerError != null ? $"-> {err.InnerError.code}:{err.InnerError.message}" : string.Empty)));
            }
        }

        private async Task<bool> HandleUnsuccessfulCall()
        {
            var errorList = new List<string>();
            if (!_options.KeyPhraseAnalysis && !_options.SentimentAnalysis && !_options.LanguageDetection)
            {
                Console.Error.WriteLine("ERROR! At least one analysis option must be specified; Sentiment analysis, Keyphrase analysis or language detection ");
                return await Task.FromResult<bool>(false);
            }
            if (_options.SentimentAnalysis && !_results.SentimentAnalysis.AnalysisResult.ActionSubmittedSuccessfully)
            {
                var firstError = _results.SentimentAnalysis.AnalysisResult.ResponseData.errors.First();
                errorList.Add(string.Format("Sentiment Analysis: Unsuccessful. Reason: {0}:{1}", firstError.code, firstError.message, firstError.InnerError != null ? $" -> {firstError.InnerError.code}:{firstError.InnerError.message}" : string.Empty));
            }
            if (_options.KeyPhraseAnalysis && !_results.KeyPhraseAnalysis.AnalysisResult.ActionSubmittedSuccessfully)
            {
                var firstError = _results.SentimentAnalysis.AnalysisResult.ResponseData.errors.First();
                errorList.Add(string.Format("Keyphrase Analysis: Unsuccessful. Reason: {0}:{1}", firstError.code, firstError.message, firstError.InnerError != null ? $" -> {firstError.InnerError.code}:{firstError.InnerError.message}" : string.Empty));
            }
            if (_options.LanguageDetection && !_results.LanguageAnalysis.AnalysisResult.ActionSubmittedSuccessfully)
            {
                var firstError = _results.LanguageAnalysis.AnalysisResult.ResponseData.errors.First();
                errorList.Add(string.Format("Language Analysis: Unsuccessful. Reason: {0}:{1}", firstError.code, firstError.message, firstError.InnerError != null ? $" -> {firstError.InnerError.code}:{firstError.InnerError.message}" : string.Empty));
            }

            if (errorList.Count > 0)
            {
                errorList.ForEach(e => Console.Error.WriteLine(e));
                return await Task.FromResult<bool>(false);
            }

            return await Task.FromResult<bool>(true);
        }
    }
}
