using Kernel.Serializers;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Kernel
{
    public class BaseResponse
    {
        public int StatusCode { get; set; }
        private IList<Error>? errors { get; set; }
        public IList<Error> Errors => errors ?? (errors = new List<Error>());

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, BaseJsonOptions.GetJsonSerializerOptions);
        }
    }

    public class Error
    {
        public string Message { get; set; }
        public string Code { get; set; }
        public string Details { get; set; }
        public string UserMessage { get; set; }
    }
}
