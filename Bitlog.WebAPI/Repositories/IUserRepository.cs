using Bitlog.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bitlog.WebAPI.Repositories
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasKey(x => x.Id);
        }
    }

    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<User> Search(string email, string phoneNumber);
    }

    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUser(User user)
        {
            var userSet = _context.Users;
            await userSet.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Search(string email, string phoneNumber)
        {
            var userSet = _context.Users;
            return await userSet
                .Where(x => x.Email == email && x.PhoneNumber == phoneNumber)
                .FirstOrDefaultAsync();
        }
    }
}
