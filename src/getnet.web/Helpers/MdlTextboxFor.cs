using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace getnet.Helpers
{
    [HtmlTargetElement("div", Attributes = "mdl-textbox-for, mdl-label, mdl-value")]
    public class MdlTextboxFor : TagHelper
    {
        [HtmlAttributeName("mdl-textbox-for")]
        public string Field { get; set; }

        [HtmlAttributeName("mdl-label")]
        public string Label { get; set; }

        [HtmlAttributeName("mdl-value")]
        public string FieldValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var html = string.Format(@"
    <input class=""mdl-textfield__input"" type=""text"" name=""{0}"" id=""{0}"" value=""{2}"">
    <label class=""mdl-textfield__label"" for=""{0}"">{1}</label>
", Field, Label, FieldValue);
            output.Attributes.Add("class", "mdl-textfield mdl-js-textfield mdl-textfield--floating-label");
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(html);
        }
    }
}
