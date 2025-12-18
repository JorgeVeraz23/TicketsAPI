using System.ComponentModel.DataAnnotations;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class Estudiante : CrudEntities
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
        [Required]
        [MaxLength(100)]
        public string Cedula { get; set; }
        [Required]
        [MaxLength(100)]
        public string Representante { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        [Required]
        public int Nivel { get; set; }
        public virtual ICollection<Documento> Documentos {  get; set; }
        public virtual ICollection<Matricula> Matriculas { get; set; }


    }
}
