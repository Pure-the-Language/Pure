using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Notebook
{
    /// <summary>
    /// Remark-cz: Might need to put this into lower level Core and properly deal with "Include" command, so as to support literate programming on the syntax level;
    /// At the moment it's only useful for a single top-level file.
    /// </summary>
    public class MarkdownHelper
    {
        public static void SaveDataTo(string filepath, ApplicationData data, bool includeCache = false)
        {
            IEnumerable<string> blocks;
            if (includeCache)
            {
                blocks = data.Cells
                    .Select(c =>
                    {
                        if (c.CellType == CellType.CacheOutput && string.IsNullOrWhiteSpace(c.Content))
                            return null;

                        string formatName = c.CellType switch
                        {
                            CellType.Python => "Python",
                            CellType.CSharp => "C#",
                            CellType.CacheOutput => "Cache",
                            _ => string.Empty
                        };
                        return c.CellType == CellType.Markdown
                            ? c.Content
                            : $"""
                            ```{formatName}
                            {c.Content.Trim()}
                            ```
                            """; // Remark-cz: Slightly different treatment than exporting without cache here, because cache can contain meaningless empty spaces at the end; In this case we trim all code blocks
                    })
                    .Where(b => b != null);
            }
            else
            {
                blocks = data.Cells
                    .Where(c => c.CellType != CellType.CacheOutput)
                    .Select(c => c.CellType == CellType.Markdown
                        ? c.Content
                        : $"""
                        ```{(c.CellType == CellType.Python ? "Python" : "C#")}
                        {c.Content}
                        ```
                        """);
            }
            string markdown = string.Join("\n\n", blocks);
            File.WriteAllText(filepath, markdown);
        }
        public static ApplicationData LoadDataFrom(string filepath)
        {
            string md = File.ReadAllText(filepath);
            IEnumerable<CellBlock> codeBlocks = Regex.Matches(md, @"(?<TextBlock>.*?)```(?<CodeFormat>.*?)[\n\r](?<CodeContent>.*?)```", RegexOptions.Singleline)
                .SelectMany(m =>
                {
                    List<CellBlock> blocks = new List<CellBlock>();

                    string markdownBlock = m.Groups["TextBlock"].Value.Trim();
                    if (!string.IsNullOrWhiteSpace(markdownBlock))
                        blocks.Add(new CellBlock()
                        {
                            CellType = CellType.Markdown,
                            Content = markdownBlock
                        });

                    string type = m.Groups["CodeFormat"].Value.Trim().ToLower();
                    string code = m.Groups["CodeContent"].Value.Trim();
                    blocks.Add(type switch
                    {
                        "python" => new CellBlock()
                        {
                            CellType = CellType.Python,
                            Content = code
                        },
                        "c#" => new CellBlock()
                        {
                            CellType = CellType.CSharp,
                            Content = code
                        },
                        "cache" => new CellBlock()
                        {
                            CellType = CellType.CacheOutput,
                            Content = code
                        },
                        _ => null,
                    });

                    return blocks;
                }).Where(c => c != null);
            return new ApplicationData()
            {
                Cells = new ObservableCollection<CellBlock>(codeBlocks)
            };
        }
    }
}
