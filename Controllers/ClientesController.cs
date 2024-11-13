using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CFAGestionClientes.Data;
using CFAGestionClientes.Models;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace CFAGestionClientes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]

    public class ClientesController : ControllerBase
    {
        private readonly CFAContext _context;

        public ClientesController(CFAContext context)
        {
            _context = context;
        }

        // Método para validar el formato del email
        private bool EsEmailValido(string email)
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<IActionResult> RegistrarCliente([FromBody] Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Valida tipo de documento según la edad
            var edad = DateTime.Now.Year - cliente.FechaNacimiento.Year;
            if (cliente.FechaNacimiento > DateTime.Now.AddYears(-edad)) edad--;

            // Define los tipos de documentos permitidos
            var tiposPermitidos = new[] { "CC", "TI", "RC" };

            // Verifica si el tipo de documento es válido
            if (!tiposPermitidos.Contains(cliente.TipoDocumento) || string.IsNullOrEmpty(cliente.TipoDocumento))
            {
                return BadRequest($"El tipo de documento no es válido. Los tipos permitidos son: {string.Join(", ", tiposPermitidos)} y es obligatorio.");
            }

            // Valida según la edad el tipo de documento que corresponde
            if ((edad >= 0 && edad <= 7 && cliente.TipoDocumento != "RC") ||
                (edad >= 8 && edad <= 17 && cliente.TipoDocumento != "TI") ||
                (edad >= 18 && cliente.TipoDocumento != "CC"))
            {
                return BadRequest("El tipo de documento seleccionado no es válido para la edad del cliente.");
            }

            // Validar que el NumeroDocumento sea un número válido
            if (cliente.NumeroDocumento <= 0)
            {
                return BadRequest("Número documento es obligatorio y debe ser un número positivo.");
            }

            // Validar longitud del número de documento
            var numeroDocumentoStr = cliente.NumeroDocumento.ToString();
            if (numeroDocumentoStr.Length < 6 || numeroDocumentoStr.Length > 11)
            {
                return BadRequest("Número documento debe tener entre 6 y 11 dígitos.");
            }
            var existeCliente = await _context.Clientes
             .AnyAsync(c => c.NumeroDocumento == cliente.NumeroDocumento && c.TipoDocumento == cliente.TipoDocumento);

            if (existeCliente)
            {
                return BadRequest("El tipo y número de documento ya están registrados.");
            }

            // Valida  longitud de Nombres
            if (string.IsNullOrEmpty(cliente.Nombres) || cliente.Nombres.Length > 30)
            {
                return BadRequest("El nombre es obligatorio y no pueden exceder los 30 caracteres.");
            }
            // Valida longitud de Apellido1
            if (string.IsNullOrEmpty(cliente.Apellido1) || cliente.Apellido1.Length > 30)
            {
                return BadRequest("Los Primer apellido es obligatorio y no pueden exceder los 30 caracteres.");
            }

            if (string.IsNullOrEmpty(cliente.Nombres) || cliente.Nombres.Length > 30)
            {
                return BadRequest("El nombre es obligatorio y no pueden exceder los 30 caracteres.");
            }

            var tiposGenero = new[] { "M", "F" };

            if (!tiposGenero.Contains(cliente.Genero) || string.IsNullOrEmpty(cliente.Genero))
            {
                return BadRequest($"El genero es un campo obligatorio y los generos permitidos son Los tipos permitidos son: {string.Join(", ", tiposGenero)}.");
            }
            // Validar el formato del Email
            if (string.IsNullOrEmpty(cliente.Email))
            {
                return BadRequest("El formato del email es inválido.");
            }
            // Validar el formato del Email
            if (!EsEmailValido(cliente.Email))
            {
                return BadRequest("El formato del email es inválido.");
            }

            // Valida longitud de Apellido2
            if (cliente.Apellido2.Length > 30)
            {
                return BadRequest("El segundo apellido no puede exceder los 30 caracteres.");
            }
            // Valida que la fecha de nacimiento no esté vacía
            if (cliente.FechaNacimiento == default(DateTime))
            {
                return BadRequest("La fecha de nacimiento es obligatoria.");
            }
            // Validar que al menos una dirección está presente y no esté vacía
            if (cliente.Direcciones == null || !cliente.Direcciones.Any() || cliente.Direcciones.Any(d => string.IsNullOrWhiteSpace(d.DireccionCompleta)))
            {
                return BadRequest("Debe proporcionar al menos una dirección válida.");
            }

            // Validar que al menos un teléfono está presente y no esté vacío
            if (cliente.Telefonos == null || !cliente.Telefonos.Any() || cliente.Telefonos.Any(t => t.NumeroTelefono <= 0))
            {
                return BadRequest("Debe proporcionar al menos un teléfono válido.");
            }

            // Agrega direcciones
            foreach (var direccion in cliente.Direcciones)
            {
                _context.Direcciones.Add(direccion);
            }

            // Agrega teléfonos
            foreach (var telefono in cliente.Telefonos)
            {
                _context.Telefonos.Add(telefono);
            }

            // Agrega el nuevo cliente a la base de datos
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return Ok(new { Cliente = cliente, Mensaje = "Cliente registrado exitosamente." });
        }

        [HttpGet("buscarPorNombre")]
        public IActionResult BuscarPorNombre(string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                return BadRequest("El nombre es obligatorio.");
            }

            var resultados = _context.Clientes
                .Where(c => c.Nombres.Contains(nombre) ||
                            c.Apellido1.Contains(nombre) ||
                            c.Apellido2.Contains(nombre))
                .OrderBy(c => c.Nombres)
                .Select(c => new
                {
                    c.NumeroDocumento,
                    NombreCompleto = $"{c.Nombres} {c.Apellido1} {c.Apellido2}"
                })
                .ToList();

            if (!resultados.Any())
            {
                return NotFound("No se encontraron clientes con ese nombre.");
            }

            return Ok(resultados);
        }

        // GET: api/clientes/buscarPorNumeroDocumento
        [HttpGet("buscarPorNumeroDocumento")]
        [Authorize] // Este método requiere autenticación
        public IActionResult BuscarPorNumeroDocumento(long numeroDocumento)
        {
            var resultado = _context.Clientes
                .Where(c => c.NumeroDocumento == numeroDocumento)
                .OrderByDescending(c => c.NumeroDocumento)
                .Select(c => new
                {
                    c.NumeroDocumento,
                    NombreCompleto = $"{c.Nombres} {c.Apellido1} {c.Apellido2}"
                })
                .FirstOrDefault();

            if (resultado == null)
            {
                return NotFound("Cliente no encontrado");
            }

            return Ok(resultado);
        }

        // GET: api/clientes/buscarPorFechaNacimiento
        [HttpGet("buscarPorFechaNacimiento")]
        public IActionResult BuscarPorFechaNacimiento(DateTime fechaNacimiento)
        {
            var resultados = _context.Clientes
                .Where(c => c.FechaNacimiento.Date == fechaNacimiento.Date)
                .OrderBy(c => c.FechaNacimiento)
                .Select(c => new { c.FechaNacimiento, NombreCompleto = $"{c.Nombres} {c.Apellido1} {c.Apellido2}" })
                .ToList();

            return Ok(resultados);
        }

        // GET: api/clientes/buscarPorRangoFechas
        [HttpGet("buscarPorRangoFechas")]
        public IActionResult BuscarPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            var resultados = _context.Clientes
                .Where(c => c.FechaNacimiento >= fechaInicio && c.FechaNacimiento <= fechaFin)
                .OrderBy(c => c.FechaNacimiento)
                .Select(c => new
                {
                    c.FechaNacimiento,
                    NombreCompleto = $"{c.Nombres} {c.Apellido1} {c.Apellido2}"
                })
                .ToList();

            return Ok(resultados);
        }

        // PUT: api/clientes/{codigo}
        [HttpPut("{codigo}")]
        public async Task<IActionResult> ActualizarCliente(int codigo, [FromBody] Cliente clienteActualizado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var clienteExistente = await _context.Clientes.FindAsync(codigo);

            if (clienteExistente == null)
            {
                return NotFound();
            }

            // Validaciones para TipoDocumento
            var edad = DateTime.Now.Year - clienteActualizado.FechaNacimiento.Year;
            if (clienteActualizado.FechaNacimiento > DateTime.Now.AddYears(-edad)) edad--;

            var tiposPermitidos = new[] { "CC", "TI", "RC" };

            // Verifica si el tipo de documento es válido
            if (!tiposPermitidos.Contains(clienteActualizado.TipoDocumento) || string.IsNullOrEmpty(clienteActualizado.TipoDocumento))
            {
                return BadRequest($"El tipo de documento no es válido. Los tipos permitidos son: {string.Join(", ", tiposPermitidos)} y es obligatorio.");
            }

            // Valida según la edad el tipo de documento que corresponde
            if ((edad >= 0 && edad <= 7 && clienteActualizado.TipoDocumento != "RC") ||
                (edad >= 8 && edad <= 17 && clienteActualizado.TipoDocumento != "TI") ||
                (edad >= 18 && clienteActualizado.TipoDocumento != "CC"))
            {
                return BadRequest("El tipo de documento no es válido para la edad del cliente.");
            }

            // Validar que el NumeroDocumento sea un número válido
            if (clienteActualizado.NumeroDocumento <= 0)
            {
                return BadRequest("Número documento es obligatorio y debe ser un número positivo.");
            }

            // Validar longitud del número de documento
            var numeroDocumentoStr = clienteActualizado.NumeroDocumento.ToString();
            if (numeroDocumentoStr.Length < 6 || numeroDocumentoStr.Length > 11)
            {
                return BadRequest("Número documento debe tener entre 6 y 11 dígitos.");
            }

            // Validar el formato del Email
            if (string.IsNullOrEmpty(clienteActualizado.Email))
            {
                return BadRequest("El formato del email es inválido.");
            }

            // Verifica si el documento ya existe por tipo y número, excluyendo el cliente actual
            var existeCliente = await _context.Clientes.AnyAsync(c =>
                c.NumeroDocumento == clienteActualizado.NumeroDocumento &&
                c.TipoDocumento == clienteActualizado.TipoDocumento &&
                c.Codigo != codigo); // Asegúrate de que no esté comparando con sí mismo

            if (existeCliente)
            {
                return BadRequest("El tipo y número de documento ya están registrados.");
            }

            // Actualiza los campos del cliente existente con los nuevos valores
            clienteExistente.TipoDocumento = clienteActualizado.TipoDocumento;
            clienteExistente.NumeroDocumento = clienteActualizado.NumeroDocumento;
            clienteExistente.FechaNacimiento = clienteActualizado.FechaNacimiento;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensaje = "Cliente actualizado exitosamente.",
                Cliente = clienteExistente
            });
        }

        // DELETE: api/clientes/{codigo}
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> EliminarCliente(int codigo)
        {
            var clienteExistente = await _context.Clientes.FindAsync(codigo);

            if (clienteExistente == null)
            {
                return NotFound(); // Retorna 404 si el cliente no se encuentra
            }

            _context.Clientes.Remove(clienteExistente); // Elimina el cliente existente
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos

            return Ok(new { Mensaje = "Cliente eliminado exitosamente." });
        }

        [HttpGet("buscar")]
        public IActionResult BuscarClientes(string? nombre = null, long? numeroDocumento = null, DateTime? fechaNacimiento = null)
        {
            var query = _context.Clientes.AsQueryable(); // Inicia con la consulta base

            // Filtrar por nombre si se pasa como parámetro
            if (!string.IsNullOrEmpty(nombre)) // Valida que nombre no sea null o vacío
            {
                query = query.Where(c => c.Nombres.Contains(nombre));
            }

            // Filtrar por número de documento si se pasa como parámetro
            if (numeroDocumento.HasValue)
            {
                query = query.Where(c => c.NumeroDocumento == numeroDocumento);
            }

            // Filtrar por fecha de nacimiento si se pasa como parámetro
            if (fechaNacimiento.HasValue)
            {
                query = query.Where(c => c.FechaNacimiento.Date == fechaNacimiento.Value.Date);
            }

            var resultados = query
                .OrderBy(c => c.Nombres)
                .Select(c => new
                {
                    c.NumeroDocumento,
                    NombreCompleto = $"{c.Nombres} {c.Apellido1} {c.Apellido2}",
                    c.FechaNacimiento
                })
                .ToList();

            return Ok(resultados);
        }

        [HttpGet("conMasDeUnTelefono")]
        public IActionResult ConsultarConMasDeUnTelefono()
        {
            var resultados = _context.Clientes
                .Where(c => c.Telefonos.Count > 1)
                .Select(c => new
                {
                    NombreCompleto = $"{c.Nombres} {c.Apellido1} {c.Apellido2}",
                    CantidadTelefonos = c.Telefonos.Count
                })
                .ToList();

            return Ok(resultados);
        }

        [HttpGet("conMasDeUnaDireccion")]
        public IActionResult ConsultarConMasDeUnaDireccion()
        {
            var clientesConMasDeUnaDireccion = _context.Clientes
                .Where(c => c.Direcciones.Count > 1)
                .ToList();

            var resultados = clientesConMasDeUnaDireccion.Select(c => new
            {
                PrimeraDireccion = c.Direcciones.FirstOrDefault()?.DireccionCompleta ?? "Sin dirección",
                NombreCompleto = $"{c.Nombres} {c.Apellido1} {c.Apellido2}"
            }).ToList();

            return Ok(resultados);
        }

    }

}