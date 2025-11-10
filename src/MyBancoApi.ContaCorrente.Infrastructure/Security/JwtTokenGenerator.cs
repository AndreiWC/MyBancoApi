using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyBancoApi.ContaCorrente.Application.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyBancoApi.ContaCorrente.Infrastructure.Security
{
    // Implementação do serviço de geração de token
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GerarToken(string idContaCorrente)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            // Pega a chave secreta do appsettings.json
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            // Define as "Claims" (informações) que irão dentro do token
            var claims = new ClaimsIdentity(new Claim[]
            {
                // Requisito: "contendo a identificação da conta corrente"
                new Claim(JwtRegisteredClaimNames.Sub, idContaCorrente),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddHours(2), // Token expira em 2 horas
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}