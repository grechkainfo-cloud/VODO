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
        // ��������� ������������ ����������� ����������� �������� ��������� �����
        opts.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;

        // ���������� ����������� ������ ��� ������������ (������������� JsonException � ������)
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        // ����������� ����������� ���������� ������� (�� ��������� 32) � ���� ������� ����� �������
        opts.JsonSerializerOptions.MaxDepth = 64;

        // �� ����������� null-�������� (�����������)
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        
        // ��������� ��������� ��� Geometry
        opts.JsonSerializerOptions.Converters.Add(new GeometrySystemTextJsonConverter());
    })
    */
    .AddNewtonsoftJson(opts =>
    {
        // ���������� �����
        opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        // ��������� ��������� ��� NetTopologySuite ���������
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
