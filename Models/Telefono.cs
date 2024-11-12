using System.ComponentModel.DataAnnotations;

namespace CFAGestionClientes.Models
{
public class Telefono
{
    public int Id { get; set; }
    public int ClienteCodigo { get; set; }
    public long NumeroTelefono { get; set; }
}
}
