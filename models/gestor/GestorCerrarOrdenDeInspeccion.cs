using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using PPAI_backend.datos.dtos;


namespace PPAI_backend.models.entities
{
    public class GestorCerrarOrdenDeInspeccion
    {
        private Sesion actualSesion = new Sesion
        {
            Usuario = new Usuario()
        };

        private Empleado empleado;

        public Empleado buscarEmpleadoRI()
        {
            empleado = actualSesion.buscarEmpleadoRI();
            return empleado;
        }
        private List<OrdenDeInspeccion> ordenesInspeccion = new List<OrdenDeInspeccion>();
        private List<(MotivoDTO motivo, string comentario)> motivosSeleccionados = new();





        public List<DatosOI> BuscarOrdenInspeccion(Empleado empleado)
        {
            List<DatosOI> resultado = new List<DatosOI>();

            foreach (var oi in ordenesInspeccion)
            {
                if (oi.esDelEmpleado(empleado) && oi.estaRealizada())
                {
                    var nombreEstacion = oi.getDatosEstacion().NombreEstacion;
                    var idSismografo = oi.getDatosEstacion().IdentificadorSismografo;

                    resultado.Add(new DatosOI
                    {
                        Numero = oi.getNumeroOrden(),
                        FechaFin = oi.getFechaFin(), // o fecha cierre según tu diseño
                        NombreEstacion = nombreEstacion,
                        IdSismografo = idSismografo
                    });
                }
            }

            return resultado;
        }
        public List<DatosOI> OrdenarOrdenInspeccion(List<DatosOI> ordenes)
        {
            return ordenes.OrderBy(o => o.FechaFin).ToList();
        }

        private OrdenDeInspeccion ordenSeleccionada;
        public void tomarOrdenSeleccionada(int numeroOrden)
        {
            ordenSeleccionada = ordenesInspeccion.FirstOrDefault(oi => oi.getNumeroOrden() == numeroOrden);

            if (ordenSeleccionada == null)
                throw new Exception($"No se encontró la orden número: {numeroOrden} en la lista mostrada anteriormente.");
        }
        public void tomarObservacion(string observacion)
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay una orden seleccionada para tomar la observación.");
            ordenSeleccionada.ObservacionCierre = observacion;

        }
        public List<MotivoDTO> ObtenerMotivosDesdeJson()
            {
                string jsonPath = "ruta/a/datos.json"; // reemplazá con ruta real
                string json = File.ReadAllText(jsonPath);

                using var doc = JsonDocument.Parse(json);
                var motivosJson = doc.RootElement.GetProperty("motivosFueraServicio");

                var motivos = new List<MotivoDTO>();

                foreach (var m in motivosJson.EnumerateArray())
                {
                    motivos.Add(new MotivoDTO
                    {
                        Id = m.GetProperty("id").GetInt32(),
                        Descripcion = m.GetProperty("descripcion").GetString()!
                    });
                }

                return motivos;
            }
            
        // Este metodo hay que cambiarlo pq ya hay un metodo obtenerMotivo en la clase Motivo
        public void tomarMotivoFueraDeServicio(List<MotivoSeleccionadoConComentarioDTO> seleccionados)
        {
            var todosLosMotivos = ObtenerMotivosDesdeJson();

            foreach (var item in seleccionados)
            {
                var motivo = todosLosMotivos.FirstOrDefault(m => m.Id == item.IdMotivo);
                if (motivo != null)
                {
                    motivosSeleccionados.Add((motivo, item.Comentario));
                }
                else
                {
                    throw new Exception($"Motivo con ID {item.IdMotivo} no encontrado.");
                }
            }
        }

        public string confirmar()
            {
                if (ordenSeleccionada == null)
                    throw new Exception("No hay una orden seleccionada para cerrar.");

                ordenSeleccionada.FechaHoraCierre = DateTime.Now; // Toma la hora de cierre de la orden de inspeccion

                return $"Orden N° {ordenSeleccionada.NumeroOrden} cerrada correctamente.";
            }


    }
}
