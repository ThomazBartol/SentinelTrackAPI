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
    [Route("api/yards")]
    [Tags("Pátios")]
    public class YardController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public YardController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<YardResponse>>> GetAll(
            [FromQuery] int? capacityMin,
            [FromQuery] int? capacityMax,
            [FromQuery] bool? hasSpace)
        {
            IQueryable<Yard> query = _context.Yards.AsQueryable();

            if (capacityMin.HasValue)
                query = query.Where(y => y.Capacity >= capacityMin.Value);

            if (capacityMax.HasValue)
                query = query.Where(y => y.Capacity <= capacityMax.Value);

            if (hasSpace.HasValue && hasSpace.Value)
            {
                query = query.Where(y =>
                    _context.Motos.Count(m => m.YardId == y.Id) < y.Capacity);
            }

            var yards = await query.ToListAsync();

            var yardIds = yards.Select(y => y.Id).ToList();
            var motos = await _context.Motos.Where(m => yardIds.Contains(m.YardId)).ToListAsync();

            var yardDtos = yards.Select(yard =>
            {
                var yardDto = _mapper.Map<YardResponse>(yard);
                var motosNoYard = motos.Where(m => m.YardId == yard.Id).ToList();
                yardDto.Motos = _mapper.Map<List<MotoResponse>>(motosNoYard);
                return yardDto;
            }).ToList();

            return Ok(yardDtos);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<YardResponse>> GetById(Guid id)
        {
            var yard = await _context.Yards.FindAsync(id);
            if (yard == null) return NotFound();

            var motosNoYard = await _context.Motos.Where(m => m.YardId == id).ToListAsync();

            var yardDto = _mapper.Map<YardResponse>(yard);
            yardDto.Motos = _mapper.Map<List<MotoResponse>>(motosNoYard);

            return Ok(yardDto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(YardResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<YardResponse>> Create(YardRequest request)
        {
            if (request.Capacity <= 0)
            {
                return BadRequest("A capacidade do pátio deve ser maior que 0.");
            }

            var yard = _mapper.Map<Yard>(request);
            _context.Yards.Add(yard);
            await _context.SaveChangesAsync();

            var yardDto = _mapper.Map<YardResponse>(yard);
            return CreatedAtAction(nameof(GetById), new { id = yardDto.Id }, yardDto);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(Guid id, YardRequest request)
        {
            if (request.Capacity <= 0)
            {
                return BadRequest("A capacidade do pátio deve ser maior que 0.");
            }

            var motosNoYard = await _context.Motos.CountAsync(m => m.YardId == id);
            if (request.Capacity < motosNoYard)
            {
                return BadRequest($"Não é possível reduzir a capacidade do pátio para menos do que o número de motos existentes. Motos atuais: {motosNoYard}.");
            }

            var yard = await _context.Yards.FindAsync(id);
            if (yard == null) return NotFound();

            // Atualiza os campos permitidos
            yard.Name = request.Name;
            yard.Capacity = request.Capacity;

            _context.Yards.Update(yard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var yard = await _context.Yards.FindAsync(id);
            if (yard == null) return NotFound();

            _context.Yards.Remove(yard);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
