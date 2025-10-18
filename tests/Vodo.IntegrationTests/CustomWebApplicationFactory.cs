using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using Vodo.DAL.Context;
using Vodo.Models;
using Vodo.Server;

namespace Vodo.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _inMemoryDbName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Удаляем стандартную регистрацию VodoContext (если есть)
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<VodoContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Регистрируем VodoContext с InMemory DB
                services.AddDbContext<VodoContext>(options =>
                {
                    options.UseInMemoryDatabase(_inMemoryDbName);
                });

                // Создаём провайдер и инициализируем тестовые данные
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<VodoContext>();

                // Убедиться, что БД создана
                db.Database.EnsureCreated();

                // Сидинг тестовых данных: Divisions
                if (!db.Divisions.Any())
                {
                    db.Divisions.Add(new Division { Name = "Test Division 1" });
                    db.Divisions.Add(new Division { Name = "Test Division 2" });
                    db.SaveChanges();
                }

                // Сидинг тестовых данных: JobObjects
                if (!db.JobObjects.Any())
                {
                    var jo1 = new JobObject
                    {
                        Name = "Test Site 1",
                        Location = new Point(new Coordinate(10.5, 20.5)) { SRID = 4326 },
                        OwnerDivision = "Ops"
                    };
                    var jo2 = new JobObject
                    {
                        Name = "Test Site 2",
                        Location = new Point(new Coordinate(5, 6)) { SRID = 4326 },
                        OwnerDivision = "Maintenance"
                    };

                    db.JobObjects.AddRange(jo1, jo2);
                    db.SaveChanges();
                }

                // Сидинг тестовых данных: Contractors
                if (!db.Contractors.Any())
                {
                    var contractor1 = new Contractor
                    {
                        Name = "Test Contractor 1",
                        Inn = "123456789012",
                        Payload = new Dictionary<string, object> { { "email", "c1@example.com" }, { "phone", "111" } }
                    };
                    var contractor2 = new Contractor
                    {
                        Name = "Test Contractor 2",
                        Inn = "987654321098",
                        Payload = new Dictionary<string, object> { { "email", "c2@example.com" }, { "phone", "222" } }
                    };

                    db.Contractors.AddRange(contractor1, contractor2);
                    db.SaveChanges();
                }

                // Сидинг тестовых данных: Jobs (связанные с JobObjects и Contractors)
                if (!db.Jobs.Any())
                {
                    // Возьмём существующие объекты/контракторов
                    var jobObject = db.JobObjects.FirstOrDefault();
                    var contractor = db.Contractors.FirstOrDefault();

                    // Если для надёжности ничего не найдено — создадим минимальные
                    if (jobObject == null)
                    {
                        jobObject = new JobObject { Name = "Auto Site", Location = new Point(new Coordinate(0, 0)) { SRID = 4326 } };
                        db.JobObjects.Add(jobObject);
                        db.SaveChanges();
                    }

                    if (contractor == null)
                    {
                        contractor = new Contractor { Name = "Auto Contractor" };
                        db.Contractors.Add(contractor);
                        db.SaveChanges();
                    }

                    var j1 = new Job
                    {
                        Title = "Test Job 1",
                        Description = "Job seeded for integration tests",
                        Type = JobType.Other,
                        StatusId = 1,
                        Priority = JobPriority.Normal,
                        JobObjectId = jobObject.Id,
                        ContractorId = contractor.Id,
                        Geometry = new Point(new Coordinate(10.5, 20.5)) { SRID = 4326 }
                    };

                    var j2 = new Job
                    {
                        Title = "Test Job 2",
                        Description = "Second seeded job",
                        Type = JobType.Inspection,
                        StatusId = 2,
                        Priority = JobPriority.Low,
                        JobObjectId = jobObject.Id,
                        ContractorId = contractor.Id,
                        Geometry = new Point(new Coordinate(5, 6)) { SRID = 4326 }
                    };

                    db.Jobs.AddRange(j1, j2);
                    db.SaveChanges();
                }
            });
        }
    }
}