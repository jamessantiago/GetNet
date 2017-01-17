using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace getnet.Helpers
{
    [HtmlTargetElement("button", Attributes = "mdl-submit-button")]
    public class MdlSubmitButton : TagHelper
    {
        [HtmlAttributeName("mdl-submit-button")]
        public string Text { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.Add("class", "mdl-button mdl-js-button mdl-button--raised mdl-js-ripple-effect mdl-button--accent");
            output.Attributes.Add("type", "submit");
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetContent(Text);
        }
    }
}
