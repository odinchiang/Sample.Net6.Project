namespace Sample.Net6.ExceptionService
{
    public class ExceptionInfoService
    {
        public void Show()
        {
            throw new Exception("Service 層的異常...");
        }
    }
}