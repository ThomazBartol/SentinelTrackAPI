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
    [Route("api/yards")]
    [Tags("Pátios")]
    public class YardController : ControllerBase
    {
        private readonly YardRepository _repository = new();
        private readonly IMapper _mapper;

        public YardController(YardRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public ActionResult<List<YardResponse>> GetAll()
        {
            var yards = _repository.GetAll();
            var yardDtos = _mapper.Map<List<YardResponse>>(yards);
            return Ok(yardDtos);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public ActionResult<YardResponse> GetById(Guid id)
        {
            var yard = _repository.GetById(id);
            if (yard == null) return NotFound();
            var yardDto = _mapper.Map<YardResponse>(yard);
            return Ok(yardDto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(YardResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult<YardResponse> Create(YardRequest request)
        {
            if (request.Capacity <= 0)
            {
                return BadRequest("A capacidade do pátio deve ser maior que 0.");
            }

            var yard = _mapper.Map<Yard>(request);
            var createdYard = _repository.Add(yard);
            var yardDto = _mapper.Map<YardResponse>(createdYard);
            return CreatedAtAction(nameof(GetById), new { id = yardDto.Id }, yardDto);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult Update(Guid id, YardRequest request)
        {
            if (request.Capacity <= 0)
            {
                return BadRequest("A capacidade do pátio deve ser maior que 0.");
            }

            var yard = _mapper.Map<Yard>(request);
            yard.Id = id;
            var success = _repository.Update(yard);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult Delete(Guid id)
        {
            var success = _repository.Remove(id);
            return success ? NoContent() : NotFound();
        }
    }
}
