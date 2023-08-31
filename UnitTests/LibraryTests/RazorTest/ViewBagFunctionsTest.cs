namespace RazorTest
{
    public class ViewBagFunctionsTest
    {

        [Fact]
        public void TemplateShouldWorkWithViewBag()
        {
            Dictionary<string, object> values = new()
            {
                { "TotalStudents", 15 },
            };
            string result = Razor.Main.RunTemplate(values, """
                Total students: @ViewBag.TotalStudents
                """, Razor.TemplateFormat.Text);
            Assert.Equal($"Total students: {values["TotalStudents"]}", result);
        }
    }
}