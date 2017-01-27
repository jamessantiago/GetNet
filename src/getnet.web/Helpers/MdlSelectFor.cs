using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace getnet.Helpers
{
    [HtmlTargetElement("div", Attributes = "mdl-select-for, mdl-label, mdl-default-value, mdl-default-value-text, mdl-values")]
    public class MdlSelectFor : TagHelper
    {
        [HtmlAttributeName("mdl-select-for")]
        public string Field { get; set; }

        [HtmlAttributeName("mdl-label")]
        public string Label { get; set; }

        [HtmlAttributeName("mdl-default-value")]
        public string FieldValue { get; set; }

        [HtmlAttributeName("mdl-default-value-text")]
        public string FieldValueText { get; set; }

        [HtmlAttributeName("mdl-values")]
        public SelectList Values { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"
<input class=""mdl-textfield__input"" id=""{Field}"" name=""{Field}"" value=""{FieldValueText}"" type=""text"" readonly tabIndex=""-1"" data-val=""{FieldValue}"" />
<label class=""mdl-textfield__label"" for=""{Field}"">{Label}</label>
<ul class=""mdl-menu mdl-menu--bottom-left mdl-js-menu"" for=""{Field}"">");

            foreach (SelectListItem val in Values.Items)
            {
                sb.AppendLine($@"<li class=""mdl-menu__item"" data-val=""{val.Value}"">{val.Text}</li>");
            }
            sb.AppendLine("</ul>");

            output.Attributes.Add("class", "mdl-textfield mdl-js-textfield mdl-textfield--floating-label getmdl-select");
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(sb.ToString());
        }
    }
}
