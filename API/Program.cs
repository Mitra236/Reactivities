using Application.Activities;

var builder = WebApplication.CreateBuilder(args);

//add services to container

builder.Services.AddControllers(opt => 
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
})
    .AddFluentValidation(config => 
{
    config.RegisterValidatorsFromAssemblyContaining<Create>();
});
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

// Configure the http request pipeline

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseXContentTypeOptions();
app.UseReferrerPolicy(opt => opt.NoReferrer());
app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
app.UseXfo(opt => opt.Deny());
app.UseCsp(opt => opt
    .BlockAllMixedContent()
    .StyleSources(s => s.Self()
        .CustomSources("fonts.googleapis.com", "sha256-yChqzBduCCi4o4xdbXRXh4U/t1rP4UUUMJt+rB+ylUI=", "sha256-r3x6D0yBZdyG8FpooR5ZxcsLuwuJ+pSQ/80YzwXS5IU="))
    .FontSources(s => s.Self().CustomSources("fonts.gstatic.com", "data:"))
    .FormActions(s => s.Self())
    .FrameAncestors(s => s.Self())
    .ImageSources(s => s.Self().CustomSources("res.cloudinary.com", "data:", "www.facebook.com", "z-p3-scontent.fbeg7-2.fna.fbcdn.net", "platform-lookaside.fbsbx.com"))
    .ScriptSources(s => s.Self()
        .CustomSources("sha256-Tb8OSe1AjOh69ILtODKm7nA8FSS8D+lyt9eq4jItLGs=", 
            "connect.facebook.net", "sha256-P/zfH4xSdYsd4Wwul8NqY2Z1n4DsygPf3gb/wWs+2es=")));
app.Use(async (context, next) => 
    {
        context.Response.Headers.Add("Permissions-Policy", new StringValues(
                                                                        "microphone=(), " +
                                                                        "payment=(), " +
                                                                        "sync-xhr=(self)"
                                                                        ));
        await next.Invoke();
    });

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}
else
{
    app.Use(async (context, next) => 
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
        await next.Invoke();
    });
}

// app.UseHttpsRedirection();

//for wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("CorsPolicy");

//authentication has to be before authorization
app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();
app.MapHub<ChatHub>("/chat");
app.MapFallbackToController("Index", "Fallback");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

using var scope = app.Services.CreateScope();

var services = scope.ServiceProvider;

try 
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await Seed.SeedData(context, userManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migraiton");
}

await app.RunAsync();
