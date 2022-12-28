using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Domain
{
    public partial class Vuelo
    {
        public int Id { get; set; }

        public string? NumeroVuelo { get; set; }

        public int? CantidadPasajeros { get; set; }

        public string? UbicacionOrigen { get; set; }

        public string? UbicacionDestino { get; set; }

        public int? CantidadEscalas { get; set; }

        public DateTime? FechaVuelo { get; set; }
    }
}
