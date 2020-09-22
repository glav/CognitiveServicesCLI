using System;
using System.Collections.Generic;
using System.Text;

namespace azCogSvc.Handlers
{
    public interface IServiceHandlerOutput
    {
        public string ToJson();
        public string ToTable();
    }
}
