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
        AppResponse<List<CategoryDTO>> GetByPresent();
        AppResponse<CategoryDTO> GetById(Guid id);
        Task<AppResponse<CategoryDTO>> Create(CategoryDTO request);
        AppResponse<CategoryDTO> Update(CategoryDTO request);
        AppResponse<string> Delete(Guid id);
    }
}
