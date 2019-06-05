using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PM.DatabaseOperations.Services
{
	public interface IDbReadService
	{
		Task<List<TEntity>> GetAllRecordsAsync<TEntity>() where TEntity : class;

		Task<List<TEntity>> GetAllRecordsAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;

		Task<TEntity> GetSingleRecordAsync<TEntity>(Expression<Func<TEntity, bool>>  expression) where TEntity : class;

		Task<bool> DoesRecordExist<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;

		void IncludeEntityNavigation<TEntity>() where TEntity : class;

		void IncludeEntityNavigation<TEntity1, TEntity2>() where TEntity1 : class where TEntity2 : class;
	}
}