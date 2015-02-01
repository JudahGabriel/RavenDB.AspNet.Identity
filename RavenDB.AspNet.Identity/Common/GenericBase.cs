using System;
using System.Runtime.CompilerServices;

namespace RavenDB.AspNet.Identity.Common
{
    public abstract class GenericBase<TKey> : IDisposable
        //where TKey : IConvertible, IComparable, IEquatable<TKey>
    {

        protected GenericBase()
        {
            //check für Valid Key
            if (!(CheckInt() || CheckString()))
            {
                ThrowTypeAccessException(typeof(TKey));
            }
        }

        protected static void ThrowTypeAccessException(Type type)
        {
            throw new TypeAccessException("Only 'int' and 'string' are valid for RavenDB Id!", new TypeAccessException(type.FullName));
        }

        protected static bool CheckInt()
        {
            return typeof(TKey) == typeof(int);
        }

        protected static bool CheckString()
        {
            return typeof(TKey) == typeof(string);
        }

        protected void CheckArgumentForOnlyNull(object input, string argumentName, [CallerMemberName]string sourceMemberName = "")
        {
            this.ThrowIfDisposed();
            if (input == null)
                throw new ArgumentNullException(argumentName, sourceMemberName);
        }

        protected void CheckArgumentForNull(object input, string argumentName, [CallerMemberName]string sourceMemberName = "")
        {
            this.ThrowIfDisposed();
            if (input is string)
            {
                if (string.IsNullOrWhiteSpace(input as string))
                    throw new ArgumentNullException(argumentName, sourceMemberName);
            }
            else if (input is int)
            {
                if (Convert.ToInt32(input) == default(int))
                    throw new ArgumentNullException(argumentName, sourceMemberName);
            }
            else
            {
                if (input == null)
                    throw new ArgumentNullException(argumentName, sourceMemberName);
            }
        }

        #region IDisposable

        protected Action HandleDisposable;
        protected bool Disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    try
                    {
                        if (HandleDisposable != null)
                        {
                            HandleDisposable.Invoke();
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GenericBase()
        {
            Dispose(false);
        }

        protected void ThrowIfDisposed()
        {
            if (this.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        #endregion
    }
}