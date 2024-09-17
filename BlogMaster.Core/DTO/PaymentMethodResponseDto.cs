using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class PaymentMethodResponseDto
    {
        public string? PaymentMethodId { get; set; }
        public string? Last4 { get; set; }
        public long ExtMonth { get; set; }
        public long ExtYear { get; set; }
        public string? Brand { get; set; }

    }
}
