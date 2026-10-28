using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClyvoCareOSChallenge.Controllers;

[ApiController]
[Route("api/seguranca")]
[Produces("application/json")]
public class SegurancaController : ControllerBase
{
    [Authorize]
    [HttpGet("protegido")]
    public IActionResult GetProtegido()
    {
        return Ok(new
        {
            mensagem = "Acesso autorizado."
        });
    }
}