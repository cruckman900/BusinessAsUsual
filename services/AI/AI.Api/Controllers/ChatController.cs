using AI.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.Api.Controllers;

/// <summary>
/// Chat endpoint backing the in-app AI assistant widget.
/// Routes to the demo tier by default; a valid provisioned company id unlocks the paid tier.
/// </summary>
[ApiController]
[Route("api/ai/chat")]
public sealed class ChatController : ControllerBase
{
    private readonly IAiChatService _chat;

    public ChatController(IAiChatService chat)
    {
        _chat = chat;
    }

    /// <summary>Request body for a chat call.</summary>
    public sealed class ChatRequestBody
    {
        /// <summary>The user's question.</summary>
        public string? Query { get; set; }

        /// <summary>Optional company id; when it maps to a provisioned company, the paid tier is used.</summary>
        public Guid? CompanyId { get; set; }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] ChatRequestBody body, CancellationToken ct)
    {
        if (body is null || string.IsNullOrWhiteSpace(body.Query))
        {
            return BadRequest(new { error = "A non-empty 'query' is required." });
        }

        var result = await _chat.AskAsync(new AiChatRequest(body.Query, body.CompanyId), ct);

        return Ok(new
        {
            answer = result.Answer,
            tier = result.Tier,
            model = result.Model
        });
    }
}
