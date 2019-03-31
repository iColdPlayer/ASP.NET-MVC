using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNet.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace AspNet // global namespace berdasarkan nama project kita.
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

            // adding method pada asp.net core
            services.AddMvc();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));


            // Menggunakan identity default .NET Core untuk full controll
            services.AddIdentity<IdentityUser, IdentityRole>()
            //services.AddDefaultIdentity<IdentityUser>()            
                .AddDefaultUI(UIFramework.Bootstrap4)
                //.AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
                
            


            // Konfigurasi Untuk Identitas User
            services.Configure<IdentityOptions>(options =>
            {
                //Berikut ini adalah password setting authentications untuk user yang akan register
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0; // there must be integer


                //mengatur Logout session
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                // maksimal user melakukan kesalahan saat akan login
                options.Lockout.MaxFailedAccessAttempts = 5;
                // memperbolehkan user baru untuk login setelah user lama logout 
                options.Lockout.AllowedForNewUsers = true;

                //User Settings
                // karakter yang diperbolehkan untuk digunakan oleh user
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-@.";
                // email yang diperbolehkan digunakan oleh user
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options => 
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });


            // setting untuk UI Default .Net Core
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddRazorPagesOptions(options => 
                {
                    options.AllowAreas = true;
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });
        }
        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // untuk mengidentifikasikan user 
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
