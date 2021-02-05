namespace DashService.WebApi
{
    public class ApiResult<T>
    {
        private readonly T _data;

        public ApiResult(T data)
        {
            _data = data;
        }

        public T Data => _data;
    }
}
