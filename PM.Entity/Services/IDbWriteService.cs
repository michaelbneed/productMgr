using System.Threading.Tasks;

namespace PM.Entity.Services
{
	public interface IDbWriteService
	{
		Task<bool> SaveChangesAsync();

		void Add<TEntity>(TEntity item) where TEntity : class;

		void Delete<TEntity>(TEntity item) where TEntity : class;

		void Update<TEntity>(TEntity item) where TEntity : class;
	}
}