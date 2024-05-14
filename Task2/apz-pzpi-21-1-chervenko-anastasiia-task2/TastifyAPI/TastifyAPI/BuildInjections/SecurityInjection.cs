﻿using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace TastifyAPI.BuildInjections
{
    public static class SecurityInjection
    {
        internal static void AddSetSecurity(this IServiceCollection services, IConfiguration config)
        {
            var securityKey = config["Jwt:Key"];
#pragma warning disable CS8604 // Possible null reference argument.
            var key = Encoding.ASCII.GetBytes(securityKey);
#pragma warning restore CS8604 // Possible null reference argument.

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });
        }
    }
}