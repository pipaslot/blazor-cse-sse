using Pipaslot.Mediator.Abstractions;
using System.Collections.Generic;

namespace App.Shared.App
{
    public static class LanguageResourceRequest
    {
        public class Query : IRequest<Result>
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
