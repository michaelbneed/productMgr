using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PM.DatabaseOperations.Models;

namespace PM.DatabaseOperations.Services
{
	public class DbReadService : IDbReadService
	{
		private readonly VandivierProductManagerContext _db;

		public DbReadService(VandivierProductManagerContext db)
		{
			this._db = db;
		}

		public async Task<List<TEntity>> GetAllRecordsAsync<TEntity>() where TEntity : class
		{
			return await _db.Set<TEntity>().ToListAsync();
		}

		public async Task<List<TEntity>> GetAllRecordsAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
		{
			return await _db.Set<TEntity>().Where(expression).ToListAsync();
		}

		public async Task<TEntity> GetSingleRecordAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
		{
			return await _db.Set<TEntity>().FirstOrDefaultAsync(expression);
		}

		public async Task<bool> DoesRecordExist<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
		{
			return await _db.Set<TEntity>().AnyAsync(expression);
		}

		public void IncludeEntityNavigation<TEntity>() where TEntity : class
		{
			var propertyNames = _db.Model
				.FindEntityType(typeof(TEntity))
				.GetNavigations()
				.Select(e => e.Name);
			foreach (var name in propertyNames) _db.Set<TEntity>().Include(name).Load();
		}

		public void IncludeEntityNavigation<TEntity1, TEntity2>() where TEntity1 : class where TEntity2 : class
		{
			IncludeEntityNavigation<TEntity1>();
			IncludeEntityNavigation<TEntity2>();
		}
	}
}
