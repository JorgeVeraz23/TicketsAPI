using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.Entities
{
    public class Profesor : CrudEntities
    {
        [Key]
        public long IdProfesor { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string Identificacion { get; set; }
        public string Celular { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public virtual ICollection<Matricula>? Matricula { get; set; }
    }
}
