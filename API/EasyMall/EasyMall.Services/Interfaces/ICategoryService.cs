using EasyMall.Models.DTOs;
using MayNghien.Models.Response.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<AppResponse<CategoryDTO>> Create(CategoryDTO request);
        AppResponse<CategoryDTO> Update(CategoryDTO request);
        AppResponse<CategoryDTO> Delete(Guid id);
    }
}
