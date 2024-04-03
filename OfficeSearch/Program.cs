using OfficeSearch.Services.Indexing;
using OfficeSearch.Services.Querying;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

builder.Services.AddScoped<IIndexBuilder, IndexBuilder>();
builder.Services.AddScoped<IQueryService, QueryService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();