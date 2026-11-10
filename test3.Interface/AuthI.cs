using test3.DTO.Common;

namespace test3.Interface
{
    public interface AuthI
    {
        (Boolean status, String? token, String message) Auth(String UAcc, String UPwd);
    }

    public interface LoginI
    {
        String Login(LoginRes Res);
    }
}