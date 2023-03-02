using Application.DTOs.Users;
using Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IAccountServices
    {
        Task<Response<AuthenticationResponse>> AutenticateAsync(AuthenticationRequest request,string IpAdress);
        Task<Response<string>> RegisterAsync(RegisterRequest request, string origin);
    }
}
