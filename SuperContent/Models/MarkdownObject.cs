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

    public MarkdownObject(string content)
    {
        var doc = Md.Parse(content, MarkdownPipeline);
        FrontMatter = default;
        
        if (doc.Descendants<YamlFrontMatterBlock>().FirstOrDefault() is { } fm)
        {
            var yaml = fm.Lines.ToSlice();
            FrontMatter = Deserializer.Deserialize<T>(yaml.Text);
            
            // we don't want front matter after it's processed
            doc.Remove(fm);
        }
        
        // turn it into HTML once
        Html = new HtmlString(doc.ToHtml());
    }

    public T? FrontMatter { get; private set; }
    
    public IHtmlContent Html { get; private set; }
}