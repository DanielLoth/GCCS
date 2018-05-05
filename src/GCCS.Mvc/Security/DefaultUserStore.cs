using GCCS.Mvc.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace GCCS.Mvc.Security
{
    public class DefaultUserStore :
        IUserStore<ApplicationUser>,
        IUserPasswordStore<ApplicationUser>,
        IUserClaimStore<ApplicationUser>
        //IUserSecurityStampStore<ApplicationUser>
    {
        private readonly DapperUserRepository _identityRepo;

        private bool _disposed = false;

        public DefaultUserStore(DapperUserRepository identityRepo)
        {
            _identityRepo = identityRepo;
        }

        #region Disposal

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public void Dispose()
        {
            // NOTE: The framework seems to invoke this multiple times.
            // So this method has been made idempotent.
            _disposed = true;
        }

        #endregion

        #region IUserStore

        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return _identityRepo.CreateAsync(user, cancellationToken);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return _identityRepo.UpdateAsync(user, cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return _identityRepo.DeleteAsync(user, cancellationToken);
        }

        public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return _identityRepo.FindByIdAsync(userId, cancellationToken);
        }

        public Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return _identityRepo.FindByNameAsync(normalizedUserName, cancellationToken);
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.NormalizedUserName = normalizedName;

            return Task.CompletedTask;
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.UserName = userName;

            return Task.CompletedTask;
        }

        #endregion

        #region IUserPasswordStore

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        #endregion

        #region IUserClaimStore

        public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return _identityRepo.GetClaimsAsync(user, cancellationToken);
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

        #region IUserSecurityStampStore

        //public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken cancellationToken)
        //{
        //    cancellationToken.ThrowIfCancellationRequested();
        //    ThrowIfDisposed();

        //    if (user == null)
        //    {
        //        throw new ArgumentNullException(nameof(user));
        //    }

        //    user.SecurityStamp = stamp ?? throw new ArgumentNullException(nameof(stamp));

        //    return Task.CompletedTask;
        //}

        //public Task<string> GetSecurityStampAsync(ApplicationUser user, CancellationToken cancellationToken)
        //{
        //    cancellationToken.ThrowIfCancellationRequested();
        //    ThrowIfDisposed();

        //    if (user == null)
        //    {
        //        throw new ArgumentNullException(nameof(user));
        //    }

        //    return Task.FromResult(user.SecurityStamp);
        //}

        #endregion
    }
}
