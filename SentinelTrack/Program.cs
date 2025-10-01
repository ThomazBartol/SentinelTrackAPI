using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("SentinelTrackDb"));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API do projeto SentinelTrack",
        Version = "v1",
        Description = "API do projeto SentinelTrack do Challenge da Mottu.",
        Contact = new OpenApiContact
        {
            Name = "Thomaz Bartol",
            Email = "rm555323@fiap.com.br"
        }
    });
    o.SchemaFilter<RequestExamplesSchemaFilter>();
    o.EnableAnnotations();
    //o.SchemaFilter<SwaggerSchemaExampleFilter>();

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    o.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Yards.Any())
    {
        var y1 = new Yard { Id = Guid.NewGuid(), Name = "Pátio Central", Address = "Av. Paulista, 1000", PhoneNumber = "+55 11 4002-8922", Capacity = 150 };
        db.Yards.Add(y1);
        db.Users.Add(new User { Id = Guid.NewGuid(), Name = "Admin", Email = "admin@sentineltrack.com", Role = "admin" });
        db.Motos.Add(new Moto { Id = Guid.NewGuid(), Plate = "ABC1D23", Model = "Honda CG 160", Color = "Preta", YardId = y1.Id });
        db.SaveChanges();
    }
}

app.Run();

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Yard> Yards => Set<Yard>();
    public DbSet<Moto> Motos => Set<Moto>();
    public DbSet<User> Users => Set<User>();
}

public class Yard
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public int Capacity { get; set; }
    public ICollection<Moto> Motos { get; set; } = new List<Moto>();
}

public class Moto
{
    public Guid Id { get; set; }
    public string Plate { get; set; } = default!;
    public string Model { get; set; } = default!;
    public string? Color { get; set; }
    public Guid YardId { get; set; }
    public Yard? Yard { get; set; }
}

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
}

public record YardRequest(string Name, string? Address, string? PhoneNumber, int Capacity);
public record YardResponse(Guid Id, string Name, string? Address, string? PhoneNumber, int Capacity);
public record MotoRequest(string Plate, string Model, string? Color, Guid YardId);
public record MotoResponse(Guid Id, string Plate, string Model, string? Color, Guid YardId);
public record UserRequest(string Name, string Email, string Role);
public record UserResponse(Guid Id, string Name, string Email, string Role);

public static class Hateoas
{
    public static object PageLinks(IUrlHelper url, string action, int page, int pageSize, int totalPages, object? extra = null)
    {
        var next = page < totalPages ? url.ActionLink(action, values: Merge(new { page = page + 1, pageSize }, extra)) : null;
        var prev = page > 1 ? url.ActionLink(action, values: Merge(new { page = page - 1, pageSize }, extra)) : null;
        return new { self = url.ActionLink(action, values: Merge(new { page, pageSize }, extra)), next, prev };
    }
    public static object ItemLinks(IUrlHelper url, string getById, string list, string update, string delete, Guid id)
    {
        return new { self = url.ActionLink(getById, values: new { id }), list = url.ActionLink(list), update = url.ActionLink(update, values: new { id }), delete = url.ActionLink(delete, values: new { id }) };
    }
    static object Merge(object a, object? b)
    {
        if (b is null) return a;
        var d = a.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(a));
        foreach (var p in b.GetType().GetProperties()) d[p.Name] = p.GetValue(b);
        return d;
    }
}

public sealed class RequestExamplesSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(YardRequest))
            schema.Example = Example(new { name = "Pátio Central", address = "Av. Paulista, 1000", phoneNumber = "+55 11 4002-8922", capacity = 150 });
        else if (context.Type == typeof(MotoRequest))
            schema.Example = Example(new { plate = "ABC1D23", model = "Honda CG 160", color = "Preta", yardId = "00000000-0000-0000-0000-000000000001" });
        else if (context.Type == typeof(UserRequest))
            schema.Example = Example(new { name = "Ana Souza", email = "ana@sentineltrack.com", role = "admin" });
    }
    static Microsoft.OpenApi.Any.OpenApiObject Example(object anon)
    {
        var o = new Microsoft.OpenApi.Any.OpenApiObject();
        foreach (var p in anon.GetType().GetProperties())
        {
            var v = p.GetValue(anon);
            o[p.Name] = v switch
            {
                null => new Microsoft.OpenApi.Any.OpenApiNull(),
                string s => new Microsoft.OpenApi.Any.OpenApiString(s),
                int i => new Microsoft.OpenApi.Any.OpenApiInteger(i),
                _ => new Microsoft.OpenApi.Any.OpenApiString(v.ToString()!)
            };
        }
        return o;
    }
}

[ApiController]
[Route("api/v1/yards")]
[SwaggerTag("Controlador responsável pelo gerenciamento de pátios e suas operações.")]
public class YardController : ControllerBase
{
    private readonly AppDbContext _db;
    public YardController(AppDbContext db) => _db = db;

