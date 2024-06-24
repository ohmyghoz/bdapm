using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BDA.Helper
{
   
    [HtmlTargetElement("label", Attributes = ForAttributeName)]
    public class LabelRequiredTagHelper : LabelTagHelper
    {
        private const string ForAttributeName = "asp-for";

        public LabelRequiredTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output);

            if (For.Metadata.IsRequired)
            {
                var sup = new TagBuilder("span");
                sup.AddCssClass("required");
                sup.InnerHtml.Append(" *");
                output.Content.AppendHtml(sup);
            }
        }
    }

    [HtmlTargetElement("securehidden")]
    public class SecureHiddenTagHelper : TagHelper
    {
        IDataProtector _protector;
        IHttpContextAccessor _httpContext;
        // the 'provider' parameter is provided by DI

        public SecureHiddenTagHelper(IDataProtectionProvider provider, IHttpContextAccessor httpContext)
        {
            _protector = provider.CreateProtector("SecureHidden");
            _httpContext = httpContext;
        }


        [HtmlAttributeName("asp-for")]
        public ModelExpression Source { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            //var contents = $@"
            //Model name: {Source.Metadata.ContainerType.FullName}<br/>
            //Property name: {Source.Name}<br/>
            //Current Value: {Source.Model}<br/> 
            //Is Required: {Source.Metadata.IsRequired}";

            var hid1 = new TagBuilder("input");
            hid1.Attributes.Add(new KeyValuePair<string, string>("id",Source.Name));
            hid1.Attributes.Add(new KeyValuePair<string, string>("type", "hidden"));
            hid1.Attributes.Add(new KeyValuePair<string, string>("name", Source.Name));
            hid1.Attributes.Add(new KeyValuePair<string, string>("value", Source.Model.ToString()));
            output.Content.AppendHtml(hid1);

            var hid2 = new TagBuilder("input");
            var identity = _httpContext.HttpContext.User.Identity;
            string originalValue = Source.Model.ToString();
            if (!string.IsNullOrEmpty(identity.Name))
            {
                originalValue = string.Format("{0}_{1}", identity.Name, originalValue);
            }

            string protectedPayload = _protector.Protect(originalValue);

            hid2.Attributes.Add(new KeyValuePair<string, string>("id", "__"+ Source.Name +"SecToken"));
            hid2.Attributes.Add(new KeyValuePair<string, string>("name", "__" + Source.Name + "SecToken"));
            hid2.Attributes.Add(new KeyValuePair<string, string>("type", "hidden"));            
            hid2.Attributes.Add(new KeyValuePair<string, string>("value", protectedPayload));
            output.Content.AppendHtml(hid2);
        }
    }


    [HtmlTargetElement("securehidden2")]
    public class SecureHidden2TagHelper : TagHelper
    {
        IDataProtector _protector;
        IHttpContextAccessor _httpContext;
        // the 'provider' parameter is provided by DI

        public SecureHidden2TagHelper(IDataProtectionProvider provider, IHttpContextAccessor httpContext)
        {
            _protector = provider.CreateProtector("SecureHidden");
            _httpContext = httpContext;
        }


        [HtmlAttributeName("name")]
        public string Name { get; set; }

        [HtmlAttributeName("value")]
        public string Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            
            var hid1 = new TagBuilder("input");
            hid1.Attributes.Add(new KeyValuePair<string, string>("id", Name));
            hid1.Attributes.Add(new KeyValuePair<string, string>("type", "hidden"));
            hid1.Attributes.Add(new KeyValuePair<string, string>("name", Name));
            hid1.Attributes.Add(new KeyValuePair<string, string>("value", Value));
            output.Content.AppendHtml(hid1);

            var hid2 = new TagBuilder("input");
            var identity = _httpContext.HttpContext.User.Identity;
            string originalValue = Value;
            if (!string.IsNullOrEmpty(identity.Name))
            {
                originalValue = string.Format("{0}_{1}", identity.Name, originalValue);
            }

            string protectedPayload = _protector.Protect(originalValue);

            hid2.Attributes.Add(new KeyValuePair<string, string>("id", "__" + Name + "SecToken"));
            hid2.Attributes.Add(new KeyValuePair<string, string>("name", "__" + Name + "SecToken"));
            hid2.Attributes.Add(new KeyValuePair<string, string>("type", "hidden"));
            hid2.Attributes.Add(new KeyValuePair<string, string>("value", protectedPayload));
            output.Content.AppendHtml(hid2);
        }
    }
}
