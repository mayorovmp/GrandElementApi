using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Responses
{
    public class DataResponse<T> : ApiResponse
    {
        public T Data { get; set; }
    }
    public class ApiResponse
    {

        public static readonly string UNAUTHORIZED = "401";
        public static readonly string OK = "200";
        public static readonly string NOT_FOUND = "404";
        public static readonly string SERVER_ERROR = "500";
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }

        public static ApiResponse DefaultError(Exception e)
        {
            return new ApiResponse() { Code = SERVER_ERROR, Message = e.Message, Success = false };
        }
        public static ApiResponse DefaultError(String mess = "Ошибка сервера.")
        {
            return new ApiResponse() { Code = SERVER_ERROR, Message = mess, Success = false };
        }
    }
}
