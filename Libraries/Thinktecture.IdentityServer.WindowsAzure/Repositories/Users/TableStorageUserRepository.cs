using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Claims;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Thinktecture.IdentityServer.TokenService;
using System.ComponentModel.Composition;

namespace Thinktecture.IdentityServer.Repositories.WindowsAzure
{
    public class TableStorageUserRepository : IUserRepository
    {
        CloudStorageAccount _account;
        
        [Import]
        public IClientCertificatesRepository ClientCertificateRepository { get; set; }

        public TableStorageUserRepository()
            : this(TableStorageContext.StorageConnectionString)
        {
            Container.Current.SatisfyImportsOnce(this);
        }

        public TableStorageUserRepository(string storageConnectionString)
        {
            _account = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(storageConnectionString));
        }

        public TableStorageUserRepository(string storageConnectionString, IClientCertificatesRepository repository)
            : this(storageConnectionString)
        {
            ClientCertificateRepository = repository;
        }

        public bool ValidateUser(string userName, string password)
        {
            var context = NewContext;

            // check if user exists
            var user = (from c in context.UserAccounts
                        where c.PartitionKey == userName.ToLowerInvariant() &&
                              c.RowKey == UserAccountEntity.EntityKind
                        select c).FirstOrDefault();

            if (user == null)
            {
                Tracing.Error("User not found in Table Storage: " + userName);
                return false;
            }

            // check password
            var result = new PasswordCrypto().ValidatePassword(password, user.PasswordHash, user.Salt);
            if (result)
            {
                Tracing.Information("Successful sign-in: " + userName);
            }
            else
            {
                Tracing.Error("Invalid password for: " + userName);
            }

            return result;
        }

        public bool ValidateUser(X509Certificate2 clientCertificate, out string userName)
        {
            return ClientCertificateRepository.TryGetUserNameFromThumbprint(clientCertificate, out userName);
        }

        public IEnumerable<string> GetRoles(string userName, RoleTypes roleType)
        {
            if (roleType == RoleTypes.IdentityServer)
            {
                UserAccountEntity account = null;
                if (TryGetUserAccount(userName, out account))
                {
                    if (!string.IsNullOrWhiteSpace(account.InternalRoles))
                    {
                        var roles = account.InternalRoles.Split(',');
                        return new List<string>(from r in roles select r);
                    }

                }
            }

            return new string[] { };
        }

        public IEnumerable<Claim> GetClaims(IClaimsPrincipal principal, RequestDetails requestDetails)
        {
            var claims = from c in NewContext.UserClaims
                         where c.PartitionKey == principal.Identity.Name.ToLower() &&
                               c.Kind == UserClaimEntity.EntityKind
                         select new Claim(c.ClaimType, c.Value);

            return claims.ToList();
        }

        public IEnumerable<string> GetSupportedClaimTypes()
        {
            return new string[] { ClaimTypes.Name, ClaimTypes.Role, ClaimTypes.Email };
        }

        private bool TryGetUserAccount(string userName, out UserAccountEntity account)
        {
            var context = NewContext;

            // check if user exists
            account = (from c in context.UserAccounts
                       where c.PartitionKey == userName.ToLowerInvariant() &&
                             c.RowKey == UserAccountEntity.EntityKind
                       select c).FirstOrDefault();

            return (account != null);
        }

        private TableStorageContext NewContext
        {
            get
            {
                var context = new TableStorageContext(_account.TableEndpoint.AbsoluteUri, _account.Credentials);
                return context;
            }
        }
    }
}