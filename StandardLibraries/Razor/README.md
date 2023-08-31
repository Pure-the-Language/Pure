# Razor

Expose .Net Razor engine in a friendly generic format. This is a very thin wrapper around Net Core RazorEngine.

When using RazorEngine, one needs to reference RazorEngine and use something like the following:

```C#
using RazorEngine;
using RazorEngine.Templating;

Engine.Razor.RunCompile(template, templateKey, modelType, model);
```

When using this library, one needs to (in Pure):

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

Notice due to limits of RazorEngine, one cannot use record types.