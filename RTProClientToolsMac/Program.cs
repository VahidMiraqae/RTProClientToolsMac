using RTProClientToolsMac.Services;
using Serilog;

var logger = new LoggerConfiguration()
            .MinimumLevel.Error()
            .WriteTo.File("logs/errors/errors.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{NewLine}__$__{NewLine}{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

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
builder.Services.AddSerilog(logger);

var app = builder.Build();
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

