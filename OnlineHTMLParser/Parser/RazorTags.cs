using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineHTMLParser.Parser
{
    public class RazorTags
    {

        public string[] tags = { "Html.ActionLink", "Ajax.ActionLink", "Html.DisplayNameFor", "Html.BeginForm", "Html.TextBox", "Html.TextArea", "Html.CheckBox", "Html.RadioButton", "Html.DropDownList", "Html.ListBox", "Html.Hidden", "Html.PasswordFor", "Html.Editor" };
        public string[] tagscolour = { "qsdwqd"  };
        public string[] attributes = { "class", "id", "title", "alt", "src", "href", "width", "height", "disabled", "target", "name", "data-tooltip", "placeholder" };
        public string[] conditons = { "<", "AND", "</", "MERGE", ">", "OR", "/>" };
    }
}