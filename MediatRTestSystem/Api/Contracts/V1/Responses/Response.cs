using System.Collections.Generic;

namespace Api.Contracts.V1.Responses
{
    public class Response<T>
    {
        public int StatusCode { get; set; }

        public IList<Error>? Errors { get; private set; }

        public T Result { get; set; }

        public Response(T result, int statusCode)
        {
            Result = result;
            StatusCode = statusCode;
        }

        public void AddError(string code, string message, string details, string userMessage)
        {
            if (Errors == null)
            {
                Errors = new List<Error>();
            }

            Errors.Add(new Error
            {
                Code = code,
                Message = message,
                Details = details,
                UserMessage = userMessage
            });
        }
    }

    public class Error
    {
        public string? Message { get; set; }
        public string? Code { get; set; }
        public string? Details { get; set; }
        public string? UserMessage { get; set; }
    }
}
