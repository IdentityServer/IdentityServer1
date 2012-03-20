using Microsoft.IdentityModel.Tokens.Saml2;

namespace Thinktecture.IdentityServer.Core.Swt
{
	internal class SwtConstants
	{
		public const string Audience = Saml2Constants.Elements.Audience;
		public const string Issuer = Saml2Constants.Elements.Issuer;
		public const string ExpiresOn = "ExpiresOn";
		public const string HmacSha256 = "HMACSHA256";
	}
}