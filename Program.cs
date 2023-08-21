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

#region JWT 注入
builder.Services.AddAuthentication(options => {
    //設罝預設使用JWT驗證方式
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters() {
        ValidateAudience = false,
        ValidateIssuer = true,
        ValidIssuer = "MyWebApi.home",
        ValidateLifetime = true,            //驗證過期時間
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

#region 整合Vue cli
app.UseDefaultFiles();  //讓此web api專案能使用預設的檔案作為進入點
app.UseStaticFiles();   //並使其能使用靜態檔案作為網頁的資源

app.Use(async (context, next) => {
    await next();

    //判斷是否要存取網頁﹐而不是發送API需求
    if (context.Response.StatusCode == 404 &&                         // 資源不存在
      !System.IO.Path.HasExtension(context.Request.Path.Value) &&     // 網址最後沒有帶副檔名
      !context.Request.Path.Value.StartsWith("/api"))                 // 網址不是 /api 開始 (這是因為用的是 Web API 專案﹐預設路徑是 /api
    {
        context.Request.Path = "/index.html";                         // 將網址導向 /index.html (這是 Vue 的起始網頁)
        context.Response.StatusCode = 200;                            // 將 HTTP 狀態改為 200 成功
        await next();
    }
});
#endregion

#region global cors policy 允許所有皆可以呼叫,測試用
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
