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
        public DbSet<Solicitud> Solicituds { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FilledForm> FilledForms { get; set; }
        public DbSet<FilledFormField> FilledFormField { get; set; }
        public DbSet<FormGroup> FormGroups { get; set; }
        public DbSet<FieldType> FieldTypes { get; set; }
        public DbSet<FormField> FormFields { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }

        
    }
}
