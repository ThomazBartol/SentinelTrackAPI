using Microsoft.AspNetCore.Mvc;
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

        public MotoController()
        {
            _repository = new MotoRepository();
        }

        [HttpGet]
        public ActionResult<List<Moto>> GetAll()
        {
            return Ok(_repository.GetAll());
        }

        [HttpGet("{id:guid}")]
        public ActionResult<Moto> GetById(Guid id)
        {
            var moto = _repository.GetById(id);
            return moto == null ? NotFound() : Ok(moto);
        }

        [HttpPost]
        public ActionResult<Moto> Create(Moto moto)
        {
            var createdMoto = _repository.Add(moto);
            return CreatedAtAction(nameof(GetById), new { id = createdMoto.Id }, createdMoto);
        }

        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, Moto moto)
        {
            if (id != moto.Id) return BadRequest();

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
