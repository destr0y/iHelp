using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.Data
{
    public class AuthOptions
    {
        public const string ISSUER = "TestAPI"; // издатель токена
        public const string AUDIENCE = "TestAPI Client"; // потребитель токена
        const string KEY = "dsjkfn!q23JNF23#*vzxc";   // ключ для шифрации
        public const int LIFETIME = 30; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
