using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelTrack.Application.DTOs.Request;
using SentinelTrack.Application.DTOs.Response;
using SentinelTrack.Domain.Entities;
using SentinelTrack.Infrastructure.Context;
using System.Net;

namespace SentinelTrack.Presentation.Controllers
{
    [ApiController]
    [Route("api/motos")]
    [Tags("Motos")]
    public class MotoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MotoController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<MotoResponse>>> GetAll()
        {
            var motos = await _context.Motos.ToListAsync();
            var motoDtos = _mapper.Map<List<MotoResponse>>(motos);
            return Ok(motoDtos);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<MotoResponse>> GetById(Guid id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) return NotFound();
            var motoDto = _mapper.Map<MotoResponse>(moto);
            return Ok(motoDto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MotoResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<MotoResponse>> Create(MotoRequest request)
        {
            var yard = await _context.Yards.FindAsync(request.YardId);
            if (yard == null) return NotFound("Pátio não encontrado.");

            int motosNoYard = await _context.Motos.CountAsync(m => m.YardId == request.YardId);
            if (motosNoYard >= yard.Capacity)
                return BadRequest("O pátio está lotado. Não é possível adicionar mais motos.");

            var moto = _mapper.Map<Moto>(request);
            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();

            var motoDto = _mapper.Map<MotoResponse>(moto);
            return CreatedAtAction(nameof(GetById), new { id = motoDto.Id }, motoDto);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(Guid id, MotoRequest request)
        {
            var yard = await _context.Yards.FindAsync(request.YardId);
            if (yard == null) return NotFound("Pátio não encontrado.");

            var existingMoto = await _context.Motos.FindAsync(id);
            if (existingMoto == null) return NotFound("Moto não encontrada.");

            if (existingMoto.YardId != request.YardId)
                return BadRequest("A moto não está associada ao pátio informado.");

            int motosNoYard = await _context.Motos.CountAsync(m => m.YardId == request.YardId);
            if (motosNoYard >= yard.Capacity)
                return BadRequest("O pátio está lotado. Não é possível adicionar mais motos.");

            existingMoto.Plate = request.Plate;
            existingMoto.Model = request.Model;
            existingMoto.Color = request.Color;
            existingMoto.YardId = request.YardId;

            _context.Motos.Update(existingMoto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) return NotFound("Moto não encontrada.");

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
