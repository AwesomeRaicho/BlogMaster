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
        public Task<SessionReturnDto> StartSessionForEmbededFormSubscription(GetFormRequestDto getFormRequestDto);


        // Customer methods

        public Customer CreateStripeCustomer(string userName, string userEmail);
        public void CancelSubscription(string subscriptionId);
        public string? GetPublishableKey();

        public Task<Subscription> GetCustomerSubscription(string customerId);

        public Task<string?> SubscriptionStatus(string customerId);
        public bool ResumeSubscription(string subscriptionId);

        public StripeList<PaymentMethod> StripePaymentMethods(string stripeCustomerId);

        public void RemovePaymentMethod(string paymentMethodId);

        public void ChangeDefaultPaymentMethod(string customerId, string paymentMethodId, string subscriptionId);
    }
}
