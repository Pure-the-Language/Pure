using Razor;

namespace RazorTest
{
    internal class InternalModel
    {
        public string Name { get; set; }
        public double Age { get; set; }
    }

    public class MainFunctionsTest
    {
        public class MyModel
        {
            public string Name { get; set; }
            public double Age { get; set; }
        }

        [Fact]
        public void TemplateShouldWork()
        {
            MyModel model = new()
            {
                Name = "James", 
                Age = 15
            };
            string result = Razor.Main.RunTemplate(model, """
                Hello @Model.Name, you age is @Model.Age.
                """, Razor.TemplateFormat.Text);
            Assert.Equal($"Hello {model.Name}, you age is {model.Age}.", result);
        }

        [Fact]
        public void TemplateShouldWorkWithLogic()
        {
            string[] items = new[] { "apple", "banada" };
            var bags = new Dictionary<string, object>
            {
                {"Items", items}
            };

            string result = Razor.Main.RunTemplate(bags, """
                # Document
                Items supported:
                @foreach(var a in ViewBag.Items)
                {
                    @($"{a} ")
                }
                """, TemplateFormat.Text);

            Assert.Equal("""
                # Document
                Items supported:
                apple banada 
                """, result);
        }


        [Fact]
        public void TemplateShouldNotWorkForInternalTypes()
        {
            InternalModel model = new()
            {
                Name = "James",
                Age = 15
            };
            Assert.Throws<RazorEngine.Templating.TemplateCompilationException>(() =>
            {
                string result = Razor.Main.RunTemplate(model, """
                    Hello @Model.Name, you age is @Model.Age.
                    """, Razor.TemplateFormat.Text);
            });
        }
    }
}