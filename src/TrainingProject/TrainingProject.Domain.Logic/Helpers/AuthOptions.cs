using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TrainingProject.DomainLogic.Helpers
{
	public class AuthOptions
	{
		public const string ISSUER = "EventObserver";
		public const string AUDIENCE = "User";
		const string KEY = "super_secret_security_key";
		public const int LIFETIME = 60;
		public static SymmetricSecurityKey GetSymmetricSecurityKey()
		{
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
		}
	}
}
