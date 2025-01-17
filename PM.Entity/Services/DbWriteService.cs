﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PM.Entity.Models;

namespace PM.Entity.Services
{
	public class DbWriteService : IDbWriteService
	{
		private readonly VandivierProductManagerContext _db;
		
		public DbWriteService(VandivierProductManagerContext db)
		{
			this._db = db;
		}

		public async Task<bool> SaveChangesAsync()
		{
			var attempts = 0;
			do
			{
				try
				{
					attempts++;
					return await _db.SaveChangesAsync() >= 0;
					break; 
				}
				catch (Exception ex)
				{
					if (attempts == 3)
						throw;

					Task.Delay(1000).Wait();
				}
			} while (true);

		}

		public Task<bool> SaveChanges()
		{
			var attempts = 0;
			do
			{
				try
				{
					attempts++;
					return Task.FromResult(_db.SaveChanges() >= 0);
					break;
				}
				catch (Exception ex)
				{
					if (attempts == 3)
						throw;

					Task.Delay(1000).Wait();
				}
			} while (true);

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
