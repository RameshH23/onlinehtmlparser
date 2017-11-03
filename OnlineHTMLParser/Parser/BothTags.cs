using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineHTMLParser.Parser
{
    public class BothTags
    {
        public string[] tags = { "a", "script", "p", "img", "title", "html", "h1", "h2", "h3", "h4", "h5", "h6", "head", "li", "ul", "form", "input", "select", "textarea", "output", "Html.ActionLink", "Ajax.ActionLink", "Html.DisplayNameFor", "Html.BeginForm", "Html.TextBox", "Html.TextArea", "Html.CheckBox", "Html.RadioButton", "Html.DropDownList", "Html.ListBox", "Html.Hidden", "Html.PasswordFor", "Html.Editor" };
        public string[] tagscolour = { "A", "SCRIPT", "P", "IMG", "TITLE", "HTML", "H1", "H2", "H3", "H4", "H5", "H6", "HEAD" };
        public string[] attributes = { "class", "id", "title", "alt", "src", "href", "width", "height", "disabled", "target", "name", "data-tooltip", "placeholder" };
        public string[] conditons = { "<", "AND", "</", "MERGE", ">", "OR", "/>" };
    }
}