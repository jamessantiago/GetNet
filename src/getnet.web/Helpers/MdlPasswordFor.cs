using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace getnet.Helpers
{
    [HtmlTargetElement("div", Attributes = "mdl-password-for, mdl-label, mdl-value, mdl-errors")]
    public class MdlPasswordFor : TagHelper
    {
        [HtmlAttributeName("mdl-password-for")]
        public string Field { get; set; }

        [HtmlAttributeName("mdl-label")]
        public string Label { get; set; }

        [HtmlAttributeName("mdl-value")]
        public string FieldValue { get; set; }

        [HtmlAttributeName("mdl-errors")]
        public ModelErrorCollection Errors { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var html = string.Format(@"
    <input class=""mdl-textfield__input"" type=""password"" name=""{0}"" id=""{0}"" value=""{2}"">
    <label class=""mdl-textfield__label"" for=""{0}"">{1}</label>
", Field, Label, FieldValue);
            if (Errors != null && Errors.Any())
                html += $@"<span class=""mdl-textfield__error"">{string.Join("; ", Errors.Select(d => d.ErrorMessage))}</span>";
            output.Attributes.Add("class", "mdl-textfield mdl-js-textfield mdl-textfield--floating-label" + (Errors != null && Errors.Any() ? " is-invalid" : ""));
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(html);
        }
    }
}
