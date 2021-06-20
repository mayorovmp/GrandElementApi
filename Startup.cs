using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;
using GrandElementApi.Interfaces;
using GrandElementApi.Middlewares;
using GrandElementApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GrandElementApi.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace GrandElementApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var conn = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            services.AddDbContext<ApplicationContext>(op => op.UseNpgsql(conn));
            services.AddAutoMapper(typeof(Startup));
            services.AddCors();
            services.AddSingleton<ISupplierService, SupplierService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ProductService, ProductService>();
            services.AddSingleton<ClientService, ClientService>();
            services.AddSingleton<CarService, CarService>();
            services.AddSingleton<RequestService, RequestService>();
            services.AddSingleton<RequestStatusService, RequestStatusService>();
            services.AddSingleton<IConnectionService, ConnectionService>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Grand Element API", Version = "v1", Description = "Put-update data, Post-create data, Get-retrieve data, Delete-remove one element." });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() { In = ParameterLocation.Header, Name = "Authorization", Type = SecuritySchemeType.ApiKey });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Authorization",
                                In = ParameterLocation.Header,

                            },
                            new List<string>()
                        }
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c=>
            {
                if (!env.IsDevelopment())
                {
                    c.PreSerializeFilters.Add((swagger, httpReq) =>
                    {
                        swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"https://{httpReq.Host.Value}/api" } };
                    });
                }
            });
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                if (env.IsDevelopment())
                {
                    c.SwaggerEndpoint( "/swagger/v1/swagger.json", "Grand Element API V1");
                }
                else
                {
                    c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Grand Element API V1");
                    // c.RoutePrefix = "api/swagger";
                }
            });
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            // Check autorization
            app.UseMiddleware<TokenMiddleware>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            //app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
