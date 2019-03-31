Tutorial membuat login dengan database menggunakan ASP.NET Core


Pada tutorial kali ini kita akan belajar membuat login page dengan ASP.NET Core menggunakan database SQLite3.

Langkah pertama:
Pastikan .NET Core sudah terintstall pada device kita.

#### Creating Your First Project

Langkah kedua:
Buatlah project baru dengan perintah seperti berikut:
```
$ dotnet new webapp --auth Individual -o NamaProject
```
Maka .NET Core akan membuat project baru.

Langkah selanjutnya adalah update database dengan perintah:

```
$ dotnet ef database update
```
Maka .NET Core akan melakukan update pada database yang akan kita gunakan seperti berikut:
```
icoldplayer@icoldplayer:~/Documents/.net/AspNet$ dotnet ef database update
info: Microsoft.EntityFrameworkCore.Infrastructure[10403]
      Entity Framework Core 2.2.3-servicing-35854 initialized 'ApplicationDbContext' using provider 'Microsoft.EntityFrameworkCore.Sqlite' with options: None
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (17ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      PRAGMA foreign_keys=ON;
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      PRAGMA foreign_keys=ON;
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (6ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT COUNT(*) FROM "sqlite_master" WHERE "name" = '__EFMigrationsHistory' AND "type" = 'table';
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      PRAGMA foreign_keys=ON;
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      PRAGMA foreign_keys=ON;
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT COUNT(*) FROM "sqlite_master" WHERE "name" = '__EFMigrationsHistory' AND "type" = 'table';
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      PRAGMA foreign_keys=ON;
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (2ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT "MigrationId", "ProductVersion"
      FROM "__EFMigrationsHistory"
      ORDER BY "MigrationId";
info: Microsoft.EntityFrameworkCore.Migrations[20405]
      No migrations were applied. The database is already up to date.
No migrations were applied. The database is already up to date.
Done.
```
Jalankan aplikasinya untuk memastikan kita telah  membuatnya dengan benar dengan perintah:
```
$ dotnet run
```

Apabila kita berhasil membuatnya tanpa kendala, maka akan muncul seperti di bawah ini:

```
icoldplayer@icoldplayer:~/Documents/.net/AspNet$ dotnet run
: Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager[0]
      User profile is available. Using '/home/icoldplayer/.aspnet/DataProtection-Keys' as key repository; keys will not be encrypted at rest.
Hosting environment: Development
Content root path: /home/icoldplayer/Documents/.net/AspNet
Now listening on: https://localhost:5001
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```
Arahkan url di atas ke browser kita, maka akan muncul default page ASP.NET di sana.

[!images](/sss.png)

Seperti yang kita lihat, pada pojok kanan atas sudah tersedia pilihan Register dan Login di sana.
Amazing bukan?.

Langkah selanjutnya adalah melakukan configurasi pada indentitas servis.

#### Configure Identity Services

Setelah kita berhasil membuat aplikasi di atas, kita sudah bisa melakukan login dan register.
Akan tetapi login dan register tersebut adalah default atau bawaan dari .NET Core itu sendiri.

Sekarang mari kita melakukan sedikit customisasi sehingga saat user melakukan register, mereka bebas dapat menggunakan karakter apapun. Tidak seperti defaultnya yang membuat kita sedikit annoying bukan? karena harus include semua karakter.

pada root folder, pastekan kode di bawah ini pada file `Startup.cs`.

```C#
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

namespace AspNet
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();


            // Konfigurasi Untuk Identitas User
            services.Configure<IdentityOptions>(options =>
            {
                //Berikut ini adalah password setting authentications untuk user yang akan register
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
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
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";
                // email yang diperbolehkan digunakan oleh user
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options => 
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                
                options.LoginPath = "Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });



            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
```
Berdasarkan source code di atas, kita dapat menentukan authentication yang akan digunakan oleh user semau kita.


Untuk dokumentasi lebih lanjut silahkan kunjungi dokumentasi dari [Official .NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-2.2&tabs=netcore-cli#scaffold-register-login-and-logout)

#### Melakukan Scaffold untuk Register, Login, dan Logout.

Selanjutnya, lakukan generate lagi untuk project kita, berdasarkan nama project masing-masing.

Follow this command!.
```
$ dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
```
Output:

```
icoldplayer@icoldplayer:~/Documents/.net/AspNet$ dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
  Writing /tmp/tmpSISgkt.tmp
info : Adding PackageReference for package 'Microsoft.VisualStudio.Web.CodeGeneration.Design' into project '/home/icoldplayer/Documents/.net/AspNet/AspNet.csproj'.
log  : Restoring packages for /home/icoldplayer/Documents/.net/AspNet/AspNet.csproj...
info :   GET https://api.nuget.org/v3-flatcontainer/microsoft.visualstudio.web.codegeneration.design/index.json
info :   OK https://api.nuget.org/v3-flatcontainer/microsoft.visualstudio.web.codegeneration.design/index.json 1127ms
info : Package 'Microsoft.VisualStudio.Web.CodeGeneration.Design' is compatible with all the specified frameworks in project '/home/icoldplayer/Documents/.net/AspNet/AspNet.csproj'.
info : PackageReference for package 'Microsoft.VisualStudio.Web.CodeGeneration.Design' version '2.2.3' added to file '/home/icoldplayer/Documents/.net/AspNet/AspNet.csproj'.
info : Committing restore...
log  : Generating MSBuild file /home/icoldplayer/Documents/.net/AspNet/obj/AspNet.csproj.nuget.g.props.
info : Writing lock file to disk. Path: /home/icoldplayer/Documents/.net/AspNet/obj/project.assets.json
log  : Restore completed in 15.83 sec for /home/icoldplayer/Documents/.net/AspNet/AspNet.csproj.
```

Setelah berhasil, lakukan generate selanjutnya untuk generate.
and then follow this command:

```
$ dotnet aspnet-codegenerator identity -dc AspNet.Data.ApplicationDbContext --files "Account.Register;Account.Login;Account.Logout"
```

Jika berhasil, the output should looks like:
```
icoldplayer@icoldplayer:~/Documents/.net/AspNet$ dotnet aspnet-codegenerator identity -dc AspNet.Data.ApplicationDbContext --files "Account.Register;Account.Login;Account.Logout"

Building project ...
Finding the generator 'identity'...
Running the generator 'identity'...
RunTime 00:00:23.51
```
Apabila proses generate di atas berjalan lancar, maka akan kita akan memiliki file `ScaffoldingReadme.txt` pada directory root kita.
[!imgs](/asdp.png)

See? and we're done here.

Jika kita ingin mencega anonymous akses pada halaman tertentu, kita bisa menggunakan `Authorizations`.

Contoh:

Edit file Privacy.cshtml.cs, lalu ikuti contoh di bawah ini.
```C#
// import authorize untuk mencegah anonymous access / user yang tidak login
using Microsoft.AspNetCore.Authorization;

namespace AspNet.Pages
{
    //mencegah akses kepada user yang tidak login untuk mengakses page yang membutuhkan authorize
    [Authorize]
    public class PrivacyModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
```

Jika berhasil, lakukan test kembali. pastikan kita telah logout dari aplikasi tersebut, laku klik link *privacy* pada halaman home. Apabila kita belum login, maka kita tidak diizinkan untuk mengakses halaman tersebut.

That's it guys!. Thank's for reading, jika ada kekeliruan dalam penulisan maupun pertanyaan, silahkan tinggalkan opini kalian di kolom komentar di bawah. 

