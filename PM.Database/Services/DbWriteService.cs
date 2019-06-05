using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PM.DatabaseOperations.Contexts;

namespace PM.DatabaseOperations.Services
{
	public class DbWriteService : IDbWriteService
	{
		private readonly Context _db;
		
		public DbWriteService(Context db)
		{
			this._db = db;
		}

		public async Task<bool> SaveChangesAsync()
		{
			try
			{
				return await _db.SaveChangesAsync() >= 0;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		public void Add<TEntity>(TEntity item) where TEntity : class
		{
			try
			{
				_db.Add(item);
			}
			catch (Exception e)
			{
				throw;
			}
		}

		public void Delete<TEntity>(TEntity item) where TEntity : class
		{
			try
			{
				_db.Set<TEntity>().Remove(item);
			}
			catch (Exception e)
			{
				throw;
			}
		}

		public void Update<TEntity>(TEntity item) where TEntity : class
		{
			try
			{
				var ent = _db.Find<TEntity>(item.GetType().GetProperty("Id").GetValue(item));
				if (ent != null)
				{
					_db.Entry(ent).State = EntityState.Detached;
				}

				_db.Set<TEntity>().Update(item);
			}
			catch (Exception e)
			{
				throw;
			}
		}
	}
}
