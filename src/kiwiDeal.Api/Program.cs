using Asp.Versioning;
using FluentValidation;
using kiwiDeal.Api.Hubs;
using kiwiDeal.Api.Infrastructure;
using kiwiDeal.Auctions.Api;
using kiwiDeal.Auctions.Application.Commands;
using kiwiDeal.Listings.Api;
using kiwiDeal.SharedKernel.Behaviours;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Outbox;
using kiwiDeal.Users.Api;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<IAuctionHubContext, AuctionHubContext>();

builder.Services.AddValidatorsFromAssembly(
    typeof(kiwiDeal.Users.Application.Commands.RegisterCommand).Assembly);
builder.Services.AddValidatorsFromAssembly(
    typeof(kiwiDeal.Listings.Application.Commands.CreateListingCommand).Assembly);
builder.Services.AddValidatorsFromAssembly(
    typeof(kiwiDeal.Auctions.Application.Commands.CreateAuctionCommand).Assembly);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(kiwiDeal.Users.Application.Commands.RegisterCommand).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(kiwiDeal.Listings.Application.Commands.CreateListingCommand).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(kiwiDeal.Auctions.Application.Commands.CreateAuctionCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthenticationBehaviour<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSecret = builder.Configuration["JwtSettings:Secret"]!;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        NameClaimType = "sub"
    };
});

builder.Services.AddAuthorization();
builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddListingsModule(builder.Configuration);
builder.Services.AddAuctionsModule(builder.Configuration);
builder.Services.AddHostedService<OutboxWorker>();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("kiwiDeal API");
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<AuctionHub>("/hubs/auctions");
app.MapHealthChecks("/health");
app.Run();

public partial class Program { }
