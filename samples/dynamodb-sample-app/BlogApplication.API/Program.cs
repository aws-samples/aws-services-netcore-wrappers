using Amazon.DynamoDb.Wrapper.Extensions;
using BlogApplication.Service;
using BlogApplication.Service.AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(EntityToDtoProfile));

builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBlogService, BlogService>();

builder.Services.RegisterDynamoDBServices(builder.Configuration);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
