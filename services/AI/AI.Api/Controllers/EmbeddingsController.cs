using Microsoft.AspNetCore.Mvc;

namespace AI.Api.Controllers;

[ApiController]
[Route("api/ai/embeddings")]
public class EmbeddingsController : ControllerBase
{
    [HttpPost("upsert")]
    public IActionResult Upsert([FromBody] object payload)
    {
        // placeholder - persist embeddings into a vector DB in future
        return Ok(new { status = "ok" });
    }

    [HttpPost("query")]
    public IActionResult Query([FromBody] object payload)
    {
        // placeholder - perform RAG and/or LLM call
        return Ok(new { answer = "This is a placeholder AI response.", sources = new string[0] });
    }
}
