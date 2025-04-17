using Sii.DescargaFolio.Helper;
using Sii.DescargaFolio.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDescargaFolioService, DescargaFolioService>();
builder.Services.AddSingleton<SiiAuthenticator>();

builder
    .Services.AddHttpClient(
        "SII",
        c =>
        {
            c.BaseAddress = new Uri("https://palena.sii.cl");
        }
    )
    .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
    {
        IConfiguration config = serviceProvider.GetRequiredService<IConfiguration>();
        return DigitalCertLoader.LoadCertificateAsync(config).GetAwaiter().GetResult();
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Sii.DescargaFolio", Version = "v1" });
    c.EnableAnnotations();
});

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
