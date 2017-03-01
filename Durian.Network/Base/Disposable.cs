using System;
using System.Threading;
using System.Collections.Generic;

namespace Durian.Network
{
	public abstract class Disposable : IDisposable
	{
		private int disposed;

		~Disposable()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		public bool Disposed
		{
			get { return disposed != 0; }
		}

		void Dispose(bool disposing)
		{
			int disposed = Interlocked.CompareExchange(ref this.disposed, 1, 0);
			if (disposed == 0)
			{
				if (disposing)
					DisposeManaged();

				DisposeUnmanaged();

				GC.SuppressFinalize(this);
			}
		}

		protected virtual void DisposeManaged()
		{ }

		protected virtual void DisposeUnmanaged()
		{ }

		protected void CheckDisposed()
		{
			if (Disposed)
				throw new ObjectDisposedException(GetType().Name);
		}

		public static void SafeDispose<T>(ref T obj)
			where T : IDisposable
		{
			if (obj != null)
			{
				obj.Dispose();
				obj = default(T);
			}
		}

		public static bool SafeDisposeReturn<T>(ref T obj)
			where T : IDisposable
		{
			if (obj != null)
			{
				obj.Dispose();
				obj = default(T);

				return true;
			}
			else
				return false;
		}

		public static void SafeDispose<T>(IEnumerable<T> objects)
			where T : IDisposable
		{
			if (objects != null)
			{
				foreach (var obj in objects)
				{
					if (obj != null)
						obj.Dispose();
				}
			}
		}
	}
}