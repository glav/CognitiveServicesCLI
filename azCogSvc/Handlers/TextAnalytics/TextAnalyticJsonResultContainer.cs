using azCogSvc.CommandLine;
using Glav.CognitiveServices.FluentApi.TextAnalytic;
using Glav.CognitiveServices.FluentApi.TextAnalytic.Domain.ApiResponses;
using System.Collections.Generic;
using System.Linq;

namespace azCogSvc.Handlers.TextAnalytics
{
    public class TextAnalyticJsonResultContainer
    {
        TextAnalyticsOptions _options;
        TextAnalyticAnalysisResults _results;

        public TextAnalyticJsonResultContainer(TextAnalyticsOptions options,TextAnalyticAnalysisResults results)
        {
            _options = options;
            _results = results;

            SetDataInContainerProperties();
        }

        private void SetDataInContainerProperties()
        {
            if (_options.SentimentAnalysis)
            {
                SentimentResults = _results.SentimentAnalysis.AnalysisResults.Select(r => r.ResponseData);
            }
            if (_options.KeyPhraseAnalysis)
            {
                KeyphraseResults = _results.KeyPhraseAnalysis.AnalysisResults.Select(r => r.ResponseData);
            }
            if (_options.LanguageDetection)
            {
                LanguageResults = _results.LanguageAnalysis.AnalysisResults.Select(r => r.ResponseData);
            }

        }

        public IEnumerable<SentimentResultResponseRoot> SentimentResults { get; internal set; }
        public IEnumerable<KeyPhraseResultResponseRoot> KeyphraseResults { get; internal set; }
        public IEnumerable<LanguagesResultResponseRoot> LanguageResults { get; internal set; }
    }
}
