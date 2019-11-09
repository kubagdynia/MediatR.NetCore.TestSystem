using System;

namespace Api.Contracts.V1.Responses
{
    public class IdDto
    {
        public Guid Id { get; private set; }

        public IdDto(Guid id)
        {
            Id = id;
        }
    }
}
