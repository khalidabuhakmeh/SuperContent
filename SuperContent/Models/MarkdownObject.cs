using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.AspNetCore.Html;
using YamlDotNet.Serialization;
using Md = Markdig.Markdown;

namespace SuperContent.Models;

public class MarkdownObject<T>
{
    private static readonly MarkdownPipeline MarkdownPipeline 
        = new MarkdownPipelineBuilder()
            .UseYamlFrontMatter()
            .UseAdvancedExtensions()
            .Build();
    private static readonly IDeserializer Deserializer 
        = new DeserializerBuilder()
            .WithYamlFormatter(new YamlFormatter())
            .WithCaseInsensitivePropertyMatching()
            .Build();
    
    private readonly MarkdownDocument document;
    
    public MarkdownObject(string content)
    {
        document = Md.Parse(content, MarkdownPipeline);
        FrontMatter = default;
        
        if (document.Descendants<YamlFrontMatterBlock>().FirstOrDefault() is { } fm)
        {
            var yaml = fm.Lines.ToSlice();
            FrontMatter = Deserializer.Deserialize<T>(yaml.Text);
            
            // we don't want front matter after it's processed
            document.Remove(fm);
        }
        
        // turn it into HTML once
        Html = new HtmlString(document.ToHtml());
    }

    public T? FrontMatter { get; private set; }
    
    public IHtmlContent Html { get; private set; }
}