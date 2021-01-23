using CMSProject.Data.Context;
using CMSProject.Data.Repositories.Concrete.EntityTypeRepositories;
using CMSProject.Data.Repositories.Interfaces.EntityTypeRepositories;
using CMSProject.Entity.Entities.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
            services.AddMemoryCache(); //Github April 4, bir makalede gereksiz oldu�unu g�rd�m
            services.AddSession(opt =>
            {
                //opt.IdleTimeout = TimeSpan.FromDays(30);
                //opt.IdleTimeout = TimeSpan.FromSeconds(10);
            });
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllersWithViews();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IPageRepository, PageRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 3;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
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

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();//Session i�lemleri i�in gelen requesleri handler ederken bu middleware u�ramak zorunda

            app.UseAuthentication(); //Gelen requestler yetkilerden �nce sisteme aythentication olmak zorunda bu y�zden "UseAuthentication" middleware buraya ekledik.
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                // NOT : => DEFAULT �ZER�NDE YAPILACAK ��LEMLER ���N TEK TEK ENDPO�NT KULLANILMASI GEREK�YOR. FAKAT AREA ���N TEK B�R  HAMLE YETERL�D�R.


                endpoints.MapControllerRoute(
                     name: "page", //Name => Yolun Ad�
                     pattern: "{slug?}", // "slug?" nedir slug yan�nda id de ta��r... => pattern Bunun yaz�lmas�n�n sebebi methodlar�n yap�laca�� i�leme g�re URL belirlenmesidir.
                defaults: new { controller = "Page", action = "Page" });  // => 

                endpoints.MapControllerRoute(
                name: "product",
                pattern: "product/{categorySlug}",
                defaults: new { controller = "Product", action = "ProductByCategory" }); // => endpointleri methodlara y�nlendirmek i�in default kullan�l�r.

                endpoints.MapControllerRoute( // => default sayfalar i�in bu end pointi kulland�k
                name: "default", //=> yolun ad�
                pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute( //=> Area i�erisinde b�t�n controller �zerinde ki methodlar�n oldu�u View sayfas�n�n g�r�nt�lenmesi i�in sadece bu endpointi kullanmak yeterlidir.
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"); // => pattern adres g�sterir, e�er direk adres belirtiyorsa defaults a gerek yok. 
                // exist => kullan�ld���nda name i�erisinde yaz�l� olan b�t�n index ve methodlar �al���r.
                // sa� taraf�na yaz�lmas� gereken adreslerin "{controller=Home}/{action=Index}/{id?}" standart� belirtilmesi yeterlidir. 
            });
        }
    }
}

