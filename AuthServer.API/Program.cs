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
//tek istekte-requestte- 1 nesne �rne�i olu�acak s�rekli onu kullanacak.
//trans: her interface ile ka��la�t���nda yeni nesne �rne�i
//singleton:uygulama boyunca tek nesne �rne�i
//coredaki interface i ekle
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
//bizim framework�m�z herhangi bir constructorda bu interface ile kar��la�t���nda ne yapaca��n� biliyor.
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
//generic olanlar�n implemantasyonu biraz farkl�.typeof... tek parametre al�yorsa <> �ift al�rsa <,> kullan.
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IServiceGeneric<,>),typeof(ServiceGeneric<,>));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//dbcontexti ekleyelim.

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"),sqlOptions=>
    {
        //Bu ayar hem ba�lant� c�mleci�i hem de migration ayar� ara�t�r.
        //assembly ismi ver hangi katnada migration olacak. data
        sqlOptions.MigrationsAssembly("AuthServer.Data");
    });
});

//�dentity i DI container a ekledik dedik ki kullan�c�m�z UserApp , rol�m�z ise �dentityrole. sen kendin role ekleyeceksen �dentityroleden miras alan bir class yapars�n...
builder.Services.AddIdentity<UserApp, IdentityRole>(Opt=>
{
    Opt.User.RequireUniqueEmail= true;//email uniq olsun.
    Opt.Password.RequireNonAlphanumeric= false;//passwordde Nonalfanum... zorunlu mu:false
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();//ef de kullanaca��n dbcontexti se�.ve �ifre s�f�rlama i�lemlerinde token �retiyoruz bunu i�in yazd�k.





// Add services to the container.
//OPT�ONS PATTERN D�YE GE�ER
//appsetting ile CustomTokenOption class�n� ileti�ime ge�irdik.DI container a bir nesne ge�tik.

builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOption"));




builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));


//token do�rulama
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//2 �emay� birbirine ba�lad�k.
    //farkl� �yelik sistemleri varsa bayiler i�in normal kullan�c�lar i�in bunlar sema biz default belirttik 1 TANE VAR. ��MD� DO�RULAMA NASIL OLCAK COOK�E-JWT AP� OLD. DOLAYI JWT YAPCAZ.
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,opts=>
{
    var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOption>();
    opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience[0],//ilk ine bakmak do�rulamak i�in yeterli
        IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

        ValidateIssuerSigningKey = true,//�nemli mutlaka bir imzas� olmal�.benim g�nderdi�im issuer mu
        ValidateAudience = true,
        ValidateIssuer = true,//bunu da do�rula...
        ValidateLifetime = true,//�mr�n� kontrol et

        //�OK KULLANILMAZ AMA B�LEL�M.
        ClockSkew = TimeSpan.Zero,//1 saaat �m�r varsa default olarak  5 dk da kendi ekler. neden: farkl� zaman aral�klar�ndaki serverlara kurabilirsin arada fark� minimize etmek i�in.ONU �PTAL ED�YORUZ G�B� 2 SERVER ZAMANINI DA E��TLED�K. FARK SIFIR.
    };
}
);
//ENDPO�NT�ME bir request yap�ld���nda benim projen o requestin header�nda o token � arayacak.




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
