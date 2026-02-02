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
        public DbSet<Paralelo> Paralelos { get; set; }
        public DbSet<TipoDocumento> TipoDocumentos { get; set; }
        public DbSet<Representante> Representantes { get; set; }

        public DbSet<Profesor> Profesors { get; set; }


        public DbSet<PeriodoEvaluativo> PeriodosEvaluativos { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =============== Defaults / precision =================
           

            modelBuilder.Entity<Calificacion>()
                .Property(x => x.Nota)
                .HasPrecision(5, 2);

            // =============== Unique constraints ===================
            modelBuilder.Entity<Estudiante>()
                .HasIndex(x => x.Cedula)
                .IsUnique();

            modelBuilder.Entity<Representante>()
                .HasIndex(x => x.NumeroDocumento)
                .IsUnique();

            modelBuilder.Entity<Profesor>()
                .HasIndex(x => x.NumeroDocumento)
                .IsUnique();

            modelBuilder.Entity<TipoDocumento>()
                .HasIndex(x => x.Codigo)
                .IsUnique();

            modelBuilder.Entity<GradoParalelo>()
                .HasIndex(x => new { x.GradoId, x.ParaleloId, x.AnioLectivoId })
                .IsUnique();

            // Documento: un tipo por estudiante (si quieres 1 vigente por tipo)
            modelBuilder.Entity<Documento>()
                .HasIndex(x => new { x.EstudianteId, x.TipoDocumentoId })
                .IsUnique();

            // =============== Relationships (delete behavior) =======
            modelBuilder.Entity<Estudiante>()
                .HasOne(x => x.Representante)
                .WithMany(x => x.Estudiantes)
                .HasForeignKey(x => x.RepresentanteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Matricula>()
                .HasOne(x => x.Estudiante)
                .WithMany(x => x.Matriculas)
                .HasForeignKey(x => x.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Matricula>()
                .HasOne(x => x.GradoParalelo)
                .WithMany()
                .HasForeignKey(x => x.GradoParaleloId)
                .OnDelete(DeleteBehavior.Restrict);

    

            modelBuilder.Entity<Documento>()
                .HasOne(x => x.Estudiante)
                .WithMany(x => x.Documentos)
                .HasForeignKey(x => x.EstudianteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Documento>()
                .HasOne(x => x.TipoDocumento)
                .WithMany()
                .HasForeignKey(x => x.TipoDocumentoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Calificacion>()
                .HasOne(x => x.Estudiante)
                .WithMany(x => x.Calificaciones)
                .HasForeignKey(x => x.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Calificacion>()
                .HasOne(x => x.Profesor)
                .WithMany(x => x.Calificaciones)
                .HasForeignKey(x => x.ProfesorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Calificacion>()
                .HasOne(x => x.Materia)
                .WithMany()
                .HasForeignKey(x => x.MateriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Calificacion>()
                .HasOne(x => x.PeriodoEvaluativo)
                .WithMany()
                .HasForeignKey(x => x.PeriodoEvaluativoId)
                .OnDelete(DeleteBehavior.Restrict);

            // =============== Soft Delete filter (opcional) =========
            // Si quieres que todo lo inactivo no salga por default:
            // modelBuilder.Entity<Estudiante>().HasQueryFilter(x => x.IsActive);
            // modelBuilder.Entity<Matricula>().HasQueryFilter(x => x.IsActive);
            // ... (y así con cada entidad)



        }


    }
}
