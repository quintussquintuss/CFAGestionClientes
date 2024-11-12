using System.ComponentModel.DataAnnotations;
namespace CFAGestionClientes.Models
{
public class Direccion
{
    public int Id { get; set; } 
    public int ClienteCodigo { get; set; }
    public string DireccionCompleta { get; set; }  = string.Empty;
}

}
