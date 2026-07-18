using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Products;
using E_Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.DataSeeding
{
    internal class CatalogDataSeeder(StoreDbContext dbContext, ILogger<CatalogDataSeeder> logger) : IDataSeeder
    {
        public async Task SeedDataAsync(CancellationToken cancellationToken = default)
        {
			try
			{
				var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
				if (pendingMigrations.Any())
				{
					await dbContext.Database.MigrateAsync(cancellationToken);

				}

				//Seeding
				//Path
				//E:\Software Engineering\Route\Full Stack\Back End\Eng Aliaa Tarek\#MY_WORK\09- API\E_Commerce\E_Commerce.API\bin\Debug\net8.0\DataSeed

				var seedRoot = Path.Combine(AppContext.BaseDirectory, "DataSeed");

				await SeedIfEmptyAsync<ProductBrand, int>(seedRoot, "brands.json", cancellationToken);
				await SeedIfEmptyAsync<ProductType, int>(seedRoot, "types.json", cancellationToken);
				await SeedIfEmptyAsync<Product, int>(seedRoot, "products.json", cancellationToken);


				int result = await dbContext.SaveChangesAsync(cancellationToken);
				if(result > 0)
				{
					logger.LogInformation($"{result} Rows Added");
				}
				else
				{
					logger.LogInformation("Database Already Seeded");
				}

			}

			catch (Exception)
			{

				throw;
			}
        }
		

		private async Task SeedIfEmptyAsync<T, TKey>(String rootPath, string fileName, CancellationToken cancellationToken) where T : BaseEntity<TKey> 
		{
			if(await dbContext.Set<T>().AnyAsync())
			{
				logger.LogInformation("Table Already Has Data");
				return;
			}

			var filePath = Path.Combine(rootPath, fileName);

			if(!File.Exists(filePath))
			{
				logger.LogWarning($"File {fileName} is not exists");
				return;
			}

			using var fileStream = File.OpenRead(filePath);

			var options = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true,
			};

			var items = await JsonSerializer.DeserializeAsync <List<T>>(fileStream, options, cancellationToken);

			if (items?.Any() ?? false)
				dbContext.Set<T>().AddRange(items);
		}
	}
}