    /// <summary>
    /// Busca todos os pátios.
    /// </summary>
    /// <param name="page">Número da página (começa em 1)</param>
    /// <param name="pageSize">Quantidade de itens por página</param>
    /// <param name="capacityMin">Filtro opcional por capacidade mínima</param>
    /// <param name="capacityMax">Filtro opcional por capacidade máxima</param>
    /// <returns>Lista de pátios</returns>
    [HttpGet]
    public IActionResult Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? capacityMin = null, [FromQuery] int? capacityMax = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        var q = _db.Yards.AsQueryable();
        if (capacityMin.HasValue) q = q.Where(x => x.Capacity >= capacityMin.Value);
        if (capacityMax.HasValue) q = q.Where(x => x.Capacity <= capacityMax.Value);
        var total = q.Count();
        var items = q.OrderBy(x => x.Name).Skip((page - 1) * pageSize).Take(pageSize).Select(x => new YardResponse(x.Id, x.Name, x.Address, x.PhoneNumber, x.Capacity)).ToList();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        var result = new { page, pageSize, totalItems = total, totalPages, items, _links = Hateoas.PageLinks(Url, nameof(Get), page, pageSize, totalPages, new { capacityMin, capacityMax }) };
        return Ok(result);
    }

    /// <summary>
    /// Busca um pátio por id.
    /// </summary>
    /// <param name="id">Id do pátio desejado</param>
    /// <returns>Pátio com o id informado</returns>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var e = _db.Yards.Find(id);
        if (e is null) return NotFound();
        var dto = new YardResponse(e.Id, e.Name, e.Address, e.PhoneNumber, e.Capacity);
        var links = Hateoas.ItemLinks(Url, nameof(GetById), nameof(Get), nameof(Put), nameof(Delete), id);
        return Ok(new { yard = dto, _links = links });
    }

    /// <summary>
    /// Cria um novo pátio
    /// </summary>
    /// <param name="request">Dados do pátio para criação</param>
    /// <returns>Pátio criado</returns>
    [HttpPost]
    public IActionResult Post([FromBody] YardRequest request)
    {
        var e = new Yard { Id = Guid.NewGuid(), Name = request.Name, Address = request.Address, PhoneNumber = request.PhoneNumber, Capacity = request.Capacity };
        _db.Yards.Add(e);
        _db.SaveChanges();
        var dto = new YardResponse(e.Id, e.Name, e.Address, e.PhoneNumber, e.Capacity);
        return CreatedAtAction(nameof(GetById), new { id = e.Id }, dto);
    }

    /// <summary>
    /// Atualiza um pátio existente
    /// </summary>
    /// <param name="id">Id do pátio a ser atualizado</param>
    /// <param name="request">Dados do pátio para atualização</param>
    /// <returns>Confirmação do pátio atualizado</returns>
    [HttpPut("{id:guid}")]
    public IActionResult Put(Guid id, [FromBody] YardRequest request)
    {
        var e = _db.Yards.Find(id);
        if (e is null) return NotFound();
        e.Name = request.Name;
        e.Address = request.Address;
        e.PhoneNumber = request.PhoneNumber;
        e.Capacity = request.Capacity;
        _db.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Deleta um pátio existente
    /// </summary>
    /// <param name="id">Id do pátio a ser deletado</param>
    /// <returns>Confirmação do pátio removido</returns>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var e = _db.Yards.Find(id);
        if (e is null) return NotFound();
        _db.Yards.Remove(e);
        _db.SaveChanges();
        return NoContent();
    }
}

[ApiController]
[Route("api/v1/motos")]
[SwaggerTag("Controlador responsável pelo gerenciamento de motos e suas operações.")]
public class MotoController : ControllerBase
{
    private readonly AppDbContext _db;
    public MotoController(AppDbContext db) => _db = db;

