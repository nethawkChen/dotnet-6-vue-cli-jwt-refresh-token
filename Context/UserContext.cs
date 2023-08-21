using dotnet_6_vue_cli_jwt_refresh_token.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace dotnet_6_vue_cli_jwt_refresh_token.Context {
    public class UserContext: DbContext {
        public UserContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions) { 
        }

        public DbSet<LoginModel>? LoginModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<LoginModel>().HasData(new LoginModel {
                Id = 1,
                UserName = "testuser",
                Password = "123@456"
            });
        }
    }
}
