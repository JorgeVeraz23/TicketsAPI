using Microsoft.EntityFrameworkCore;
using TicketsAPI.Entities;

namespace TicketsAPI
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
            
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Rols { get; set; }
        public DbSet<AnioLectivo> AnioLectivo { get; set; }
        public DbSet<Documento> Documento { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Grado> Grados { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<MateriaParalelo> MateriaParalelos { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<Pago> Pagos { get; set; }  
        public DbSet<Paralelo> Paralelos { get; set; }


        
      


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }

        
    }
}
