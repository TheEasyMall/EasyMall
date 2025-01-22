using EasyMall.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Models.DTOs.Auth.Responses
{
    public class LogInResponse
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
