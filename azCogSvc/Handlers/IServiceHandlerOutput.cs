using Glav.CognitiveServices.FluentApi.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace azCogSvc.Handlers
{
    public interface IServiceHandlerOutput<TResults> where TResults : IAnalysisResults
    {
        public string ToJson(TResults results);
        public string ToTable(TResults results);
    }
}
