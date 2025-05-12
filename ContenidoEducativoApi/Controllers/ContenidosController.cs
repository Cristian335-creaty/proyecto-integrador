using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContenidoEducativoApi.Context;
using ContenidoEducativoApi.Models;

namespace ContenidoEducativoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContenidosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContenidosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Contenidos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contenido>>> GetContenidos()
        {
            return await _context.Contenidos.ToListAsync();
        }

        // GET: api/Contenidos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contenido>> GetContenido(int id)
        {
            var contenido = await _context.Contenidos.FindAsync(id);

            if (contenido == null)
            {
                return NotFound();
            }

            return contenido;
        }

        // PUT: api/Contenidos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContenido(int id, Contenido contenido)
        {
            if (id != contenido.Id)
            {
                return BadRequest();
            }

            _context.Entry(contenido).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContenidoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Contenidos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Contenido>> PostContenido(Contenido contenido)
        {
            _context.Contenidos.Add(contenido);
            await _context.SaveChangesAsync();

            // return CreatedAtAction("GetContenido", new { id = contenido.Id }, contenido);
            return CreatedAtAction(nameof(GetContenido), new { id = contenido.Id }, contenido);
        }

        // DELETE: api/Contenidos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContenido(int id)
        {
            var contenido = await _context.Contenidos.FindAsync(id);
            if (contenido == null)
            {
                return NotFound();
            }

            _context.Contenidos.Remove(contenido);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContenidoExists(int id)
        {
            return _context.Contenidos.Any(e => e.Id == id);
        }
    }
}
