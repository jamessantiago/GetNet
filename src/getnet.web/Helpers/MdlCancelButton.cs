using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace getnet.Helpers
{
    [HtmlTargetElement("button", Attributes = "mdl-cancel-button, mdl-return-to")]
    public class MdlCancelButton : TagHelper
    {
        [HtmlAttributeName("mdl-cancel-button")]
        public string Text { get; set; }

        [HtmlAttributeName("mdl-return-to")]
        public string ReturnAddress { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.Add("class", "mdl-button mdl-js-button mdl-button--raised mdl-js-ripple-effect mdl-button--accent");
            output.Attributes.Add("type", "button");
            output.Attributes.Add("onclick", "window.location='" + ReturnAddress + "'");
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetContent(Text);
        }
    }
}
