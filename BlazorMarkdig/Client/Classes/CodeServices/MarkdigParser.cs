using Markdig;

namespace BlazorMarkdig.Client.Classes.CodeServices
{
    public class MarkdigParser
    {
        public MarkdigParser()
        {
            this.pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        }

        private MarkdownPipeline pipeline;

        /// <summary>
        /// Converts a Markdown string to HTML.
        /// </summary>
        /// <param name="markdownText">Markdown text to convert</param>
        /// <returns>The HTML result of the conversion from markdown text</returns>
        public string ToHtml(string markdownText)
        {
            return Markdown.ToHtml(markdownText, this.pipeline);
        }
    }
}