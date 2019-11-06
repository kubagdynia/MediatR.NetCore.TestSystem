namespace Api.Contracts.V1.Responses
{
    public class BaseResponse<T>
    {
        public BaseResponse(T response)
        {
            Data = response;
        }

        public T Data { get; set; }
    }
}
