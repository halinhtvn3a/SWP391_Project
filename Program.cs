
using CourtManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CourtManagement
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			var services = builder.Services;
			var configuration = builder.Configuration;

			// Add services to the container.
			//services.AddAuthentication().AddGoogle(googleOptions =>
			//{
			//	googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
			//	googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
			//});
			services.AddAuthentication()
				.AddGoogle(googleOptions =>
				{
					googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
					googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
				})
				.AddFacebook(facebookOptions =>
				{
					facebookOptions.AppId = configuration["Authentication:Facebook:AppId"];
					facebookOptions.AppSecret = configuration["Authentication:Facebook:AppSecret"];
				});


			services.AddDbContext<CourtBookingContext>(options =>
					options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));



			services.AddControllersWithViews();

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

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
