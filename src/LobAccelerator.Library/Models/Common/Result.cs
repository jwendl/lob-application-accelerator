using LobAccelerator.Library.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace LobAccelerator.Library.Models.Common
{
    public class Result<T> : IResult
    {
        public T Value { get; set; }
        public bool HasError { get; set; } = false;
        public string Error { get; set; } = string.Empty;
        public string DetailedError { get; set; } = string.Empty;

        bool IResult.HasError()
        {
            return HasError;
        }

        public string GetError()
        {
            return Error;
        }

        public string GetDetailedError()
        {
            return DetailedError;
        }
    }

    public static class Result
    {
        public static IResult Combine(params IResult[] results)
        {
            if (!results.Any())
            {
                return new Result<NoneResult>();
            }

            var failedResults = results.Where(x => x.HasError()).ToList();

            return failedResults.Any()
                ? failedResults.First()
                : results.First();
        }

        public static IResult Combine<T>(List<Result<T>> results)
        {
            if (!results.Any())
            {
                return new Result<NoneResult>();
            }

            var failedResults = results.Where(x => x.HasError).ToList();

            return failedResults.Any()
                ? failedResults.First()
                : results.First();
        }
    }

    public class NoneResult { }
}
