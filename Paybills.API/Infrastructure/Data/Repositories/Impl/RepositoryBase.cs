using System.Threading.Tasks;

namespace Paybills.API.Infrastructure.Data.Repositories.Impl
{
    public class RepositoryBase
    {
        protected readonly DataContext _context;
        public RepositoryBase(DataContext context)
        {
            this._context = context;
        }

        public async Task<bool> SaveAllAsync() => await _context.SaveChangesAsync() > 0;
    }
}