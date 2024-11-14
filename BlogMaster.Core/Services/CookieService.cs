using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;

namespace BlogMaster.Core.Services
{
    public class CookieService
    {
        private readonly IDataProtector _protector;

        public CookieService(IDataProtectionProvider dataProtectionProvider)
        {
            _protector = dataProtectionProvider.CreateProtector("CookieService.Protection");
        }

        public string? Protect(string? value)
        {
            if (value == null)
            {
                return null;
            }
            return _protector.Protect(value);
        }

        public string? Unprotect(string? value)
        {
            if (value == null)
            {
                return null;
            }
            return _protector.Unprotect(value);
        }
    }
}
