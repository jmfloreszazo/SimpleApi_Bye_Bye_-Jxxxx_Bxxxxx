using FluentValidation;
using SimpleApi.DTO;
using SimpleApi.Mapping;
using SimpleApi.Validation;
using SimpleApi.Handlers;
using SimpleApi.Pipeline;
using SimpleApi.Features.Users;
using SimpleApi.Vanilla;

var builder = WebApplication.CreateBuilder(args);

// ── Simple approach (manual handler) ──
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
builder.Services.AddScoped<CreateUserHandler>();

// ── Mini-framework pipeline approach ──
builder.Services.AddDispatcher();
builder.Services.AddHandler<CreateUserCommand, UserResponse, CreateUserCommandHandler>();
builder.Services.AddValidationBehavior<CreateUserCommand, UserResponse>();
builder.Services.AddLoggingBehavior<CreateUserCommand, UserResponse>();

// ── Vanilla approach (ZERO external dependencies) ──
builder.Services.AddScoped<IVanillaValidator<CreateUserRequest>, CreateUserRequestVanillaValidator>();
builder.Services.AddScoped<IVanillaValidator<CreateUserVanillaCommand>, CreateUserCommandVanillaValidator>();
builder.Services.AddHandler<CreateUserVanillaCommand, UserResponse, CreateUserVanillaCommandHandler>();
builder.Services.AddVanillaValidationBehavior<CreateUserVanillaCommand, UserResponse>();
builder.Services.AddLoggingBehavior<CreateUserVanillaCommand, UserResponse>();

var app = builder.Build();

// ═══════════════════════════════════════════════════════════════
// ENDPOINT 1: Simple approach (manual validation + mapping + handler)
// ═══════════════════════════════════════════════════════════════
app.MapPost("/users", async (
    CreateUserRequest request,
    IValidator<CreateUserRequest> validator,
    CreateUserHandler handler) =>
{
    // 1 VALIDATE
    var validation = await validator.ValidateAsync(request);

    if (!validation.IsValid)
        return Results.ValidationProblem(validation.ToDictionary());

    // 2 MAP DTO → DOMAIN
    var user = UserMapper.ToDomain(request);

    // 3 HANDLE
    var created = handler.Handle(user);

    // 4 MAP DOMAIN → RESPONSE
    var response = UserMapper.ToResponse(created);

    return Results.Created($"/users/{response.Id}", response);
});

// ═══════════════════════════════════════════════════════════════
// ENDPOINT 2: Pipeline approach (mini-framework replaces MediatR)
//   Validation + Logging behaviors run automatically.
// ═══════════════════════════════════════════════════════════════
app.MapPost("/v2/users", async (CreateUserRequest request, IDispatcher dispatcher) =>
{
    var command = new CreateUserCommand(request.Email, request.Name, request.Age);

    var response = await dispatcher.SendAsync<CreateUserCommand, UserResponse>(command);

    return Results.Created($"/v2/users/{response.Id}", response);
})
.AddEndpointFilter(async (ctx, next) =>
{
    // Catch FluentValidation exceptions from the pipeline and return 400
    try
    {
        return await next(ctx);
    }
    catch (ValidationException ex)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        return Results.ValidationProblem(errors);
    }
});

// ═══════════════════════════════════════════════════════════════
// ENDPOINT 3: Vanilla approach — ZERO external dependencies
//   No FluentValidation, no AutoMapper, no MediatR, no nada.
// ═══════════════════════════════════════════════════════════════
app.MapPost("/v3/users", (CreateUserRequest request, IVanillaValidator<CreateUserRequest> validator, CreateUserHandler handler) =>
{
    // 1 VALIDATE (vanilla — no FluentValidation)
    var validation = validator.Validate(request);
    if (!validation.IsValid)
        return Results.ValidationProblem(validation.ToDictionary());

    // 2 MAP DTO → DOMAIN (static mapper — no AutoMapper)
    var user = UserMapper.ToDomain(request);

    // 3 HANDLE (direct call — no MediatR)
    var created = handler.Handle(user);

    // 4 MAP DOMAIN → RESPONSE
    var response = UserMapper.ToResponse(created);

    return Results.Created($"/v3/users/{response.Id}", response);
});

// ═══════════════════════════════════════════════════════════════
// ENDPOINT 4: Vanilla pipeline — ZERO external dependencies + pipeline behaviors
// ═══════════════════════════════════════════════════════════════
app.MapPost("/v4/users", async (CreateUserRequest request, IDispatcher dispatcher) =>
{
    var command = new CreateUserVanillaCommand(request.Email, request.Name, request.Age);
    var response = await dispatcher.SendAsync<CreateUserVanillaCommand, UserResponse>(command);
    return Results.Created($"/v4/users/{response.Id}", response);
})
.AddEndpointFilter(async (ctx, next) =>
{
    try
    {
        return await next(ctx);
    }
    catch (VanillaValidationException ex)
    {
        return Results.ValidationProblem(ex.Errors);
    }
});

app.Run();

// Required for WebApplicationFactory<Program> in integration tests
public partial class Program;

