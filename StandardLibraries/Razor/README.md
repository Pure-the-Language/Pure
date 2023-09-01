# Razor

Expose .Net Razor engine in a friendly generic format. This is a very thin wrapper around Net Core RazorEngine.

When using RazorEngine, one needs to reference RazorEngine and use something like the following:

```C#
using RazorEngine;
using RazorEngine.Templating;

Engine.Razor.RunCompile(template, templateKey, modelType, model);
```

When using this library, one needs to (in Pure) use the following setup - access the exposed model/dictionary using `@Model` or `@ViewBag`:

```C#
Import(Razor)
public class MyModel
{
    public string Name { get; set; }
    public double Age { get; set; }
}
MyModel model = new()
{
    Name = "James", 
    Age = 15
};
string result = Razor.Main.RunTemplate(model, """
    Hello @Model.Name, you age is @Model.Age.
    """, Razor.TemplateFormat.Text);
```

```C#
Dictionary<string, object> values = new()
{
    { "TotalStudents", 15 },
};
string result = Razor.Main.RunTemplate(values, """
    Total students: @ViewBag.TotalStudents
    """, Razor.TemplateFormat.Text);
Assert.Equal($"Total students: {values["TotalStudents"]}", result);
```

Notice due to limits of RazorEngine, one cannot use record types.

## Known Issue

Due to limits of complied in-memory types from Roslyn cannot be accessed inside Razor, we cannot use generic version of the RunTemplate function inside Pure. This is a serious issue.