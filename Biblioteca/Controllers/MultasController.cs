using Biblioteca.Core.Entities;
using Biblioteca.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MultasController : ControllerBase
{
    private readonly IMultaService _service;
    private readonly IBaseRepository<Multa> _repo;

    public MultasController(IMultaService service, IBaseRepository<Multa> repo)
    {
        _service = service;
        _repo = repo;
    }

    // GET: api/multas
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _repo.GetAll();
        return Ok(data.Select(m => new {
            m.Id,
            m.PrestamoId,
            m.UsuarioId,
            m.Motivo,
            m.Detalle,
            m.MontoBs,
            m.Estado
        }));
    }

    // GET: api/multas/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var m = await _repo.GetById(id);
        if (m is null) return NotFound();
        return Ok(new { m.Id, m.PrestamoId, m.UsuarioId, m.Motivo, m.Detalle, m.MontoBs, m.Estado });
    }

    // GET: api/multas/usuario/3
    [HttpGet("usuario/{usuarioId:int}")]
    public async Task<IActionResult> ListarPorUsuario(int usuarioId)
    {
        var todas = await _repo.GetAll();
        var result = todas.Where(m => m.UsuarioId == usuarioId)
                          .Select(m => new { m.Id, m.PrestamoId, m.UsuarioId, m.Motivo, m.Detalle, m.MontoBs, m.Estado });
        return Ok(result);
    }

    // PUT: api/multas/5/pagar
    [HttpPut("{id:int}/pagar")]
    public async Task<IActionResult> Pagar(int id)
    {
        var ok = await _service.PagarMultaAsync(id);
        return ok ? NoContent() : NotFound(new { message = "La multa no existe." });
    }

    // PUT: api/multas/5/cancelar?motivo=Error
    [HttpPut("{id:int}/cancelar")]
    public async Task<IActionResult> Cancelar(int id, [FromQuery] string? motivo = null)
    {
        var ok = await _service.CancelarMultaAsync(id, motivo);
        return ok ? NoContent() : NotFound(new { message = "La multa no existe." });
    }
}
