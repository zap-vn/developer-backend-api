using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CRM.Authentication.Infrastructure.Persistence.Repositories
{
    public class PostgresUserRepository : IUserRepository
    {
        private readonly PostgresDbContext _context;

        public PostgresUserRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return null;
            return await _context.Users.FirstOrDefaultAsync(u => u.id == guid);
        }

        public async Task<User?> GetByEmailAndMerchantAsync(string email, string merchantCode)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.email == email || u.username == email);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.email == email || u.username == email);
        }

        public async Task<User?> GetByPhoneAsync(string phone)
        {
            return null;
        }

        public async Task<bool> ExistsAsync(string email, string merchantName)
        {
            return await _context.Users.AnyAsync(u => u.email == email || u.username == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.email == email);
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            // Phone column not found in identity.user screenshot
            return false;
        }

        public async Task<bool> MerchantNameExistsAsync(string merchantName)
        {
            return await Task.FromResult(false);
        }

        public async Task<bool> MerchantUrlExistsAsync(string merchantUrl)
        {
            return await _context.TenantNodes.AnyAsync(t => t.slug == merchantUrl);
        }

        public async Task<TenantNode?> GetTenantBySlugAsync(string slug)
        {
            return await _context.TenantNodes.FirstOrDefaultAsync(t => t.slug == slug);
        }

        public async Task CreateTenantNodeAsync(TenantNode node)
        {
            _context.TenantNodes.Add(node);
            await _context.SaveChangesAsync();
        }

        public async Task<long> GetNextSequenceAsync(string sequenceName)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = $"SELECT nextval('{sequenceName.ToLower()}')";
                if (command.Connection?.State != System.Data.ConnectionState.Open)
                {
                    await _context.Database.OpenConnectionAsync();
                }

                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToInt64(result) : 0;
            }
        }

        public async Task CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }


        private Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? _transaction;

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task DeleteAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return;
            var user = await _context.Users.FindAsync(guid);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
