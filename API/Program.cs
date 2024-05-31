
using Repositories.Helper;
using Services.Interface;
using Services;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            //VNPay Service
            builder.Services.AddScoped<Services.VnpayService>();


            //Email Service
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddTransient<IMailService, MailService>();

            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

            //cors cho mấy thằng gà react
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5000") // Địa chỉ của ứng dụng React
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            
                app.UseSwagger();
                app.UseSwaggerUI();
            

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();
            app.UseCors("AllowSpecificOrigin");

            app.MapControllers();

            app.Run();
        }
    }
}
