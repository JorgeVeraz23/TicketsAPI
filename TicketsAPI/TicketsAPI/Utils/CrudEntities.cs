namespace TicketsAPI.Utils
{
    public class CrudEntities
    {
        public string? UsuarioCreacion { get; set; }
        public string? UsuarioModificacion { get; set; }
        public string? UsuarioEliminacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
