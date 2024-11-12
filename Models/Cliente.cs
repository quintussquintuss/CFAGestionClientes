using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CFAGestionClientes.Models
{
    public class Cliente
    {
        [Key]
        public int Codigo { get; set; } // Este campo es autoincremental
        public string TipoDocumento { get; set; } = string.Empty;
        public long NumeroDocumento { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellido1 { get; set; } = string.Empty;
        public string Apellido2 { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
        public ICollection<Telefono> Telefonos { get; set; } = new List<Telefono>();
    }
}