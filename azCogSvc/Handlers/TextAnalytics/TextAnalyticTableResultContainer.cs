﻿using azCogSvc.CommandLine;
using Glav.CognitiveServices.FluentApi.TextAnalytic;
using Glav.CognitiveServices.FluentApi.TextAnalytic.Domain.ApiResponses;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace azCogSvc.Handlers.TextAnalytics
{
    public class TextAnalyticTableResultContainer
    {
        TextAnalyticsOptions _options;
        TextAnalyticAnalysisResults _results;

        public TextAnalyticTableResultContainer(TextAnalyticsOptions options, TextAnalyticAnalysisResults results)
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

        public override string ToString()
        {
            var output = new StringBuilder();

            if (_options.SentimentAnalysis)
            {
                output.AppendLine(">> Sentiment Analysis <<");
                _results.SentimentAnalysis.AnalysisResults.ForEach(r =>
                {
                    output.AppendFormat("DocumentSetId\n{0}\n", r.ResponseData.id);
                    output.AppendLine("\tId\tScore\t\t\tSentiment");
                    r.ResponseData.documents.ToList().ForEach(d =>
                    {
                        output.AppendFormat("\t{0}\t{1:0.0000000000000000}\t{2}", d.id, d.score, "-not done-\n");
                    });
                });

            }
            if (_options.KeyPhraseAnalysis)
            {
                KeyphraseResults = _results.KeyPhraseAnalysis.AnalysisResults.Select(r => r.ResponseData);
            }
            if (_options.LanguageDetection)
            {
                LanguageResults = _results.LanguageAnalysis.AnalysisResults.Select(r => r.ResponseData);
            }

            return output.ToString();
        }
    }
}