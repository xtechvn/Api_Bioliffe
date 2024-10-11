using Entities.ConfigModels;
using HuloToys_Service.RedisWorker;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HuloToys_Service
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

            // khoi tao lan dau tien chuoi config khi ung dung duoc chay.
                        

            // Register services   
            services.AddSingleton(Configuration);
           

            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressConsumesConstraintForFormFileParameters = true;
                    options.SuppressInferBindingSourcesForParameters = true;
                    options.SuppressModelStateInvalidFilter = true;
                    options.SuppressMapClientErrors = true;
                })
               .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.PropertyNamingPolicy = null; // Để giữ nguyên case của các thuộc tính
                   options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
               });


            //   builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            // builder.Services.AddEndpointsApiExplorer();
            // builder.Services.AddSwaggerGen();

            // Configure JWT Authentication
            var key = "this is my custom Secret key for authentication";
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });
            
            services.Configure<DataBaseConfig>(Configuration.GetSection("DataBaseConfig"));
            services.Configure<MailConfig>(Configuration.GetSection("MailConfig"));
            services.Configure<DomainConfig>(Configuration.GetSection("DomainConfig"));


            // Register services
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<RedisConn>();

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RedisConn redisService)
        {         
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Inject the authorization middleware into the Request pipeline.
            app.UseAuthentication();
            app.UseAuthorization();

            //Redis conn Call the connect method
            redisService.Connect();
            app.UseCors("MyApi");        

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

           // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }            
        }
    }
}
