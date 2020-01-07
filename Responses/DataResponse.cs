using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Responses
{
    public class DataResponse<T> : ApiResponse
    {
        public DataResponse() { }
        public DataResponse(T data): base()
        {
            Data = data;
        }
        public T Data { get; set; }
    }
}
