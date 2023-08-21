using dotnet_6_vue_cli_jwt_refresh_token.Context;
using dotnet_6_vue_cli_jwt_refresh_token.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserContext>(opts =>
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:RefreshTokenSample"]));

#region JWT �`�J
builder.Services.AddAuthentication(options => {
    //�]�_�w�]�ϥ�JWT���Ҥ覡
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters() {
        ValidateAudience = false,
        ValidateIssuer = true,
        ValidIssuer = "MyWebApi.home",
        ValidateLifetime = true,            //���ҹL���ɶ�
        ValidateIssuerSigningKey = false, 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("0FBCF3C1-0818-42EB-83B2-B3E15FC16C2C")),
        ClockSkew = TimeSpan.Zero
    };

});
#endregion

builder.Services.AddTransient<ITokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region ��XVue cli
app.UseDefaultFiles();  //����web api�M�ׯ�ϥιw�]���ɮק@���i�J�I
app.UseStaticFiles();   //�èϨ��ϥ��R�A�ɮק@���������귽

app.Use(async (context, next) => {
    await next();

    //�P�_�O�_�n�s�������M�Ӥ��O�o�eAPI�ݨD
    if (context.Response.StatusCode == 404 &&                         // �귽���s�b
      !System.IO.Path.HasExtension(context.Request.Path.Value) &&     // ���}�̫�S���a���ɦW
      !context.Request.Path.Value.StartsWith("/api"))                 // ���}���O /api �}�l (�o�O�]���Ϊ��O Web API �M�סM�w�]���|�O /api
    {
        context.Request.Path = "/index.html";                         // �N���}�ɦV /index.html (�o�O Vue ���_�l����)
        context.Response.StatusCode = 200;                            // �N HTTP ���A�אּ 200 ���\
        await next();
    }
});
#endregion

#region global cors policy ���\�Ҧ��ҥi�H�I�s,���ե�
app.UseCors(x => x
.SetIsOriginAllowed(origin => true)
.AllowAnyMethod()
.AllowAnyHeader()
.AllowCredentials());
#endregion

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
