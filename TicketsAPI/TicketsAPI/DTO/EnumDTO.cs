namespace TicketsAPI.DTO
{
    public enum EnumRolUsuarioDTO
    {
        Cliente,
        Administrador
    }

    public enum EnumTipoSolicitud
    {
        Reclamo, 
        Consulta
    }

    public enum EnumEstadoSolicitud
    {
        Ingresada,
        EnAtencion,
        Atendida,
        NoResuelta
    }
}
