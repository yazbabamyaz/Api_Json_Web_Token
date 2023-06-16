using AuthServer.Core.Configuration;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data;
using AuthServer.Data.Repositories;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Configurations;

var builder = WebApplication.CreateBuilder(args);

//DI registered
//tek istekte-requestte- 1 nesne örneði oluþacak sürekli onu kullanacak.
//trans: her interface ile kaþýlaþtýðýnda yeni nesne örneði
//singleton:uygulama boyunca tek nesne örneði
//coredaki interface i ekle
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
//bizim frameworkümüz herhangi bir constructorda bu interface ile karþýlaþtýðýnda ne yapacaðýný biliyor.
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
//generic olanlarýn implemantasyonu biraz farklý.typeof... tek parametre alýyorsa <> çift alýrsa <,> kullan.
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IServiceGeneric<,>),typeof(ServiceGeneric<,>));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//dbcontexti ekleyelim.

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"),sqlOptions=>
    {
        //Bu ayar hem baðlantý cümleciði hem de migration ayarý araþtýr.
        //assembly ismi ver hangi katnada migration olacak. data
        sqlOptions.MigrationsAssembly("AuthServer.Data");
    });
});

//ýdentity i DI container a ekledik dedik ki kullanýcýmýz UserApp , rolümüz ise ýdentityrole. sen kendin role ekleyeceksen ýdentityroleden miras alan bir class yaparsýn...
builder.Services.AddIdentity<UserApp, IdentityRole>(Opt=>
{
    Opt.User.RequireUniqueEmail= true;//email uniq olsun.
    Opt.Password.RequireNonAlphanumeric= false;//passwordde Nonalfanum... zorunlu mu:false
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();//ef de kullanacaðýn dbcontexti seç.ve þifre sýfýrlama iþlemlerinde token üretiyoruz bunu için yazdýk.





// Add services to the container.
//OPTÝONS PATTERN DÝYE GEÇER
//appsetting ile CustomTokenOption classýný iletiþime geçirdik.DI container a bir nesne geçtik.

builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOption"));




builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));


//token doðrulama
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//2 þemayý birbirine baðladýk.
    //farklý üyelik sistemleri varsa bayiler için normal kullanýcýlar için bunlar sema biz default belirttik 1 TANE VAR. ÞÝMDÝ DOÐRULAMA NASIL OLCAK COOKÝE-JWT APÝ OLD. DOLAYI JWT YAPCAZ.
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,opts=>
{
    var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOption>();
    opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience[0],//ilk ine bakmak doðrulamak için yeterli
        IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

        ValidateIssuerSigningKey = true,//önemli mutlaka bir imzasý olmalý.benim gönderdiðim issuer mu
        ValidateAudience = true,
        ValidateIssuer = true,//bunu da doðrula...
        ValidateLifetime = true,//ömrünü kontrol et

        //ÇOK KULLANILMAZ AMA BÝLELÝM.
        ClockSkew = TimeSpan.Zero,//1 saaat ömür varsa default olarak  5 dk da kendi ekler. neden: farklý zaman aralýklarýndaki serverlara kurabilirsin arada farký minimize etmek için.ONU ÝPTAL EDÝYORUZ GÝBÝ 2 SERVER ZAMANINI DA EÞÝTLEDÝK. FARK SIFIR.
    };
}
);
//ENDPOÝNTÝME bir request yapýldýðýnda benim projen o requestin headerýnda o token ý arayacak.




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
