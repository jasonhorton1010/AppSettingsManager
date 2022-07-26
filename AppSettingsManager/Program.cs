using AppSettingsManager.Models;
using Microsoft.Extensions.Options; 

var builder = WebApplication.CreateBuilder(args);

// The block statement below is replicating the default hierarchy of where .NET Core looks for app settings.
// You could change the order but probably not ever required or recommended.
builder.Host.ConfigureAppConfiguration((context, appConfiguration) =>
{
    appConfiguration.Sources.Clear();
    appConfiguration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    appConfiguration.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
    if (context.HostingEnvironment.IsDevelopment())
    {
        appConfiguration.AddUserSecrets<Program>();
    }
    appConfiguration.AddEnvironmentVariables();
    appConfiguration.AddCommandLine(args);
}
);

//This technique is used for creating an extension method to provide a class TwilioSettings of AppSettings values
//builder.Configuration.AddConfiguration<TwilioSettings>(builder.Configuration, "Twilio");
//Need to figure out why the below line does not work!
//builder.Services.AddConfiguration<TwilioSettings>(builder.Configuration, "Twilio");


builder.Services.Configure<SocialLoginSettings>(builder.Configuration.GetSection("SocialLoginSettings"));

// This is used for the IOptions technique - see CTOR in the Home Controller.
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));

//  Technique to BIND the TwilioSettings class from the Program.cs
var twilioSettings = new TwilioSettings();
new ConfigureFromConfigurationOptions<TwilioSettings>(builder.Configuration.GetSection("Twilio")).Configure(twilioSettings);
builder.Services.AddSingleton(twilioSettings);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
