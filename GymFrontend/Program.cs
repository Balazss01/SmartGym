namespace GymFrontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpClient("Api", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
            });
            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler =
                System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });
            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
