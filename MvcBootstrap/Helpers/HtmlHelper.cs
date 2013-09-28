using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcBootstrap.Helpers
{
    public class HtmlHelpers
    {
        public static Dictionary<string, object> LabelAttributes()
        {
            return new Dictionary<string, object>
            {
                { "class", "control-label" }
            };
        }

        public static Dictionary<string, object> TextBoxAttributes(string placeholder)
        {
            return new Dictionary<string, object>
            {
                { "class", "input-xxlarge" },
                { "placeholder", placeholder },
                { "x-webkit-speech", "x-webkit-speech" }
            };
        }
    }
}