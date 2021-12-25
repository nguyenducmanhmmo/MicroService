using CustomerApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApi.Messaging.Send.Sender
{
    public interface ICustomerUpdateSender
    {
        void SendCustomer(Customer customer);
    }
}
