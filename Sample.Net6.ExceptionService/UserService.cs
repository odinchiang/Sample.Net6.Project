namespace Sample.Net6.ExceptionService
{
    public class UserService : IUserService
    {
        public bool Validate(string id, string name)
        {
            // 此處連接資料庫取得使用者的資訊，並校驗 name 是否正確
            return true;
        }
    }
}
