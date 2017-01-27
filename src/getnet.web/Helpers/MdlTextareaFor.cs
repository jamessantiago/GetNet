using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace getnet.Helpers
{
    [HtmlTargetElement("div", Attributes = "mdl-textarea-for, mdl-label, mdl-value, mdl-rows")]
    public class MdlTextareaFor : TagHelper
    {
        [HtmlAttributeName("mdl-textarea-for")]
        public string Field { get; set; }

        [HtmlAttributeName("mdl-label")]
        public string Label { get; set; }

        [HtmlAttributeName("mdl-value")]
        public string FieldValue { get; set; }

        [HtmlAttributeName("mdl-rows")]
        public int? Rows { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var html = $@"
<textarea class=""mdl-textfield__input"" type=""text"" name=""{Field}"" rows=""{Rows ?? 4}"" id=""{Field}"">{FieldValue}</textarea>
<label class=""mdl-textfield__label"" for=""{Field}"">{Label}</label>";

            output.Attributes.Add("class", "mdl-textfield mdl-js-textfield mdl-textfield--floating-label");
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(html);
        }
    }
}
