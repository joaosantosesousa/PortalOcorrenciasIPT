using Microsoft.EntityFrameworkCore;
using PortalOcorrenciasIPT.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PortalOcorrenciasIPT.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Registo dos serviços principais da aplicação.
// Razor Pages suporta a interface web, Controllers suporta a API REST
// e SignalR permite comunicação em tempo real com o browser.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddSignalR();

// Configuração do Entity Framework Core com SQL Server.
// EnableRetryOnFailure acrescenta resiliência para falhas transitórias,
// útil em ambiente cloud, como Azure SQL Database.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions =>
        {
            sqlServerOptions.EnableRetryOnFailure();
        }));

// Filtro útil em desenvolvimento para apresentar erros relacionados com migrations/base de dados.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configuração do ASP.NET Identity.
// A aplicação usa ApplicationUser e roles para distinguir Utilizador e Gestor.
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Configuração da autenticação JWT usada pela API.
// A autenticação por cookies continua a ser usada pela interface Razor Pages,
// enquanto o JWT permite autenticar pedidos externos à API.
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

var app = builder.Build();

// Configuração do pipeline HTTP.
// Em desenvolvimento são apresentados erros de migrations;
// em produção é usada uma página de erro genérica e HSTS.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Encaminha códigos de erro, como 404 ou 403, para a página Error.
app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");

app.UseHttpsRedirection();

app.UseRouting();

// A autenticação tem de ser executada antes da autorização,
// para que a aplicação saiba quem é o utilizador antes de validar permissões.
app.UseAuthentication();

app.UseAuthorization();

// Mapeamento dos recursos estáticos e das Razor Pages.
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

// Mapeamento dos endpoints da API REST.
app.MapControllers();

// Mapeamento do hub SignalR usado para notificações em tempo real.
app.MapHub<OcorrenciasHub>("/ocorrenciasHub");

// Criação inicial das roles necessárias à aplicação.
// Isto garante que as roles existem quando a aplicação arranca,
// tanto localmente como após publicação no Azure.
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Utilizador", "Gestor" };

    foreach (var role in roles)
    {
        bool roleExiste = await roleManager.RoleExistsAsync(role);

        if (!roleExiste)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

app.Run();
