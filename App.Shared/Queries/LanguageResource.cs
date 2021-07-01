using App.Shared.CQRSAbstraction;
using System.Collections.Generic;

namespace App.Shared.Queries
{
    public static class LanguageResource
    {
        public class Query : IQuery<Result>
        {
            public string Language { get; set; }
            public string TypeName { get; set; }
        }

        public class Result
        {
            public Dictionary<string, string> Resource { get; init; }
        }
    }
}
