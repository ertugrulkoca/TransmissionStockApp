using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using TransmissionStockApp.Data;
using TransmissionStockApp.Mapping;
using TransmissionStockApp.Repositories;
using TransmissionStockApp.Services;
using TransmissionStockApp.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Controllers & Views
builder.Services.AddControllersWithViews();

// DbContext: Pooling + Retry
builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(cs, sql =>
    {
        sql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
        sql.CommandTimeout(60);
    });
});

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddScoped<ITransmissionStockService, TransmissionStockService>();
builder.Services.AddScoped<ILookupService, LookupService>();
builder.Services.AddScoped<ITransmissionStockRepository, TransmissionStockRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ITransmissionBrandService, TransmissionBrandService>();
builder.Services.AddScoped<ITransmissionStatusService, TransmissionStatusService>();
builder.Services.AddScoped<ITransmissionDriveTypeService, TransmissionDriveTypeService>();
builder.Services.AddScoped<IStockLocationService, StockLocationService>();
builder.Services.AddScoped<ITransmissionStockLocationService, TransmissionStockLocationService>();
builder.Services.AddScoped<IVehicleBrandService, VehicleBrandService>();
builder.Services.AddScoped<IVehicleModelService, VehicleModelService>();

// Swagger sadece Dev
builder.Services.AddEndpointsApiExplorer();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

// Reverse proxy/IIS ise gerçek client IP ve HTTPS bilgisini al
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Prod: hata sayfasý + HSTS + HTTPS yönlendirme
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();                 // Strict-Transport-Security
    app.UseHttpsRedirection();     // 80 -> 443
}
else
{
    // Dev: Swagger açýk
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Statik dosyalar
app.UseStaticFiles();

app.UseRouting();

// (Varsa) Kimlik doðrulama/Yetkilendirme
// app.UseAuthentication();
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// (Opsiyonel) Sadece geliþtirmede otomatik migration
// if (app.Environment.IsDevelopment())
// {
//     using var scope = app.Services.CreateScope();
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate();
// }

app.Run();
