using Microsoft.AspNetCore.Mvc;
using Markdig;

namespace BusinessAsUsual.Admin
{
    /// <summary>
    /// Controller to display .md files as help content
    /// </summary>
    public class HelpController : Controller
    {
        /// <summary>
        /// Default route
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public IActionResult Index(string topic = "getting-started")
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "HelpDocs", $"{topic}.md");

            if (!System.IO.File.Exists(filePath))
                return NotFound("Help topic not found.");

            var markdown = System.IO.File.ReadAllText(filePath);
            var html = Markdown.ToHtml(markdown);

            ViewData["Title"] = $"Help: {topic.Replace("-", " ").ToUpper()}";
            ViewData["Content"] = html;

            return View();
        }
    }
}