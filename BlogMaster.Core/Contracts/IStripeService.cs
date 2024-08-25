using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogMaster.Core.Models;
using Stripe.Checkout;

namespace BlogMaster.Core.Contracts
{
    public interface IStripeService
    {
        public Session StartSessionForEmbededForm(GetFormRequestDto getFormRequestDto);

        public string? GetPublishableKey();        
    }
}
