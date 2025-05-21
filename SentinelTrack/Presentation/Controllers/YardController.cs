using Microsoft.AspNetCore.Mvc;
using SentinelTrack.Domain.Entities;
using SentinelTrack.Infrastructure.Repositories;

namespace SentinelTrack.Presentation.Controllers
{
    [ApiController]
    [Route("api/yards")]
    [Tags("Pátios")]
    public class YardController : ControllerBase
    {
        private readonly YardRepository _repository = new();

        [HttpGet]
        public ActionResult<List<Yard>> GetAll() => Ok(_repository.GetAll());

        [HttpGet("{id:guid}")]
        public ActionResult<Yard> GetById(Guid id)
        {
            var yard = _repository.GetById(id);
            return yard is null ? NotFound() : Ok(yard);
        }

        [HttpPost]
        public ActionResult<Yard> Create(Yard yard)
        {
            var created = _repository.Add(yard);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, Yard yard)
        {
            if (id != yard.Id) return BadRequest();
            var success = _repository.Update(yard);
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
