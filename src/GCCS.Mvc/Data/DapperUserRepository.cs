using Dapper;
using GCCS.Mvc.Data;
using GCCS.Mvc.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace GCCS.Mvc.Data
{
    public class DapperUserRepository :
        IUserStore<ApplicationUser>,
        IUserClaimStore<ApplicationUser>
    {
        public const string ReturnValueParam = "@ReturnValue";

        private static readonly IdentityErrorDescriber ErrorDescriber = new IdentityErrorDescriber();

        private readonly DbConnectionProvider _dbConnectionProvider;

        public DapperUserRepository(DbConnectionProvider dbConProvider)
        {
            _dbConnectionProvider = dbConProvider;
        }

        #region Private helper methods

        private DynamicParameters GetCreateUserParameters(ApplicationUser user)
        {
            var p = new DynamicParameters();

            p.Add("@AccessFailedCount", user.AccessFailedCount, DbType.Int32);
            p.Add("@Email", user.Email, DbType.String);
            p.Add("@EmailConfirmed", user.EmailConfirmed, DbType.Boolean);
            p.Add("@LockoutEnabled", user.LockoutEnabled, DbType.Boolean);
            p.Add("@LockoutEnd", user.LockoutEnd, DbType.DateTimeOffset);
            p.Add("@PasswordHash", user.PasswordHash, DbType.String);
            p.Add("@PhoneNumber", user.PhoneNumber, DbType.String);
            p.Add("@PhoneNumberConfirmed", user.PhoneNumberConfirmed, DbType.Boolean);
            p.Add("@SecurityStamp", user.SecurityStamp, DbType.String);
            p.Add("@TwoFactorEnabled", user.TwoFactorEnabled, DbType.Boolean);
            p.Add("@UserName", user.UserName, DbType.String);
            p.Add(ReturnValueParam, dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            return p;
        }

        private CommandDefinition GetCommandDefinition(
            string sql, object parameters, CancellationToken cancellationToken)
        {
            return new CommandDefinition(sql, parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region IUserStore

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            using (var con = await _dbConnectionProvider.OpenConnectionAsync(cancellationToken))
            {
                var p = GetCreateUserParameters(user);

                try
                {
                    var cmd = GetCommandDefinition("security.sec_CreateUser", p, cancellationToken);

                    await con.ExecuteAsync(cmd);

                    var returnValue = p.Get<int>(ReturnValueParam);

                    if (returnValue == 0)
                    {
                        return IdentityResult.Success;
                    }
                    else
                    {
                        return IdentityResult.Failed(ErrorDescriber.DefaultError());
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == SqlMessageNumbers.DuplicateKey)
                    {
                        if (ex.Message.Contains("UQ_User_UserName"))
                        {
                            return IdentityResult.Failed(ErrorDescriber.DuplicateUserName(user.UserName));
                        }
                    }

                    return IdentityResult.Failed(ErrorDescriber.DefaultError());
                }
                catch
                {
                    return IdentityResult.Failed(ErrorDescriber.DefaultError());
                }
            }
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            try
            {
                //await DeleteAsync(user, cancellationToken);

                return await Task.FromResult(IdentityResult.Success);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (int.TryParse(userId, out int userIdAsInt))
            {
                using (var con = await _dbConnectionProvider.OpenConnectionAsync(cancellationToken))
                {
                    var p = new DynamicParameters();
                    p.Add("@UserId", userIdAsInt, DbType.Int32);

                    var cmd = GetCommandDefinition("security.sec_GetUserByUserId", p, cancellationToken);
                    var user = await con.QueryFirstOrDefaultAsync<ApplicationUser>(cmd);

                    return user;
                }
            }

            return null;
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _dbConnectionProvider.OpenConnectionAsync(cancellationToken))
            {
                var p = new DynamicParameters();
                p.Add("@UserName", normalizedUserName, DbType.String);

                var cmd = GetCommandDefinition("security.sec_GetUserByName", p, cancellationToken);
                var user = await con.QueryFirstOrDefaultAsync<ApplicationUser>(cmd);

                return user;
            }
        }

        #region Unsupported operations

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        #endregion

        #endregion


        #region IUserClaimStore

        private class MyClaim
        {
            public string ClaimType { get; set; }
            public string ClaimValue { get; set; }
        }

        public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            using (var con = await _dbConnectionProvider.OpenConnectionAsync(cancellationToken))
            {
                var p = new DynamicParameters();
                p.Add("@UserId", user.Id, DbType.Int32);

                var cmd = GetCommandDefinition("security.sec_GetClaimsByUserId", p, cancellationToken);
                var myClaims = await con.QueryAsync<MyClaim>(cmd);

                var claims = myClaims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).AsList();

                return claims;
            }
        }

        public Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            // DO NOTHING - This class is intended to be a stateless singleton.
        }

        #endregion
    }
}
