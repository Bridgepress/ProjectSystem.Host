using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using ProjectSystem.Core.Extensions;
using ProjectSystem.DataAccess;
using ProjectSystem.Api.Filters;
using ProjectSystem.Api;
using ProjectSystem.Repositories.Implementation;
using ProjectSystem.Services.Implementation;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.MSSqlServer(
        builder.Configuration.GetConnectionString(ApplicationDbContext.ConnectionStringKey),
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "Logs",
            SchemaName = "dbo",
            AutoCreateSqlTable = true
        },
        restrictedToMinimumLevel: Enum.Parse<LogEventLevel>(
            builder.Configuration["Logging:LogLevel:Default"] ??
            throw new Exception("Cannot find 'Logging:LogLevel:Default'")))
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Services.AddControllers();
builder.Services.AddInstallersFromAssemblies(builder.Configuration,
    typeof(ApplicationDbContext), typeof(ApiAssemblyMarker),
    typeof(RepositoryManager), typeof(ServiceAssemblyMarker));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddSingleton(Log.Logger);
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Logging.AddSerilog();

var app = builder.Build();
app.UseMiddleware<GlobalErrorHandlingMiddleware>();
app.UseCors("ProjectSystem_FrontEnd");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
