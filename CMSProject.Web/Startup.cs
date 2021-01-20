using CMSProject.Data.Context;
using CMSProject.Data.Repositories.Concrete.EntityTypeRepositories;
using CMSProject.Data.Repositories.Interfaces.EntityTypeRepositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMSProject.Web
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
            services.AddControllersWithViews();

            //Proje i�erisinde; birbiri ile ba�lant�l� olan s�n�flar�m� kendi i�lerinde inject ediyorum m�teakiben IOC container'a register etmek i�in startup.cs i�erisinde bulunan ConfigureServices methodunun i�erisine ekliyorum. IOC container'da bana ba��ml�l�klar� tersine �evirmeyi temin ediyor. Bu y�ntem s�n�flar�m�n s�k� s�k�ya ba��ml�l���n� ortadan kald�rmak amac�yla yap�lm��t�r.

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IPageRepository, PageRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();


          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                
            });
        }
    }
}
