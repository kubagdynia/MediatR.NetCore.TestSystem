namespace Api.Contracts.V1.Responses
{
    public class InvoiceResponse : Response<InvoiceDto>
    {
        public InvoiceResponse(InvoiceDto result, int statusCode) : base(result, statusCode)
        {
        }
    }
}
