using Application.DTOs.Users;
using Application.Enums;
using Application.Exceptions;
using Application.Interface;
using Application.Wrappers;
using Domain.Settings;
using Identity.Helpers;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Services
{
    public class AccountService : IAccountServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWTSettings _jwtSettings;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IDateTimeService _datetimeService;
        public AccountService(UserManager<ApplicationUser> userManager, IOptions<JWTSettings> jwtSettings, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IDateTimeService datetimeService)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _datetimeService = datetimeService;
        }

        public async Task<Response<AuthenticationResponse>> AutenticateAsync(AuthenticationRequest request, string IpAdress)
        {
           var usuario = await _userManager.FindByEmailAsync(request.Email);
            if (usuario == null)
            {
                throw new ApiException($"El email {request.Email} no se encuentra registrado");
            }

            var result = await _signInManager.PasswordSignInAsync(usuario.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new ApiException($"Credenciales invalidas");
            }

            JwtSecurityToken jwtSecurityToken = await GJWToken(usuario);
            AuthenticationResponse response = new AuthenticationResponse();
            response.Id = usuario.Id;
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Email= usuario.Email;
            response.UserName=usuario.UserName;
            var roleList = await _userManager.GetRolesAsync(usuario).ConfigureAwait(false);
            response.Roles = roleList.ToList();
            response.IsVerified = usuario.EmailConfirmed;

            var refreshToken = GenerateRefreshToken(IpAdress);
            response.RefreshToken = refreshToken.Token;

            return new Response<AuthenticationResponse>(response, $"Autenticado {usuario.UserName}");

        }


        public async Task<Response<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            var usuarioConElMismoUserName = await _userManager.FindByNameAsync(request.UserName);
            if (usuarioConElMismoUserName !=null)
            {
                throw new ApiException($"Usuario {request.UserName} ya fue registrado previamente");
            }
            var user = new ApplicationUser()
            {
                Email = request.Email,
                Nombre= request.Nombre,
                Apellido = request.Apellido,
                UserName = request.UserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            var usuarioConElMismoCorreo = await _userManager.FindByEmailAsync(request.Email);
            if (usuarioConElMismoCorreo != null)
            {
                throw new ApiException($"El email {request.Email} ya fue registrado previamente");
            }
            else
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                     await _userManager.AddToRoleAsync(user, Roles.Basic.ToString());
                    return new Response<string>(user.Id, message: $"Usuario registrado exitosamente. {request.UserName}");
                }
                else
                {
                    throw new ApiException($"{result.Errors}");
                }
            }

        }

        private async Task<JwtSecurityToken> GJWToken(ApplicationUser usuario)
        {
            var userClaims = await _userManager.GetClaimsAsync(usuario);
            var roles = await _userManager.GetRolesAsync(usuario);
            var rolesClaimas = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                rolesClaimas.Add(new Claim("roles", roles[i]));
            }
            string ipAddress = IpHelper.GetIpAddress();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim("uid",usuario.Id),
                new Claim("ip",ipAddress),
            }.Union(userClaims).Union(rolesClaimas);

            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials
                );

            return jwtSecurityToken;
        }

        private RefreshToken GenerateRefreshToken(string ipAdress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now,
                CreatedByIp = ipAdress,
            };
        }


        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();

            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", ""); 
        }
    }
}
