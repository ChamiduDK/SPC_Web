var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

// 1. Add session services
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options =>
{
    // Optional: set session idle timeout
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddHttpClient();

var app = builder.Build();

// 2. Use session in the middleware pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); 

app.MapRazorPages();
app.Run();
