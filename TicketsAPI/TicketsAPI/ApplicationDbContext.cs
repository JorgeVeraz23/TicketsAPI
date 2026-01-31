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
        public DbSet<TipoDocumento> TipoDocumentos { get; set; }
        public DbSet<Representante> Representantes { get; set; }

        public DbSet<Profesor> Profesors { get; set; }


        public DbSet<PeriodoEvaluativo> PeriodosEvaluativos { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // ---------- ESTUDIANTE ↔ REPRESENTANTE ----------
            modelBuilder.Entity<Estudiante>()
                .HasOne(e => e.Representante)
                .WithMany(r => r.Estudiantes)
                .HasForeignKey(e => e.RepresentanteId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- PROFESOR ----------
            modelBuilder.Entity<Profesor>()
                .HasIndex(p => new { p.TipoDocumento, p.NumeroDocumento })
                .IsUnique();

            // ---------- REPRESENTANTE ----------
            modelBuilder.Entity<Representante>()
                .HasIndex(r => new { r.TipoDocumento, r.NumeroDocumento })
                .IsUnique();

            // ---------- DOCUMENTO ----------
            modelBuilder.Entity<Documento>()
                .HasOne(d => d.Estudiante)
                .WithMany(e => e.Documentos)
                .HasForeignKey(d => d.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Documento>()
                .HasOne(d => d.TipoDocumento)
                .WithMany()
                .HasForeignKey(d => d.TipoDocumentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- CALIFICACIÓN ----------
            modelBuilder.Entity<Calificacion>()
                .HasOne(c => c.Estudiante)
                .WithMany() // si luego agregas ICollection<Calificacion> en Estudiante, cambia aquí
                .HasForeignKey(c => c.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Calificacion>()
                .HasOne(c => c.Profesor)
                .WithMany(p => p.Calificaciones)
                .HasForeignKey(c => c.ProfesorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Evitar duplicado de calificación por estudiante + materia + periodo
            modelBuilder.Entity<Calificacion>()
                .HasIndex(c => new { c.EstudianteId, c.Materia, c.Periodo })
                .IsUnique();



        }


    }
}
