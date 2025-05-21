using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SentinelTrack.Application.DTOs.Response;
using SentinelTrack.Application.DTOs.Request;
using SentinelTrack.Domain.Entities;
using SentinelTrack.Infrastructure.Repositories;

namespace SentinelTrack.Presentation.Controllers
{
    [ApiController]
    [Route("api/motos")]
    [Tags("Motos")]
    public class MotoController : ControllerBase
    {
        private readonly MotoRepository _repository;
        private readonly IMapper _mapper;

        public MotoController(MotoRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<List<MotoResponse>> GetAll()
        {
            var motos = _repository.GetAll();
            var motoDtos = _mapper.Map<List<MotoResponse>>(motos);
            return Ok(motoDtos);
        }

        [HttpGet("{id:guid}")]
        public ActionResult<MotoResponse> GetById(Guid id)
        {
            var moto = _repository.GetById(id);
            if (moto == null) return NotFound();
            var motoDto = _mapper.Map<MotoResponse>(moto);
            return Ok(motoDto);
        }

        [HttpPost]
        public ActionResult<MotoResponse> Create(MotoRequest request)
        {
            var moto = _mapper.Map<Moto>(request);
            var createdMoto = _repository.Add(moto);
            var motoDto = _mapper.Map<MotoResponse>(createdMoto);
            return CreatedAtAction(nameof(GetById), new { id = motoDto.Id }, motoDto);
        }

        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, MotoRequest request)
        {
            var moto = _mapper.Map<Moto>(request);
            moto.Id = id;
            var success = _repository.Update(moto);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var success = _repository.Remove(id);
            return success ? NoContent() : NotFound();
        }
    }
}
