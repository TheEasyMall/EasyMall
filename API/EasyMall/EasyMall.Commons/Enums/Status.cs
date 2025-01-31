using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Commons.Enums
{
    public enum Status
    {
        Cancelled = -1,
        Pending = 0,
        Processing = 1,
        Shipping = 2,
        Completed = 3
    }
}
