using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Product.Infrastructure.Persistence.Repositories
{
    public class PostgresDiningOptionRepository : IDiningOptionRepository
    {
        private readonly PostgresDbContext _context;

        public PostgresDiningOptionRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiningOption>> GetAllAsync(int localeId)
        {
            var query = from do_opt in _context.DiningOptions
                        join dot in _context.DiningOptionTranslations on do_opt.id equals dot.dining_option_id into dotJoin
                        from dotTransl in dotJoin.Where(t => t.locale_id == localeId).DefaultIfEmpty()
                        select new DiningOption
                        {
                            id = do_opt.id,
                            code = do_opt.code,
                            is_active = do_opt.is_active,
                            order_tracking_enabled = do_opt.order_tracking_enabled,
                            DisplayName = dotTransl != null ? dotTransl.name : do_opt.code,
                            UsedInLocations = _context.DiningOptionLocationLinks
                                .Count(l => l.dining_option_id == do_opt.id && l.is_active)
                        };

            return await query.ToListAsync();
        }

        public async Task<DiningOption?> GetByIdAsync(int id, int? localeId)
        {
            var query = from do_opt in _context.DiningOptions
                        where do_opt.id == id
                        join dot in _context.DiningOptionTranslations on do_opt.id equals dot.dining_option_id into dotJoin
                        from dotTransl in dotJoin.Where(t => localeId != null && t.locale_id == localeId).DefaultIfEmpty()
                        select new DiningOption
                        {
                            id = do_opt.id,
                            code = do_opt.code,
                            is_active = do_opt.is_active,
                            order_tracking_enabled = do_opt.order_tracking_enabled,
                            DisplayName = dotTransl != null ? dotTransl.name : do_opt.code,
                            UsedInLocations = _context.DiningOptionLocationLinks
                                .Count(l => l.dining_option_id == do_opt.id && l.is_active)
                        };

            return await query.FirstOrDefaultAsync();
        }

        public async Task CreateAsync(DiningOption entity)
        {
            await _context.DiningOptions.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DiningOption entity)
        {
            _context.DiningOptions.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
