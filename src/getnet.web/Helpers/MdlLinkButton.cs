using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace getnet.Helpers
{
    [HtmlTargetElement("button", Attributes = "mdl-link-button, mdl-href")]
    public class MdlLinkButton : TagHelper
    {
        [HtmlAttributeName("mdl-link-button")]
        public string Text { get; set; }

        [HtmlAttributeName("mdl-href")]
        public string Link { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.Add("class", "mdl-button mdl-js-button mdl-button--raised mdl-js-ripple-effect mdl-button--colored");
            output.Attributes.Add("type", "button");
            output.Attributes.Add("onclick", "window.location = '" + Link + "'");
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetContent(Text);
        }
    }
}
