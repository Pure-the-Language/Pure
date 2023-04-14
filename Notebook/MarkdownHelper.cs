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
        public static ApplicationData LoadDataFrom(string filepath)
        {
            string md = File.ReadAllText(filepath);
            IEnumerable<CellBlock> codeBlocks = Regex.Matches(md, @"```(.*?)[\n\r](.*?)```", RegexOptions.Singleline)
                .Select(m =>
                {
                    string type = m.Groups[1].Value.Trim().ToLower();
                    string code = m.Groups[2].Value.Trim();
                    return type switch
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
                        _ => null,
                    };
                }).Where(c => c != null);
            return new ApplicationData()
            {
                Cells = new ObservableCollection<CellBlock>(codeBlocks)
            };
        }
    }
}
