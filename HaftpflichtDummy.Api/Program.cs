using HaftpflichtDummy.Api;
using HaftpflichtDummy.BusinessLogic.Services.WebApiServices;
using HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;
using HaftpflichtDummy.DataProviders.Repositories.Database;
using HaftpflichtDummy.DataProviders.Repositories.Database.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config => { config.EnableAnnotations(); });
builder.Services.AddSingleton<FakeDb>();
builder.Services.AddScoped<IInsurer, Insurer>();
builder.Services.AddScoped<ITariff, Tariff>();
builder.Services.AddScoped<IInsurerService, InsurerService>();
builder.Services.AddScoped<ITariffService, TariffService>();
builder.Services.AddSingleton<PayloadService>();
builder.Services.AddLogging();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

DummyData.LoadDummyData(app);
app.Run();