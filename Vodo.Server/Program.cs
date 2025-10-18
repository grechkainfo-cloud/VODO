using System.Text.Json.Serialization;
using Vodo.Application;
using Vodo.DAL;
using Vodo.Server.SystemTextJsonConverters;
using Vodo.Server.NewtonsoftConverters;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    /*
    .AddJsonOptions(opts =>
    {
        // Разрешаем сериализацию специальных именованных значений плавающей точки
        opts.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;

        // Игнорируем циклические ссылки при сериализации (предотвращает JsonException о циклах)
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        // Увеличиваем максимально допустимую глубину (по умолчанию 32) — если объекты очень вложены
        opts.JsonSerializerOptions.MaxDepth = 64;

        // Не сериализуем null-значения (опционально)
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        
        // Добавляем конвертер для Geometry
        opts.JsonSerializerOptions.Converters.Add(new GeometrySystemTextJsonConverter());
    })
    */
    .AddNewtonsoftJson(opts =>
    {
        // Игнорируем циклы
        opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        // Добавляем конвертер для NetTopologySuite геометрий
        opts.SerializerSettings.Converters.Add(new GeometryNewtonsoftConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddDAL(builder.Configuration);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
