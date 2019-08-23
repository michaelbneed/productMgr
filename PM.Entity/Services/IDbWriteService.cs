using System.Threading.Tasks;

namespace PM.Entity.Services
{
	public interface IDbWriteService
	{
		Task<bool> SaveChangesAsync();

		Task<bool> SaveChanges();

		void Add<TEntity>(TEntity item) where TEntity : class;

		void Delete<TEntity>(TEntity item) where TEntity : class;

		void Update<TEntity>(TEntity item) where TEntity : class;
	}
}