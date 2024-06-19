using Bitlog.WebApi.Dtos;
using Bitlog.WebApi.Entities;
using Bitlog.WebApi.Validators;
using Bitlog.WebAPI.Repositories;
using Bitlog.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var userValidator = new UserValidator();
userValidator
    .SetNext(new UserEmailPhoneValidator(new UserEmailValidator(), new UserPhoneValidator()))
    .SetNext(new UserPasswordValidator());

builder.Services.AddSingleton<IValidator<UserPayload>>(userValidator);
builder.Services.AddTransient<IUserRepository, UserRepository>();

builder.Services.AddTransient<IUserNotificationService, SmsService>();
builder.Services.AddTransient<IUserNotificationService, SmtpService>();

builder.Services.AddTransient<IUserNotificationServiceFactory, UserNotificationServiceFactory>();

builder.AddNpgsqlDbContext<UserDbContext>("Bitlog");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<UserDbContext>();
    try
    {
        context.Database.EnsureCreated();
    }
    catch (Exception _)
    {
        await Task.Delay(3000);
        context.Database.EnsureCreated();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/search", async (string? email, string? phoneNumber, IUserRepository userRepository) =>
{
    if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phoneNumber))
    {
        return Results.BadRequest(new Result<Error>(false, new Error("Atleast email or phonenumber must be provided.")));
    }

    var user = await userRepository.Search(email, User.GetFormattedPhoneNumber(phoneNumber));

    return user == null ? Results.NotFound(new Result<Error>(false, new Error("User not found")))
    : Results.Ok(new Result<UserDto>(true, new UserDto { Email = user.Email, PhoneNumber = user.PhoneNumber }));
})
.WithName("Search")
.WithOpenApi();

app.MapPost("/user", async (
    UserPayload userRequest, 
    IValidator<UserPayload> userValidator, 
    IUserRepository userRepository, 
    IUserNotificationServiceFactory userNotificationServiceFactory) =>
{
    var (valid, errorMessage) = userValidator.Validate(userRequest);

    if (valid is false)
    {
        return Results.BadRequest(new Result<Error>(false, new Error(errorMessage)));
    }

    var existingUser = await userRepository.Search(userRequest.Email, userRequest.PhoneNumber);

    if (existingUser is not null)
    {
        return Results.Conflict(new Result<Error>(false, new Error("User already exists.")));
    }

    var userEntity = User.CreateUser(userRequest.Email, userRequest.PhoneNumber, userRequest.Password);
    await userRepository.CreateUser(userEntity);

    // this can be enqueued to a background job.
    userNotificationServiceFactory.GetApplicableService(userEntity).Notify(userEntity);

    return Results.Ok(
        new Result<UserDto>(
            true,
            new UserDto
            {
                Email = userEntity.Email,
                PhoneNumber = userEntity.PhoneNumber
            }));
})
.WithName("Users")
.WithOpenApi();

app.Run();