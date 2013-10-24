using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Raven.Client;

namespace RavenDB.AspNet.Identity
{
	public class UserStore<TUser> : IUserStore<TUser>, IUserLoginStore<TUser>, IUserClaimStore<TUser>, IUserRoleStore<TUser>,
		IUserPasswordStore<TUser>, IUserSecurityStampStore<TUser>
		where TUser : IdentityUser
	{
		private bool _disposed;
		private Func<IDocumentSession> getSessionFunc;
		private IDocumentSession _session;

		private IDocumentSession session
		{
			get
			{
				if (_session == null)
					_session = getSessionFunc();
				return _session;
			}
		}

		public UserStore(Func<IDocumentSession> getSession)
		{
			this.getSessionFunc = getSession;
		}

		public UserStore(IDocumentSession session)
		{
			this._session = session;
		}

		public Task CreateAsync(TUser user)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			this.session.Store(user);
			return Task.FromResult(true);
		}

		public Task DeleteAsync(TUser user)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			this.session.Delete(user);
			return Task.FromResult(true);
		}

		public Task<TUser> FindByIdAsync(string userId)
		{
			var user = this.session.Load<TUser>(userId);
			return Task.FromResult(user);
		}

		public Task<TUser> FindByNameAsync(string userName)
		{
			var user = this.session.Query<TUser>()
				.FirstOrDefault(u => u.UserName == userName);

			return Task.FromResult(user);
		}

		public Task UpdateAsync(TUser user)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			return Task.FromResult(true);
		}

		private void ThrowIfDisposed()
		{
			if (this._disposed)
				throw new ObjectDisposedException(this.GetType().Name);
		}

		public void Dispose()
		{
			this._disposed = true;
		}

		public Task AddLoginAsync(TUser user, UserLoginInfo login)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			if (!user.Logins.Any(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey))
			{
				user.Logins.Add(login);

				this.session.Store(new IdentityUserLogin
				{
					Id = Util.GetLoginId(login),
					UserId = user.Id,
					Provider = login.LoginProvider,
					ProviderKey = login.ProviderKey
				});
			}

			return Task.FromResult(true);
		}

		public Task<TUser> FindAsync(UserLoginInfo login)
		{
			string loginId = Util.GetLoginId(login);

			var loginDoc = session.Include<IdentityUserLogin>(x => x.UserId)
				.Load(loginId);

			TUser user = null;

			if (loginDoc != null)
				user = this.session.Load<TUser>(loginDoc.UserId);

			return Task.FromResult(user);
		}

		public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			return Task.FromResult(user.Logins.ToIList());
		}

		public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			string loginId = Util.GetLoginId(login);
			var loginDoc = this.session.Load<IdentityUserLogin>(loginId);
			if (loginDoc != null)
				this.session.Delete(loginDoc);

			user.Logins.RemoveAll(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);
			
			return Task.FromResult(0);
		}

		public Task AddClaimAsync(TUser user, Claim claim)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			if (!user.Claims.Any(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value))
			{
				user.Claims.Add(new IdentityUserClaim
				{
					ClaimType = claim.Type,
					ClaimValue = claim.Value
				});
			}
			return Task.FromResult(0);
		}

		public Task<IList<Claim>> GetClaimsAsync(TUser user)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			IList<Claim> result = user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
			return Task.FromResult(result);
		}

		public Task RemoveClaimAsync(TUser user, Claim claim)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			user.Claims.RemoveAll(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
			return Task.FromResult(0);
		}

		public Task<string> GetPasswordHashAsync(TUser user)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			return Task.FromResult(user.PasswordHash);
		}

		public Task<bool> HasPasswordAsync(TUser user)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			return Task.FromResult<bool>(user.PasswordHash != null);
		}

		public Task SetPasswordHashAsync(TUser user, string passwordHash)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			user.PasswordHash = passwordHash;
			return Task.FromResult(0);
		}

		public Task<string> GetSecurityStampAsync(TUser user)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");
			
			return Task.FromResult(user.SecurityStamp);
		}

		public Task SetSecurityStampAsync(TUser user, string stamp)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			user.SecurityStamp = stamp;
			return Task.FromResult(0);
		}

		public Task AddToRoleAsync(TUser user, string role)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			if (!user.Roles.Contains(role, StringComparer.InvariantCultureIgnoreCase))
				user.Roles.Add(role);

			return Task.FromResult(0);
		}

		public Task<IList<string>> GetRolesAsync(TUser user)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			return Task.FromResult<IList<string>>(user.Roles);
		}

		public Task<bool> IsInRoleAsync(TUser user, string role)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			return Task.FromResult(user.Roles.Contains(role, StringComparer.InvariantCultureIgnoreCase));
		}

		public Task RemoveFromRoleAsync(TUser user, string role)
		{
			this.ThrowIfDisposed();
			if (user == null)
				throw new ArgumentNullException("user");

			user.Roles.RemoveAll(r => String.Equals(r, role, StringComparison.InvariantCultureIgnoreCase));

			return Task.FromResult(0);
		}
	}
}
