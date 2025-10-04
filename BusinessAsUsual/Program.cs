// <copyright file="Program.cs" company="BusinessAsUsual">
// Copyright (c) BusinessAsUsual. All rights reserved.
// </copyright>

namespace BusinessAsUsual
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using MudBlazor.Services;

    /// <summary>
    /// The entry point for the BusinessAsUsual web application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Configures and runs the web application.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services);

            var app = builder.Build();

            ConfigureMiddleware(app);

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }

        /// <summary>
        /// Registers services required by the application.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddMudServices();
        }

        /// <summary>
        /// Configures the middleware pipeline for the application.
        /// </summary>
        /// <param name="app">The application builder instance.</param>
        private static void ConfigureMiddleware(WebApplication app)
        {
            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }
    }
}