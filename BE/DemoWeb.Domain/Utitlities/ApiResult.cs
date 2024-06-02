
namespace DemoWeb.Domain.Utitlities
{
    public class ApiResult<T> where T : class
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public List<string> Errors { get; set; }
        public ApiResult()
        {

        }
        public ApiResult(T data)
        {
            Data = data;
        }
    }
}
