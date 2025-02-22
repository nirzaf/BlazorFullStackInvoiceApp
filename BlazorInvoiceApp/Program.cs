using AutoMapper;
using BlazorInvoiceApp.Areas.Identity;
using BlazorInvoiceApp.Data;
using BlazorInvoiceApp.Repository;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace BlazorInvoiceApp;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        string connectionString
            = builder.Configuration.GetConnectionString("DefaultConnection") ??
              throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString), ServiceLifetime.Transient);
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services
            .AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
        builder.Services.AddScoped<DialogService>();
        builder.Services.AddTransient<IRepositoryCollection, RepositoryCollection>();

        MapperConfiguration mapperConfig = new MapperConfiguration(mc => { mc.AddProfile(new AutoMapperProfile()); });

        IMapper mapper = mapperConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        app.Run();
    }
}