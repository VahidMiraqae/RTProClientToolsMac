using ClientToolsMac.Services;
using RTProClientToolsMac.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder1 =>
    {
        builder1.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    }));

builder.Services.AddSingleton<Configurations>();
builder.Services.AddSingleton<PrintService>();
builder.Services.AddSingleton<PrintFileResolver>();
builder.Services.AddSingleton<PeriodicTaskRunner>();

var app = builder.Build();
_ = app.Services.GetRequiredService<PeriodicTaskRunner>().LoadTempFileSchedule();
app.UseCors("MyPolicy");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseHttpsRedirection();

app.Run();

