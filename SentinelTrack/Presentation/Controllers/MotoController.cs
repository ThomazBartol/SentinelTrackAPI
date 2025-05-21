using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SentinelTrack.Application.DTOs.Response;
using SentinelTrack.Application.DTOs.Request;
using SentinelTrack.Domain.Entities;
using SentinelTrack.Infrastructure.Repositories;
using System.Net;

namespace SentinelTrack.Presentation.Controllers
{
    [ApiController]
    [Route("api/motos")]
    [Tags("Motos")]
    public class MotoController : ControllerBase
    {
        private readonly MotoRepository _repository;
        private readonly YardRepository _yardRepository;
        private readonly IMapper _mapper;

        public MotoController(MotoRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public ActionResult<List<MotoResponse>> GetAll()
        {
            var motos = _repository.GetAll();
            var motoDtos = _mapper.Map<List<MotoResponse>>(motos);
            return Ok(motoDtos);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public ActionResult<MotoResponse> GetById(Guid id)
        {
            var moto = _repository.GetById(id);
            if (moto == null) return NotFound();
            var motoDto = _mapper.Map<MotoResponse>(moto);
            return Ok(motoDto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MotoResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<MotoResponse> Create(MotoRequest request)
        {
            var yard = _yardRepository.GetById(request.YardId);
            if (yard == null)
            {
                return NotFound("Pátio não encontrado.");
            }

            int motosNoYard = _repository.GetAll().Count(m => m.YardId == request.YardId);
            if (motosNoYard >= yard.Capacity)
            {
                return BadRequest("O pátio está lotado. Não é possível adicionar mais motos.");
            }

            var moto = _mapper.Map<Moto>(request);
            var createdMoto = _repository.Add(moto);
            var motoDto = _mapper.Map<MotoResponse>(createdMoto);
            return CreatedAtAction(nameof(GetById), new { id = motoDto.Id }, motoDto);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult Update(Guid id, MotoRequest request)
        {
            var yard = _yardRepository.GetById(request.YardId);
            if (yard == null)
            {
                return NotFound("Pátio não encontrado.");
            }

            var existingMoto = _repository.GetById(id);
            if (existingMoto == null)
            {
                return NotFound("Moto não encontrada.");
            }

            if (existingMoto.YardId != request.YardId)
            {
                return BadRequest("A moto não está associada ao pátio informado.");
            }

            int motosNoYard = _repository.GetAll().Count(m => m.YardId == request.YardId);

            if (motosNoYard >= yard.Capacity)
            {
                return BadRequest("O pátio está lotado. Não é possível adicionar mais motos.");
            }

            var moto = _mapper.Map<Moto>(request);
            moto.Id = id;
            var success = _repository.Update(moto);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult Delete(Guid id)
        {
            var moto = _repository.GetById(id);

            if (moto == null)
            {
                return NotFound("Moto não encontrada.");
            }

            var success = _repository.Remove(id);
            return success ? NoContent() : NotFound();
        }
    }
}
