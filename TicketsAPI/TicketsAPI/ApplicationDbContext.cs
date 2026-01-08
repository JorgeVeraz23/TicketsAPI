using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketsAPI.Entities;

namespace TicketsAPI
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
             : base(options)
        {
        }

   
        public DbSet<AnioLectivo> AnioLectivo { get; set; }
        public DbSet<Documento> Documento { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Grado> Grados { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<GradoParalelo> GradoParalelos { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<Pago> Pagos { get; set; }  
        public DbSet<Paralelo> Paralelos { get; set; }


        
      


   

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }

        
    }
}
