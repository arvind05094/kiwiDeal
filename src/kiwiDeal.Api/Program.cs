using Asp.Versioning;
using FluentValidation;
using kiwiDeal.Api.Hubs;
using kiwiDeal.Api.Infrastructure;
using kiwiDeal.Auctions.Api;
using kiwiDeal.Auctions.Application.Commands;
using kiwiDeal.Listings.Api;
using kiwiDeal.Listings.Application.Events;
using kiwiDeal.Messages.Api;
using kiwiDeal.Payments.Api;
using kiwiDeal.SharedKernel.Behaviours;
using kiwiDeal.SharedKernel.Contracts;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Outbox;
using kiwiDeal.Users.Api;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
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
builder.Services.AddScoped<IListingHubContext, ListingHubContext>();

builder.Services.AddValidatorsFromAssembly(
    typeof(kiwiDeal.Users.Application.Commands.RegisterCommand).Assembly);
builder.Services.AddValidatorsFromAssembly(
    typeof(kiwiDeal.Listings.Application.Commands.CreateListingCommand).Assembly);
builder.Services.AddValidatorsFromAssembly(
    typeof(kiwiDeal.Auctions.Application.Commands.CreateAuctionCommand).Assembly);
builder.Services.AddValidatorsFromAssembly(
    typeof(kiwiDeal.Payments.Application.Commands.CreateCheckoutSession.CreateCheckoutSessionCommand).Assembly);
builder.Services.AddValidatorsFromAssembly(
    typeof(kiwiDeal.Messages.Application.Commands.StartConversationCommand).Assembly);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(kiwiDeal.Users.Application.Commands.RegisterCommand).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(kiwiDeal.Listings.Application.Commands.CreateListingCommand).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(kiwiDeal.Auctions.Application.Commands.CreateAuctionCommand).Assembly);

    cfg.RegisterServicesFromAssembly(
    typeof(kiwiDeal.Payments.Application.Commands.CreateCheckoutSession.CreateCheckoutSessionCommand).Assembly);
    cfg.RegisterServicesFromAssembly(
    typeof(kiwiDeal.Messages.Application.Commands.StartConversationCommand).Assembly);
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
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                context.Token = accessToken;
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var registry = new OutboxTypeRegistry();
registry.Register<AuctionClosedEvent>();
registry.Register<AuctionCreatedEvent>();
registry.Register<BidPlacedEvent>();
registry.Register<ListingCreatedEvent>();
registry.Register<ListingCancelledEvent>();
registry.Register<ListingSoldEvent>();
registry.Register<PaymentCompletedEvent>();
registry.Register<PaymentFailedEvent>();


builder.Services.AddSingleton<IOutboxTypeRegistry>(registry);

builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddListingsModule(builder.Configuration);
builder.Services.AddAuctionsModule(builder.Configuration);
builder.Services.AddPaymentsModule(builder.Configuration);
builder.Services.AddMessagesModule(builder.Configuration);
builder.Services.AddHostedService<OutboxWorker>();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(",")
            ?? ["http://localhost:5173"];
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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
app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();
app.MapHub<AuctionHub>("/hubs/auctions");
app.MapHub<ListingHub>("/hubs/listings");
app.MapHub<MessageHub>("/hubs/messages");
app.MapHealthChecks("/health");
app.Run();

public partial class Program { }
