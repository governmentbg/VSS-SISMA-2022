using SISMA.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("hosting.json", true, true).AddEnvironmentVariables("ASPNETCORE_");

// Add services to the container.

builder.Services.AddControllers(options =>
    {
        options.RespectBrowserAcceptHeader = true;
    })
    .AddXmlSerializerFormatters()
    .AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationDbContext(builder.Configuration);


// За добавяне на услуги, използвайте extension метода!!!
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
