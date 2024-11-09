using BlogMaster.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Contracts
{
    public interface IAppSubscriptionService
    {
        public Task<SubscriptionResponseDto> CreateSubscription(SubscriptionRequestDto subscriptionRequestDto);

        public Task<SubscriptionResponseDto> CancelAtEndOfCycle(SubscriptionRequestDto subscriptionRequestDto);

        public Task<SubscriptionResponseDto> ResumeSubscription(SubscriptionRequestDto subscriptionRequestDto);

        public Task<SubscriptionResponseDto> AddNewPaymentMethod(NewPaymentMethodDto newPaymentMethodDto);
        
        public Task<SubscriptionResponseDto?> GetSubscriptionDetails(SubscriptionRequestDto subscriptionRequestDto);

        public Task<List<PaymentMethodResponseDto>> GetAllPaymentMethods(SubscriptionRequestDto subscriptionRequestDto);

        public Task UpdateSuscription(SubscriptionRequestDto subscriptionRequestDto);

        public Task<bool> IsSubscriptionActive(SubscriptionRequestDto subscriptionRequestDto);

        public Task<SubscriptionResponseDto> SuccessfulPayment(SubscriptionRequestDto subscriptionRequestDto);
        
        //public Task ChangePaymentMethod(SubscriptionRequestDto subscriptionRequestDto);
        //public bool IsSubscribed(string subscriptionId);
    }
}
