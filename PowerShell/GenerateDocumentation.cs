/* Generate Documentation
Version: v0.1

Generates MD documentation from generated XML documentation files.
*/

Import(Markdig)
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;

public record Member(string Name, string Summary);
public record Documentation(string AssemblyName, Member[] Members);

public static Documentation ParseXML(string filePath)
{
    XmlDocument doc = new XmlDocument();
    doc.Load(filePath);

    string assemblyName = doc.SelectSingleNode("/doc/assembly/name").InnerText;
    Member[] members = doc.SelectSingleNode("/doc/members")
        .ChildNodes
        .OfType<XmlElement>()
        .Select(e => {
            string name = e.GetAttribute("name");
            string summary = e.SelectSingleNode("summary").InnerText.Trim();
            summary = Regex.Replace(summary, @"^\s+", string.Empty, RegexOptions.Multiline);
            return new Member(name, summary);
        })
        .ToArray();

    return new Documentation(assemblyName, members);
}
public static Documentation[] ParseXMLDocumentation(string lookupFolderPath)
{
    return Directory.GetFiles(lookupFolderPath, "*.xml")
        .Select(ParseXML)
        .ToArray();
}
public static void OutputDocumentations(string outputFilePath, Documentation[] docs)
{
    // Build Markdown
    StringBuilder builder = new();
    builder.AppendLine("# Pure API Documentation\n");

    foreach(var doc in docs.OrderBy(d => d.AssemblyName))
    {
        builder.AppendLine($"## {doc.AssemblyName}\n");
        
        foreach(var member in doc.Members)
        {
            builder.AppendLine($"### `{member.Name}`\n");
            builder.AppendLine(member.Summary + '\n');
        }
    }

    // Save MD
    string markdown = builder.ToString().TrimEnd();
    string documentationName = Path.GetFileNameWithoutExtension(outputFilePath);
    strinrg outputFolder = Path.GetDirectory(outputFilePath);
    File.WriteAllText(Path.Combine(outputFolder, $"{documentationName}.md"), markdown);

    // Save HTMl
    var html = Markdown.ToHtml(markdown);
    File.WriteAllText(Path.Combine(outputFolder, $"{documentationName}.html"), html);
}

if (Arguments.Length != 2)
    WriteLine("Arguments: <Lookup Folder Path> <Output File Path>");
else
{
    string lookupFolderPath = Path.GetFullPath(Arguments[0]);
    string outputFilePath = Path.GetFullPath(Arguments[1]);

    var docs = ParseXMLDocumentation(lookupFolderPath);
    OutputDocumentations(outputFilePath, docs);
}