    /// <summary>
    /// Busca todas as motos.
    /// </summary>
    /// <param name="page">Número da página (começa em 1)</param>
    /// <param name="pageSize">Quantidade de itens por página</param>
    /// <param name="plate">Filtro opcional por placa</param>
    /// <param name="yardId">Filtro opcional por id do pátio</param>
    /// <returns>Lista de motos</returns>
    [HttpGet]
    public IActionResult Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? plate = null, [FromQuery] Guid? yardId = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        var q = _db.Motos.AsQueryable();
        if (!string.IsNullOrWhiteSpace(plate)) q = q.Where(x => x.Plate.Contains(plate));
        if (yardId.HasValue) q = q.Where(x => x.YardId == yardId.Value);
        var total = q.Count();
        var items = q.OrderBy(x => x.Plate).Skip((page - 1) * pageSize).Take(pageSize).Select(x => new MotoResponse(x.Id, x.Plate, x.Model, x.Color, x.YardId)).ToList();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        var result = new { page, pageSize, totalItems = total, totalPages, items, _links = Hateoas.PageLinks(Url, nameof(Get), page, pageSize, totalPages, new { plate, yardId }) };
        return Ok(result);
    }

    /// <summary>
    /// Busca uma moto por id.
    /// </summary>
    /// <param name="id">Id da moto desejada</param>
    /// <returns>Moto com o id informado</returns>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var e = _db.Motos.Find(id);
        if (e is null) return NotFound();
        var dto = new MotoResponse(e.Id, e.Plate, e.Model, e.Color, e.YardId);
        var links = Hateoas.ItemLinks(Url, nameof(GetById), nameof(Get), nameof(Put), nameof(Delete), id);
        return Ok(new { moto = dto, _links = links });
    }

    /// <summary>
    /// Cria uma nova moto
    /// </summary>
    /// <param name="request">Dados da moto para criação</param>
    /// <returns>Moto criado</returns>
    [HttpPost]
    public IActionResult Post([FromBody] MotoRequest request)
    {
        var yard = _db.Yards.Find(request.YardId);
        if (yard is null) return BadRequest(new { error = "YardId inválido" });
        var e = new Moto { Id = Guid.NewGuid(), Plate = request.Plate, Model = request.Model, Color = request.Color, YardId = request.YardId };
        _db.Motos.Add(e);
        _db.SaveChanges();
        var dto = new MotoResponse(e.Id, e.Plate, e.Model, e.Color, e.YardId);
        return CreatedAtAction(nameof(GetById), new { id = e.Id }, dto);
    }

    /// <summary>
    /// Atualiza uma moto existente
    /// </summary>
    /// <param name="id">Id da moto a ser atualizada</param>
    /// <param name="request">Dados da moto para atualização</param>
    /// <returns>Confirmação da moto atualizada</returns>
    [HttpPut("{id:guid}")]
    public IActionResult Put(Guid id, [FromBody] MotoRequest request)
    {
        var e = _db.Motos.Find(id);
        if (e is null) return NotFound();
        var yard = _db.Yards.Find(request.YardId);
        if (yard is null) return BadRequest(new { error = "YardId inválido" });
        e.Plate = request.Plate;
        e.Model = request.Model;
        e.Color = request.Color;
        e.YardId = request.YardId;
        _db.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Deleta uma moto existente
    /// </summary>
    /// <param name="id">Id da moto a ser deletada</param>
    /// <returns>Confirmação da moto removida</returns>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var e = _db.Motos.Find(id);
        if (e is null) return NotFound();
        _db.Motos.Remove(e);
        _db.SaveChanges();
        return NoContent();
    }
}

[ApiController]
[Route("api/v1/users")]
[SwaggerTag("Controlador responsável pelo gerenciamento de usuários e suas operações.")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _db;
    public UserController(AppDbContext db) => _db = db;

    /// <summary>
    /// Busca todos os usuários.
    /// </summary>
    /// <param name="page">Número da página (começa em 1)</param>
    /// <param name="pageSize">Quantidade de itens por página</param>
    /// <param name="email">Filtro opcional por email</param>
    /// <returns>Lista de usuários</returns>
    [HttpGet]
    public IActionResult Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? email = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        var q = _db.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(email)) q = q.Where(x => x.Email.Contains(email));
        var total = q.Count();
        var items = q.OrderBy(x => x.Name).Skip((page - 1) * pageSize).Take(pageSize).Select(x => new UserResponse(x.Id, x.Name, x.Email, x.Role)).ToList();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        var result = new { page, pageSize, totalItems = total, totalPages, items, _links = Hateoas.PageLinks(Url, nameof(Get), page, pageSize, totalPages, new { email }) };
        return Ok(result);
    }

    /// <summary>
    /// Busca um usuário por id.
    /// </summary>
    /// <param name="id">Id do usuário desejado</param>
    /// <returns>Usuário com o id informado</returns>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var e = _db.Users.Find(id);
        if (e is null) return NotFound();
        var dto = new UserResponse(e.Id, e.Name, e.Email, e.Role);
        var links = Hateoas.ItemLinks(Url, nameof(GetById), nameof(Get), nameof(Put), nameof(Delete), id);
        return Ok(new { user = dto, _links = links });
    }

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    /// <param name="request">Dados do usuário para criação</param>
    /// <returns>Usuário criado</returns>
    [HttpPost]
    public IActionResult Post([FromBody] UserRequest request)
    {
        var e = new User { Id = Guid.NewGuid(), Name = request.Name, Email = request.Email, Role = request.Role };
        _db.Users.Add(e);
        _db.SaveChanges();
        var dto = new UserResponse(e.Id, e.Name, e.Email, e.Role);
        return CreatedAtAction(nameof(GetById), new { id = e.Id }, dto);
    }

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    /// <param name="id">Id do usuário a ser atualizado</param>
    /// <param name="request">Dados do usuário para atualização</param>
    /// <returns>Confirmação do usuário atualizado</returns>
    [HttpPut("{id:guid}")]
    public IActionResult Put(Guid id, [FromBody] UserRequest request)
    {
        var e = _db.Users.Find(id);
        if (e is null) return NotFound();
        e.Name = request.Name;
        e.Email = request.Email;
        e.Role = request.Role;
        _db.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Deleta um usuário existente
    /// </summary>
    /// <param name="id">Id do usuário a ser deletado</param>
    /// <returns>Confirmação do usuário removido</returns>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var e = _db.Users.Find(id);
        if (e is null) return NotFound();
        _db.Users.Remove(e);
        _db.SaveChanges();
        return NoContent();
    }
}
