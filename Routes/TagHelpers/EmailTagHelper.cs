using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routes.TagHelpers
{
    public class EmailTagHelper : TagHelper
    {
        public string Mail { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            output.Attributes.SetAttribute("href", "mailto:" + Mail);
            output.Content.SetContent(Mail);
        }
    }
}
