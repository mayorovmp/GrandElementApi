using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Responses
{
    public class ApiResponse
    {

        public static readonly string UNAUTHORIZED = "401";
        public static readonly string OK = "200";
        public static readonly string NOT_FOUND = "404";
        public static readonly string BAD_REQUEST = "400";
        public static readonly string SERVER_ERROR = "500";
        public ApiResponse() {
            Success = true;
            Code = OK;
        }

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
        public static ApiResponse UserError(String mess = "Некорректный ввод.")
        {
            return new ApiResponse() { Code = BAD_REQUEST, Message = mess, Success = false };
        }
    }
}
