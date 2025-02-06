﻿using EasyMall.DTO;
using MayNghien.Models.Response.Base;

namespace EasyMall.Services.Interfaces
{
    public interface IOrderService
    {
        Task<AppResponse<OrderDTO>> Create(OrderDTO request);
    }
}
