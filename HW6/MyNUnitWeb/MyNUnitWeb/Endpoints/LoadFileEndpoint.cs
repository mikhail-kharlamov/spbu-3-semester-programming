using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("my-nunit/api/load-file")]
public class TestEndpoint : ControllerBase
{
    [HttpGet]
    public IActionResult Get(int id)
        => Ok(new { id });
}
