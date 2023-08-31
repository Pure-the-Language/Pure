using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace Razor
{
    public enum TemplateFormat
    {
        Text,
        HTML
    }
    public static class Main
    {
        #region Method
        public static string RunTemplate<TType>(TType model, string template, TemplateFormat format = TemplateFormat.HTML, Dictionary<string, object> additionalValues = null)
        {
            var config = new TemplateServiceConfiguration()
            {
                Language = RazorEngine.Language.CSharp,
                EncodedStringFactory = format == TemplateFormat.HTML ? new HtmlEncodedStringFactory() : new RawStringFactory(),
                Debug = false
            };
            var service = RazorEngineService.Create(config);

            if (additionalValues == null)
                return service.RunCompile(template, "templateKey", typeof(TType), model);
            else
                return service.RunCompile(template, template.GetHashCode().ToString(), typeof(TType), model, new DynamicViewBag(additionalValues));
        }
        public static string RunTemplate(Dictionary<string, object> values, string template, TemplateFormat format = TemplateFormat.HTML)
        {
            var config = new TemplateServiceConfiguration()
            {
                Language = RazorEngine.Language.CSharp,
                EncodedStringFactory = format == TemplateFormat.HTML ? new HtmlEncodedStringFactory() : new RawStringFactory(),
                Debug = false
            };
            var service = RazorEngineService.Create(config);

            return service.RunCompile(template, template.GetHashCode().ToString(), null, null, new DynamicViewBag(values));
        }
        #endregion
    }
}