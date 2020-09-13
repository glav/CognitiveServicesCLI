using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace azCogSvc.CommandLine
{
    public class TextAnalyticsOptions : BaseOptions
    {
        public bool SentimentAnalysis { get; set; }
        public bool KeyPhraseAnalysis { get; set; }
        public bool LanguageDetection { get; set; }
        public FileInfo Filename { get; set; }
        public string TextToAnalyse { get; set; }

        public override string ToString()
        {
            return $"-- TextAnalytics --\n--> {base.ToString()}, Filename: {Filename}, TextToAnalyse: {TextToAnalyse}, KeyPhrase: {KeyPhraseAnalysis}, SentimentAnalysis: {SentimentAnalysis}, LanguageDetection:{LanguageDetection}";
        }
    }
}
