using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BDA.Helper
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateSecureHiddenInputsAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] properties;
        IDataProtector _protector;
        public ValidateSecureHiddenInputsAttribute(string properties, IDataProtectionProvider provider = null)
        {
            if (properties == null || !properties.Any())
            {
                throw new ArgumentException("properties");
            }

            this.properties = properties.Split(","[0]);
            _protector = provider.CreateProtector("SecureHidden");
        }


        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            this.properties.ToList().ForEach(property => Validate(context, property));
        }

        private void Validate(AuthorizationFilterContext filterContext, string property)
        {
           
            var protectedValue = filterContext.HttpContext.Request.Form[string.Format("__{0}SecToken", property)];
            var decryptedValue = _protector.Unprotect(protectedValue);

            if (decryptedValue == null)
            {
                throw new HttpSecureHiddenInputException("A required security token was not supplied or was invalid.");
            }
            
            var originalValue = filterContext.HttpContext.Request.Form[property];
            var identity = filterContext.HttpContext.User.Identity;
            if (!string.IsNullOrEmpty(identity.Name))
            {
                originalValue = string.Format("{0}_{1}", identity.Name, originalValue);
            }

            if (!decryptedValue.Equals(originalValue))
            {
                throw new HttpSecureHiddenInputException("A required security token was not supplied or was invalid.");
            }
        }
    }

    public class HttpSecureHiddenInputException : Exception
    {
        public HttpSecureHiddenInputException(string message) : base(message)
        {
        }
    }

}
