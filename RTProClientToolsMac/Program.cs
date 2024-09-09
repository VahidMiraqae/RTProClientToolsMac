using RTProClientToolsMac.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Error()
            .WriteTo.File($"{nameof(RTProClientToolsMac)}/logs/errors/errors.txt", rollingInterval: RollingInterval.Day, retainedFileTimeLimit: TimeSpan.FromDays(30), outputTemplate: "{NewLine}__$__{NewLine}{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddPolicy("DefaultPolicy", builder1 =>
    {
        builder1.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    }));

builder.Services.AddSingleton<Configurations>();
builder.Services.AddSingleton<PrintService>();
builder.Services.AddSingleton<PrintFileResolver>();

var app = builder.Build();

app.UseExceptionHandler(_ => { });
app.UseCors("DefaultPolicy");
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();

