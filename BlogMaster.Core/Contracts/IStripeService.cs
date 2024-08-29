using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogMaster.Core.DTO;
using BlogMaster.Core.Models;
using Stripe;
using Stripe.Checkout;

namespace BlogMaster.Core.Contracts
{
    public interface IStripeService
    {
        public Task<Session> StartSessionForEmbededFormDonation(GetFormRequestDto getFormRequestDto);


        public Session StartSessionForEmbededFormSubscription(GetFormRequestDto getFormRequestDto);


        // Customer methods
        public Customer CreateStripeCustomer(AppSubscription appSubscription);


        public void CancelSubscription(string subId);
        public string? GetPublishableKey();

    }
}
