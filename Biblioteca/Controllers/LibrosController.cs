using Biblioteca.Core.Entities;
using Biblioteca.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibrosController : ControllerBase
{
    private readonly IBaseRepository<Libro> _repo;

    public LibrosController(IBaseRepository<Libro> repo)
    {
        _repo = repo;
    }

    // GET: api/libros
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _repo.GetAll();
        return Ok(data.Select(l => new {
            l.Id,
            l.Titulo,
            l.Autor,
            l.Categoria,
            l.CostoReposicionBs,
            l.TotalEjemplares,
            l.EjemplaresDisponibles,
            l.Habilitado,
            l.CondicionGeneral
        }));
    }

    // GET: api/libros/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var l = await _repo.GetById(id);
        if (l is null) return NotFound();
        return Ok(new
        {
            l.Id,
            l.Titulo,
            l.Autor,
            l.Categoria,
            l.CostoReposicionBs,
            l.TotalEjemplares,
            l.EjemplaresDisponibles,
            l.Habilitado,
            l.CondicionGeneral
        });
    }

    // POST: api/libros
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Libro model)
    {
        if (string.IsNullOrWhiteSpace(model.Titulo) || string.IsNullOrWhiteSpace(model.Autor))
            return BadRequest(new { message = "Titulo y Autor son requeridos" });

        if (string.IsNullOrWhiteSpace(model.Categoria)) model.Categoria = "General";
        if (model.TotalEjemplares < 0 || model.EjemplaresDisponibles < 0)
            return BadRequest(new { message = "Los ejemplares no pueden ser negativos" });

        if (string.IsNullOrWhiteSpace(model.CondicionGeneral)) model.CondicionGeneral = "Good";
        var permitidos = new[] { "New", "Good", "Fair", "Poor" };
        if (!permitidos.Contains(model.CondicionGeneral))
            return BadRequest(new { message = "CondicionGeneral debe ser New|Good|Fair|Poor" });

        // al crear, si no mandan Disponibles, lo igualamos a Total
        if (model.EjemplaresDisponibles == 0 && model.TotalEjemplares > 0)
            model.EjemplaresDisponibles = model.TotalEjemplares;

        model.Habilitado = model.Habilitado; // por defecto true
        await _repo.Add(model);
        return CreatedAtAction(nameof(GetById), new { id = model.Id }, new { model.Id });
    }

    // PUT: api/libros/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Libro model)
    {
        var l = await _repo.GetById(id);
        if (l is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(model.Titulo)) l.Titulo = model.Titulo;
        if (!string.IsNullOrWhiteSpace(model.Autor)) l.Autor = model.Autor;
        if (!string.IsNullOrWhiteSpace(model.Categoria)) l.Categoria = model.Categoria;

        if (model.CostoReposicionBs > 0) l.CostoReposicionBs = model.CostoReposicionBs;
        if (model.TotalEjemplares >= 0) l.TotalEjemplares = model.TotalEjemplares;
        if (model.EjemplaresDisponibles >= 0) l.EjemplaresDisponibles = model.EjemplaresDisponibles;

        if (!string.IsNullOrWhiteSpace(model.CondicionGeneral))
        {
            var permitidos = new[] { "New", "Good", "Fair", "Poor" };
            if (!permitidos.Contains(model.CondicionGeneral))
                return BadRequest(new { message = "CondicionGeneral debe ser New|Good|Fair|Poor" });
            l.CondicionGeneral = model.CondicionGeneral;
        }

        // Habilitado si lo envían (true/false)
        if (l.Habilitado != model.Habilitado) l.Habilitado = model.Habilitado;

        await _repo.Update(l);
        return NoContent();
    }

    // DELETE: api/libros/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var l = await _repo.GetById(id);
        if (l is null) return NotFound();
        await _repo.Delete(id);
        return NoContent();
    }

    // PUT: api/libros/5/habilitar
    [HttpPut("{id:int}/habilitar")]
    public async Task<IActionResult> Habilitar(int id)
    {
        var l = await _repo.GetById(id);
        if (l is null) return NotFound();
        l.Habilitado = true;
        await _repo.Update(l);
        return NoContent();
    }

    // PUT: api/libros/5/deshabilitar
    [HttpPut("{id:int}/deshabilitar")]
    public async Task<IActionResult> Deshabilitar(int id)
    {
        var l = await _repo.GetById(id);
        if (l is null) return NotFound();
        l.Habilitado = false;
        await _repo.Update(l);
        return NoContent();
    }

    // PUT: api/libros/5/condicion?condicion=Fair
    [HttpPut("{id:int}/condicion")]
    public async Task<IActionResult> CambiarCondicion(int id, [FromQuery] string condicion)
    {
        var permitidos = new[] { "New", "Good", "Fair", "Poor" };
        if (!permitidos.Contains(condicion))
            return BadRequest(new { message = "CondicionGeneral debe ser New|Good|Fair|Poor" });

        var l = await _repo.GetById(id);
        if (l is null) return NotFound();
        l.CondicionGeneral = condicion;
        await _repo.Update(l);
        return NoContent();
    }
}
