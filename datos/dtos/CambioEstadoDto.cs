using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PPAI_backend.datos.dtos
{
    public class CambioEstadoDto
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonProperty("FechaHoraInicio")]
        public DateTime FechaHoraInicio { get; set; }
        
        [JsonProperty("FechaHoraFin")]
        public DateTime? FechaHoraFin { get; set; }
        
        [JsonProperty("EstadoId")]
        public string EstadoId { get; set; } = string.Empty;
        
        [JsonProperty("Motivos")]
        public List<int> Motivos { get; set; } = new List<int>();
    }
}