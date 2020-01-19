using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Responses
{
    public class DataResponse<T>
    {

        public static readonly string OK = "200";
        public static readonly string NOT_FOUND = "404";
        public static readonly string BAD_REQUEST = "400";
        public static readonly string SERVER_ERROR = "500";
        public static readonly string UNAUTHORIZED = "401";
        public DataResponse() {
            Success = true;
            Code = OK;
        }
        public DataResponse(T data) : this()
        {
            this.Data = data;
        }
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }

        public static DataResponse<T> DefaultError(Exception e)
        {
            return new DataResponse<T>() { Code = SERVER_ERROR, Message = e.Message, Success = false };
        }
        public static DataResponse<T> DefaultError(String mess = "Ошибка сервера.")
        {
            return new DataResponse<T>() { Code = BAD_REQUEST, Message = mess, Success = false };
        }
        public static DataResponse<T> UserError(String mess = "Некорректный ввод.")
        {
            return new DataResponse<T>() { Code = BAD_REQUEST, Message = mess, Success = false };
        }

    }
}
