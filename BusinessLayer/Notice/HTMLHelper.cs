using BDA.DataModel;
using RazorEngineCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDA.BusinessLayer
{
    public static class Html
    {
        public class Table : HtmlBase, IDisposable
        {
            public Table(StringBuilder sb, string classAttributes = "", string id = "") : base(sb)
            {
                Append("<table  border=1");
                AddOptionalAttributes(classAttributes, id);
            }

            public void StartHead(string classAttributes = "", string id = "")
            {
                Append("<thead");
                AddOptionalAttributes(classAttributes, id);
            }

            public void EndHead()
            {
                Append("</thead>");
            }

            public void StartFoot(string classAttributes = "", string id = "")
            {
                Append("<tfoot");
                AddOptionalAttributes(classAttributes, id);
            }

            public void EndFoot()
            {
                Append("</tfoot>");
            }

            public void StartBody(string classAttributes = "", string id = "")
            {
                Append("<tbody");
                AddOptionalAttributes(classAttributes, id);
            }

            public void EndBody()
            {
                Append("</tbody>");
            }

            public void Dispose()
            {
                Append("</table>");
            }

            public Row AddRow(string classAttributes = "", string id = "")
            {
                return new Row(GetBuilder(), classAttributes, id);
            }
        }

        public class Row : HtmlBase, IDisposable
        {
            public Row(StringBuilder sb, string classAttributes = "", string id = "") : base(sb)
            {
                Append("<tr");
                AddOptionalAttributes(classAttributes, id);
            }
            public void Dispose()
            {
                Append("</tr>");
            }
            public void AddCell(string innerText, string classAttributes = "", string id = "", string colSpan = "",string width ="")
            {
                Append("<td width=\"100px\"");
                AddOptionalAttributes(classAttributes, id, colSpan,width);
                Append(innerText);
                Append("</td>");
            }
        }

        public abstract class HtmlBase
        {
            private StringBuilder _sb;

            protected HtmlBase(StringBuilder sb)
            {
                _sb = sb;
            }

            public StringBuilder GetBuilder()
            {
                return _sb;
            }

            protected void Append(string toAppend)
            {
                _sb.Append(toAppend);
            }

            protected void AddOptionalAttributes(string className = "", string id = "", string colSpan = "", string width = "")
            {

                if (String.IsNullOrEmpty(id)==true)
                {
                    _sb.Append($" id=\"{id}\"");
                }
                if (String.IsNullOrEmpty(className) == true)
                {
                    _sb.Append($" class=\"{className}\"");
                }
                if (String.IsNullOrEmpty(colSpan) == true)
                {
                    _sb.Append($" colspan=\"{colSpan}\"");
                }
                if (String.IsNullOrEmpty(width) == true)
                {
                    _sb.Append($" width=\"{width}\"");
                }
                _sb.Append(">");
            }
        }
    }

}
