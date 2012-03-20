/* 
 * Credits: http://zamd.net/2011/02/08/using-simple-web-token-swt-with-wif/ 
 * kzu: simplified impl. for netfx
 * 
 */

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel.Security;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Xml;
using Microsoft.IdentityModel;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Thinktecture.IdentityServer.Core.Swt
{
	/// <summary>
	/// Handles SWT tokens.
	/// </summary>
	public class SwtSecurityTokenHandler : SecurityTokenHandler
	{
		public override string[] GetTokenTypeIdentifiers()
		{
			return new[] { "http://schemas.microsoft.com/ws/2010/07/identitymodel/tokens/SWT",
						   "http://schemas.xmlsoap.org/ws/2009/11/swt-token-profile-1.0" };
		}

		public override Type TokenType
		{
			get { return typeof(SimpleWebToken); }
		}

		public override bool CanReadToken(XmlReader reader)
		{
			// since this STS does not accept incoming SWT token, the code path is short circuited
			return false;

			return
				reader.IsStartElement(WSSecurity10Constants.Elements.BinarySecurityToken, WSSecurity10Constants.Namespace) &&
				reader.GetAttribute(WSSecurity10Constants.Attributes.ValueType) == "http://schemas.xmlsoap.org/ws/2009/11/swt-token-profile-1.0";
		}

		public override SecurityToken ReadToken(XmlReader reader)
		{
			if (!this.CanReadToken(reader))
				throw new WebFaultException(System.Net.HttpStatusCode.Unauthorized);

			var swtBuffer = Convert.FromBase64String(reader.ReadElementContentAsString());
			var swt = Encoding.Default.GetString(swtBuffer);

			try
			{
				return new SimpleWebToken(swt);
			}
			catch (InvalidSecurityTokenException)
			{
				throw new WebFaultException(System.Net.HttpStatusCode.Unauthorized);
			}
		}

		public override bool CanValidateToken
		{
			get { return true; }
		}

		public override bool CanWriteToken
		{
			get { return true; }
		}

		public override void WriteToken(XmlWriter writer, SecurityToken token)
		{
			var swt = token as SimpleWebToken;

			if (swt == null)
				throw new InvalidSecurityTokenException();

			// Wrap the token into a binary token for XML transport.
			writer.WriteStartElement(WSSecurity10Constants.Elements.BinarySecurityToken, WSSecurity10Constants.Namespace);
			writer.WriteAttributeString(WSSecurity10Constants.Attributes.ValueType, "http://schemas.xmlsoap.org/ws/2009/11/swt-token-profile-1.0");
			writer.WriteAttributeString(WSSecurity10Constants.Attributes.EncodingType, WSSecurity10Constants.EncodingTypes.Base64);
			writer.WriteValue(Convert.ToBase64String(Encoding.Default.GetBytes(swt.RawToken)));
			writer.WriteEndElement();
		}

		public override ClaimsIdentityCollection ValidateToken(SecurityToken token)
		{
			var swt = token as SimpleWebToken;
			if (swt == null)
				throw new WebFaultException(System.Net.HttpStatusCode.Unauthorized);

			if (base.Configuration.IssuerNameRegistry != null)
			{
				var resolvedIssuer = base.Configuration.IssuerNameRegistry.GetIssuerName(token);
				if (resolvedIssuer != swt.Issuer)
					throw new WebFaultException(System.Net.HttpStatusCode.Unauthorized);
			}

			// If we get this far, it's because the issuer is a trusted one, or we don't 
			// care as we didn't setup an issuerNameRegistry at all. To the resolver 
			// always returns the key.
			var securityKey = base.Configuration.IssuerTokenResolver.ResolveSecurityKey(
				new SwtSecurityKeyClause()) as InMemorySymmetricSecurityKey;

			if (securityKey == null)
				throw new WebFaultException(System.Net.HttpStatusCode.Unauthorized);

			if (!IsHMACValid(swt.RawToken, securityKey.GetSymmetricKey()))
				throw new WebFaultException(System.Net.HttpStatusCode.Unauthorized);

			if (swt.IsExpired)
				throw new WebFaultException(System.Net.HttpStatusCode.Unauthorized);

			if (base.Configuration.AudienceRestriction.AudienceMode != System.IdentityModel.Selectors.AudienceUriMode.Never)
			{
				var allowedAudiences = base.Configuration.AudienceRestriction.AllowedAudienceUris;
				var swtAudienceUri = default(Uri);
				if (!Uri.TryCreate(swt.Audience, UriKind.RelativeOrAbsolute, out swtAudienceUri))
					throw new WebFaultException(System.Net.HttpStatusCode.Unauthorized);

				if (!allowedAudiences.Any(uri => uri == swtAudienceUri))
					throw new WebFaultException(System.Net.HttpStatusCode.Unauthorized);
			}

			var incomingIdentity = swt.ToClaimsIdentity();

			if (base.Configuration.SaveBootstrapTokens)
			{
				incomingIdentity.BootstrapToken = token;
			}

			return new ClaimsIdentityCollection(new IClaimsIdentity[] { incomingIdentity });
		}

		public override SecurityToken CreateToken(SecurityTokenDescriptor tokenDescriptor)
		{
			var sb = new StringBuilder();

			CreateClaims(tokenDescriptor, sb);

			sb.AppendFormat("Issuer={0}&", HttpUtility.UrlEncode(tokenDescriptor.TokenIssuerName));
			sb.AppendFormat("Audience={0}&", HttpUtility.UrlEncode(tokenDescriptor.AppliesToAddress));

			var seconds = (tokenDescriptor.Lifetime.Expires - tokenDescriptor.Lifetime.Created);
			double lifeTimeInSeconds = 3600;
			if (seconds.HasValue)
				lifeTimeInSeconds = seconds.Value.TotalSeconds;

			sb.AppendFormat("ExpiresOn={0:0}", DateTime.UtcNow.ToEpochTime() + lifeTimeInSeconds);

			var unsignedToken = sb.ToString();

			var key = (InMemorySymmetricSecurityKey)tokenDescriptor.SigningCredentials.SigningKey;
			var hmac = new HMACSHA256(key.GetSymmetricKey());
			var sig = hmac.ComputeHash(Encoding.ASCII.GetBytes(unsignedToken));

			var signedToken = String.Format("{0}&HMACSHA256={1}",
				unsignedToken,
				HttpUtility.UrlEncode(Convert.ToBase64String(sig)));

			return new SimpleWebToken(signedToken);
		}

		private static void CreateClaims(SecurityTokenDescriptor tokenDescriptor, StringBuilder sb)
		{
			var claims = new Dictionary<string, string>();

			foreach (var claim in tokenDescriptor.Subject.Claims)
			{
				string value;
				if (claims.TryGetValue(claim.ClaimType, out value))
				{
					claims[claim.ClaimType] = claims[claim.ClaimType] + "," + claim.Value;
				}
				else
				{
					claims.Add(claim.ClaimType, claim.Value);
				}
			}

			foreach (var kv in claims)
			{
				sb.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(kv.Key), HttpUtility.UrlEncode(kv.Value));
			}
		}

		public override SecurityKeyIdentifierClause CreateSecurityTokenReference(SecurityToken token, bool attached)
		{
			var swt = token as SimpleWebToken;
			if (swt == null)
				throw new InvalidSecurityTokenException("Expected SWT token.");

			return new KeyNameIdentifierClause(swt.Issuer);
		}

		private static bool IsHMACValid(string swt, byte[] sha256HMACKey)
		{
			var swtWithSignature = swt.Split(new string[] { String.Format("&{0}=", SwtConstants.HmacSha256) }, StringSplitOptions.None);
			if (swtWithSignature.Length != 2)
				return false;

			using (var hmac = new HMACSHA256(sha256HMACKey))
			{
				var locallyGeneratedSignatureInBytes = hmac.ComputeHash(Encoding.ASCII.GetBytes(swtWithSignature[0]));
				var locallyGeneratedSignature = HttpUtility.UrlEncode(Convert.ToBase64String(locallyGeneratedSignatureInBytes));

				return String.Equals(locallyGeneratedSignature, swtWithSignature[1], StringComparison.InvariantCulture);
			}
		}
	}
}