using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplicationManagerApi.AuthApp;
using WebApplicationManagerApi.ContextFolder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(@"Server = (localdb)\MSSQLLocalDB; 
                                            DataBase = DataBaseApplication; 
                                            Trusted_connection = true;"));
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var _RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var _UserManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();



    var s = scope.ServiceProvider;
    var c = s.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(c, _UserManager, _RoleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context, UserManager<User> _UserManager, RoleManager<IdentityRole> _RoleManager)
    {
        context.Database.EnsureCreated(); //�������, ���� ��� �� �������
                                          //�� ������������ ��������� user 
                                          //� �� ����� ���������, ��� ������� � ������ ���������

        if (context.Users.Any()) return; //���� ���� ���� ���� ������������, �� �������� �� ���������:
        var res = _RoleManager.CreateAsync(new IdentityRole("Admin")).Result; //�������� ���� � �������
        User admin = new User { UserName = "admin" }; //������� �������� ������
        var createResult = _UserManager.CreateAsync(admin, "admin").Result; //���������������� ������ � ��
        res = _UserManager.AddToRoleAsync(admin, "Admin").Result; //������ ������ ���� ������


        //��� �� ��������� ����������� �������� ������ 


    }
}
