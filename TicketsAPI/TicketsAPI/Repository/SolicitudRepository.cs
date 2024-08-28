using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class SolicitudRepository : SolicitudInterface
    {
        private readonly ILogger<SolicitudRepository> _logger;

        private readonly ApplicationDbContext _context;

        public SolicitudRepository(ApplicationDbContext context, ILogger<SolicitudRepository> logger)
        {

            _context = context;
            _logger = logger;
        }
        public async Task<MessageInfoSolicitudDTO> CreateSolicitud(SolicitudDTO solicitud)
        {
            try
            {
                //Verifica si se subio un archivo
                string filePath = null;
                if(solicitud.Justificativo != null)
                {
                    //Guardar el archivo en el servidor
                    filePath = Path.Combine("uploads", solicitud.Justificativo.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await solicitud.Justificativo.CopyToAsync(stream);
                    }
                }
               
                Solicitud solicitudEntity = new Solicitud
                {

                    tipoSolicitud = solicitud.tipoSolicitud,
                    DescripcionSolicitud = solicitud.DescripcionSolicitud,
                    Justificativo = filePath,
                    estadoSolicitud = EnumEstadoSolicitud.Ingresada,
                    DetalleGestion = solicitud.DetalleGestion,
                    FechaIngreso = DateTime.Now,
                    IdUsuario = solicitud.IdUsuario,

                };

                await _context.Solicituds.AddAsync(solicitudEntity);
                await _context.SaveChangesAsync();

                

                return new MessageInfoSolicitudDTO
                {
                    Cod = "201",
                    Mensaje = "Se ha registrado la solicitud",
                };
            }catch(Exception ex) {
                _logger.LogError(ex, "Error al crear la solicitud con tipo {tipoSolicitud}" + solicitud.tipoSolicitud);
                return new MessageInfoSolicitudDTO { Mensaje  = "Error al intentar crear la solicitud", Cod = "500"};
            }
        }

        public async Task<List<MostrarSolicitudDTO>> GetAllSolicitudes(long idUsuario)
        {
            try
            {
                var solicitudesUsuario = await _context.Solicituds.Where(x => x.IdUsuario == idUsuario).Select(c => new MostrarSolicitudDTO
                {
                    IdSolicitud = c.IdSolicitud,
                    IdUsuario = c.IdUsuario,
                    DescripcionSolicitud = c.DescripcionSolicitud,
                    Justificativo = c.Justificativo,
                    tipoSolicitud = c.tipoSolicitud,
                    DetalleGestion = c.DetalleGestion,
                    FechaIngreso = c.FechaIngreso,
                }).ToListAsync() ;

                //Retorna la lista de solicitudes, puede estar vacia si no hay resultados
                return solicitudesUsuario;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitudes para el usuario con Id {IdUsuario}", idUsuario);
                return new List<MostrarSolicitudDTO>();
            }
        }

        public async Task<List<MostrarSolicitudDTO>> GetAllSolicitudesByFilterCliente(long idUsuario, DateTime FechaIngreso, EnumEstadoSolicitud Estado)
        {
            try
            {
                var filtro = await _context.Solicituds.Where(x => x.IdUsuario == idUsuario && x.FechaIngreso == FechaIngreso && x.estadoSolicitud == Estado).Select(c => new MostrarSolicitudDTO
                {
                    IdSolicitud = c.IdSolicitud,
                    IdUsuario = c.IdUsuario,
                    tipoSolicitud = c.tipoSolicitud,
                    DescripcionSolicitud = c.DescripcionSolicitud,
                    Justificativo = c.Justificativo,
                    FechaIngreso = c.FechaIngreso,
                    DetalleGestion = c.DetalleGestion,
                }).ToListAsync();

                return filtro;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitudes para el usuario con Id {IdUsuario}", idUsuario);
                return new List<MostrarSolicitudDTO>();
            }
        }

        public async Task<List<MostrarSolicitudDTO>> GetAllSolicitudesAdministrador()
        {
            try
            {
                var solicitudes = await _context.Solicituds.Select(c => new MostrarSolicitudDTO
                {
                    IdSolicitud = c.IdSolicitud,
                    IdUsuario = c.IdUsuario,
                    tipoSolicitud = c.tipoSolicitud,
                    DescripcionSolicitud = c.DescripcionSolicitud,
                    Justificativo = c.Justificativo,
                    FechaIngreso = c.FechaIngreso,
                    DetalleGestion = c.DetalleGestion

                } ).ToListAsync();

                return solicitudes;


            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitudes para el usuario con Id {IdUsuario}", 400);
                return new List<MostrarSolicitudDTO>();
            }
        }

        //Enpoint de obtener solicitudes mediante filtro de administrador
        public async Task<List<MostrarSolicitudAdministradorDTO>> GetAllSolitudesByFilter(long idUsuario, DateTime fechaIngreso)
        {
            try
            {

                var filtro = await _context.Solicituds.Where(x => x.IdUsuario == idUsuario && x.FechaIngreso.Date == fechaIngreso.Date).Select(c => new MostrarSolicitudAdministradorDTO
                {
                    IdSolicitud = c.IdSolicitud,
                    IdUsuario = c.IdUsuario,
                    tipoSolicitud = c.tipoSolicitud,
                    DescripcionSolicitud = c.DescripcionSolicitud,
                    Justificativo = c.Justificativo,
                    DetalleGestion = c.DetalleGestion,
                    FechaIngreso = c.FechaIngreso,
                    FechaActualizacion = c.FechaActualizacion,
                    FechaGestion = c.FechaGestion,
                    
                }).ToListAsync();

                return filtro;

            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitudes para el usuario con Id {IdUsuario}", 400);
                return new List<MostrarSolicitudAdministradorDTO>();
            }
        }

        public async Task<List<MostrarJustificativoDTO>> GetJustificativo(long idSolicitud)
        {
            var justificacion = await _context.Solicituds.Where(x =>  x.IdSolicitud == idSolicitud).Select(c => new MostrarJustificativoDTO
            {
                Justificativo = c.Justificativo
            }).ToListAsync();

            return justificacion;
        }

        public async Task<MostrarSolicitudAdministradorDTO> VerDetalleSolicitud(long idSolicitud)
        {
            var solicitudoIngresada = await _context.Solicituds.Where(x => x.IdSolicitud == idSolicitud).Select(c => new MostrarSolicitudAdministradorDTO
            {
                IdSolicitud = c.IdSolicitud,
                IdUsuario = c.IdUsuario,
                FechaIngreso = c.FechaIngreso,
                DescripcionSolicitud = c.DescripcionSolicitud,
                Justificativo = c.Justificativo,
                DetalleGestion = c.DetalleGestion,
                estadoSolicitud = c.estadoSolicitud,
                FechaActualizacion = c.FechaActualizacion,
                FechaGestion = c.FechaGestion,
                tipoSolicitud = c.tipoSolicitud,
            }).FirstOrDefaultAsync() ?? new MostrarSolicitudAdministradorDTO { };

            return solicitudoIngresada;
        }

        public async Task<MessageInfoSolicitudDTO> ActualizarSolicitud(ActualizarSolicitudDTO solicitud)
        {
            try
            {
                var solicitudToUpdate = await _context.Solicituds.Where(x => x.IdSolicitud == solicitud.IdSolicitud).FirstOrDefaultAsync();

                if(solicitudToUpdate == null)
                {
                    return new MessageInfoSolicitudDTO
                    {
                        Cod = "404",
                        Mensaje = "Not Found"
                    };
                }

                //Actualiza los valores de la solicitud existente
                solicitudToUpdate.estadoSolicitud = solicitud.estadoSolicitud;
                solicitudToUpdate.FechaGestion = solicitud.FechaGestion;
                solicitudToUpdate.DetalleGestion = solicitud.DetalleGestion;
                solicitudToUpdate.FechaActualizacion = solicitud.FechaActualizacion;


                //Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                return new MessageInfoSolicitudDTO
                {
                    Cod = "201",
                    Mensaje = "Solicitud Gestionada exitosamente"
                };


            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error al crear la solicitud con tipo {tipoSolicitud}" + solicitud.IdSolicitud);
                return new MessageInfoSolicitudDTO { Mensaje = "Error al intentar crear la solicitud", Cod = "500" };
            }
        }
    }
}
