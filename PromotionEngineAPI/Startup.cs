using ApplicationCore.Chain;
using ApplicationCore.Notification;
using ApplicationCore.Services;
using ApplicationCore.Worker;
using AutoMapper;
using Infrastructure.Models;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PromotionEngineAPI.Hubs;
using System;
using System.Text;
using WebAPI.Hubs;

namespace PromotionEngineAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("VueCorsPolicy", builder =>
                {
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithOrigins("http://localhost:8080")
                        .WithOrigins("https://manage-pe.reso.vn")
                        .WithOrigins("https://stg-admin.unilo.net")
                        .WithOrigins("https://stg-api.beanoi.com")
                        .WithOrigins("https://stg-manage-pe.reso.vn");

                });
            });
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {

                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AppSettings:SecretKey"]))
                    };
                });
            services.AddControllers();
            // add config swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Promotion Engine API", Version = "1" });
            });
            services.AddDbContext<PromotionEngineContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:PromotionEngineDatabase"]));
            services.AddTransient<PromotionEngineContext, PromotionEngineContext>();
            // add config auto mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // connect unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //DI for all service
            ServiceAddScoped(services);
            // configure controller
            services.AddControllers().AddNewtonsoftJson(option => option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddSignalR();

        }
        #region Add Scope
        private void ServiceAddScoped(IServiceCollection services)
        {

            //Account
            services.AddScoped<IAccountService, AccountService>();
            //Action
            services.AddScoped<IActionService, ActionService>();
            //Brand
            services.AddScoped<IBrandService, BrandService>();
            //Channel
            services.AddScoped<IChannelService, ChannelService>();
            //ConditionRule
            services.AddScoped<IConditionRuleService, ConditionRuleService>();
            //Holiday
            //Membership
            services.AddScoped<IMembershipService, MembershipService>();
            //OrderCondition
            services.AddScoped<IOrderConditionService, OrderConditionService>();
            //ProductCondition
            services.AddScoped<IProductConditionService, ProductConditionService>();
            //MembershipAction
            services.AddScoped<IGiftService, GiftService>();
            //Promotion
            services.AddScoped<IPromotionService, PromotionService>();
            //PromotionStoreMapping
            services.AddScoped<IPromotionStoreMappingService, PromotionStoreMappingService>();
            //PromotionTier
            services.AddScoped<IPromotionTierService, PromotionTierService>();
            //RoleEntity
            services.AddScoped<IRoleService, RoleService>();
            //Store
            services.AddScoped<IStoreService, StoreService>();
            //Voucher
            services.AddScoped<IVoucherService, VoucherService>();
            //VoucherChannel
            services.AddScoped<IVoucherChannelService, VoucherChannelService>();
            //VoucherGroup
            services.AddScoped<IVoucherGroupService, VoucherGroupService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IProductCateService, ProductCategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IMemberLevelService, MemberLevelService>();

            //LoginService
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IGameCampaignService, GameCampaignService>();
            services.AddScoped<IGameItemService, GameItemService>();
            services.AddScoped<IGameMasterService, GameMasterService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IPushNotification, PushNotification>();
            //MembershipProgram
            services.AddScoped<IMembershipProgramService, MembershipProgramService>();
            //WalletType
            services.AddScoped<IWalletTypeService, WalletTypeService>();
            //MemberActionType
            services.AddScoped<IMemberActionTypeService, MemberActionTypeService>();
            //MemberAction
            services.AddScoped<IMemberActionService, MemberActionService>();
            //MembershipLevel
            services.AddScoped<IMembershipLevelService, MembershipLevelService>();

            ChainOfResponsibilityServices(services);

            WorkerServices(services);

        }
        private void ChainOfResponsibilityServices(IServiceCollection services)
        {
            //ApplyPromotionHandler
            services.AddScoped<ICheckPromotionHandler, CheckPromotionHandler>();
            //PromotionHandle
            services.AddScoped<IPromotionHandle, PromotionHandle>();
            //TimeframeHandle
            services.AddScoped<ITimeframeHandle, TimeframeHandle>();
            //MembershipHandle
            services.AddScoped<IMembershipConditionHandle, MembershipConditionHandle>();
            //OrderHandle
            services.AddScoped<IConditionHandle, ConditionHandle>();
            //ProductHandle
            services.AddScoped<IProductConditionHandle, ProducConditiontHandle>();
            //OrderConditionHandle
            services.AddScoped<IOrderConditionHandle, OrderConditionHandle>();
            //ApplyPromotion
            services.AddScoped<IApplyPromotion, ApplyPromotion>();
        }
        private void WorkerServices(IServiceCollection services)
        {
            services.AddScoped<IVoucherWorker, VoucherWorker>();
        }
        #endregion

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Promotion Engine API V1");
            });

            app.UseRouting();

            app.UseCors("VueCorsPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                ReigsterHubs(endpoints);
                endpoints.MapControllers();
            });
        }
        #region Register hubs
        private void ReigsterHubs(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<VoucherHub>("/voucher/notify");
            endpoints.MapHub<NotifyMessageHub>("/notify/message");
        }
        #endregion
    }
}
