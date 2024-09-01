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

         

        //public 

        //public void UnsubscribeUser();
        //public bool IsSubscribed(string subscriptionId);
    }
}
