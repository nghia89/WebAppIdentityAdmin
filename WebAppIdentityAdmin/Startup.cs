using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppIdentityAdmin.Services;

namespace WebAppIdentityAdmin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
             .AddOpenIdConnect("oidc", options =>
             {
                 options.Authority = Configuration["Authorization:AuthorityUrl"];
                 options.RequireHttpsMetadata = false;
                 options.GetClaimsFromUserInfoEndpoint = true;

                 options.ClientId = Configuration["Authorization:ClientId"];
                 options.ClientSecret = Configuration["Authorization:ClientSecret"];
                 options.ResponseType = "code";

                 options.SaveTokens = true;

                 options.Scope.Add("openid");
                 options.Scope.Add("profile");
                 options.Scope.Add("offline_access");
                 options.Scope.Add("api.webApp");
                 options.UseTokenLifetime = true;
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     NameClaimType = "name",
                     RoleClaimType = "role"
                 };
                 options.Events = new OpenIdConnectEvents
                 {
                     // that event is called after the OIDC middleware received the auhorisation code,
                     // redeemed it for an access token and a refresh token,
                     // and validated the identity token
                     OnTokenValidated = x =>
                              {
                                  // store both access and refresh token in the claims - hence in the cookie
                                  var identity = (ClaimsIdentity)x.Principal.Identity;
                                  identity.AddClaims(new[]
                                  {
                                new Claim("access_token", x.TokenEndpointResponse.AccessToken),
                                new Claim("refresh_token", x.TokenEndpointResponse.RefreshToken)
                                     });

                                  // so that we don't issue a session cookie but one with a fixed expiration
                                  x.Properties.IsPersistent = true;

                                  // align expiration of the cookie with expiration of the
                                  // access token
                                  var accessToken = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken);
                                  x.Properties.ExpiresUtc = accessToken.ValidTo;

                                  return Task.CompletedTask;
                              }
                 };
             });

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });

            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseMiddleware<CheckAuthenticationMiddlewares>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapFallbackToController("Index", "Home");
            });
        }
    }
}
