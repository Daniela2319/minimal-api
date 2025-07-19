using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Infraestrutura.Db
{
    public class DbContexto : DbContext
    {
        private readonly IConfiguration _configuracaoAppSettings;
        public DbContexto(IConfiguration configuracaoAppSettings)
        {
            _configuracaoAppSettings = configuracaoAppSettings;
        }
        public DbSet<Administrador> Administradores { get; set; } = default!;
        public DbSet<Veiculo> Veiculos { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrador>().HasData(
                new Administrador
                {   Id = 1,
                    Email = "dani@test.com",
                    Senha = "senha123",
                    Perfil = "Adm"
                }
            );
        }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var stringConexao = _configuracaoAppSettings.GetConnectionString("postgresql")?.ToString();
            if (!string.IsNullOrEmpty(stringConexao))
            {
                options.UseNpgsql(stringConexao);
            }
        }

    }
}
