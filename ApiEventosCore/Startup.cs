﻿using ApiEventosCore.Data;
using ApiEventosCore.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace ApiEventosCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>( 
               _ => Configuration);
            services.AddTransient<EventoDapperDAO>();

            EventosDataContext.ConnectionString = Configuration.GetConnectionString("BaseEventos");

            //services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy("Policy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });



            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                ValidateIssuer = true,
                ValidIssuer = "ExampleIssuer",

                ValidateAudience = true,
                ValidAudience = "ExampleAudience",
                
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthorization(t =>
            {
                t.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            }
            );
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(optionsjwtB => {
                optionsjwtB.TokenValidationParameters = tokenValidationParameters;
            }).AddCookie(x => {
                x.ExpireTimeSpan = TimeSpan.FromDays(105);
                x.LoginPath = "/Account/Login";
                x.LogoutPath = "/Account/Logout";
            });                    

            services.AddMvc();

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            //});

        }

        private static readonly string secretKey = "apieventos_secretkey!123";
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Evento API V1");
            //});

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var options = new TokenProviderOptions
            {
                Audience = "ExampleAudience",
                Issuer = "ExampleIssuer",
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            };

            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));

            app.UseMvc();
        }
    }
}
