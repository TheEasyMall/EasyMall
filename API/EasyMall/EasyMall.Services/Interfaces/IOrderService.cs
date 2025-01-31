using EasyMall.Models.DTOs;
using MayNghien.Models.Response.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Services.Interfaces
{
    public interface IOrderService
    {
        Task<AppResponse<OrderDTO>> Create(OrderDTO request);
    }
}
