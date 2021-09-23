using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo4j.Driver;
using System;
using FormatZPD.Repositories;

namespace FormatZPD
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
            services.AddCors(); // fix cors for api
        
            services.AddControllers();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add auth neo4j 
            var neo4jSettings = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "1234"));
            services.AddSingleton(neo4jSettings);
            CreateDatabaseInitialize(neo4jSettings);
            
            // Add repository
            services.AddScoped<INodeRepo, NodeRepo>();
            services.AddScoped<IPersonRepo, PersonRepo>();
            services.AddScoped<IBeliefRepo, BeliefRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void CreateDatabaseInitialize(IDriver driver)
        {
            IAsyncSession session = driver.AsyncSession();
            try
            {
                session.RunAsync(@"
CREATE CONSTRAINT node_unique IF NOT EXISTS
ON (n:Node)
ASSERT n.id IS UNIQUE;

CREATE CONSTRAINT person_unique IF NOT EXISTS
ON (p:Person)
ASSERT p.id IS UNIQUE;

CREATE CONSTRAINT belief_unique IF NOT EXISTS
ON (b:Belief)
ASSERT b.id IS UNIQUE;

OPTIONAL MATCH (n:Node{title: 'Root', type: 'root', removed: false})
WITH n WHERE n IS NULL
CREATE (:Node {title:'Root', type: 'root', removed: false, id: apoc.create.uuid()})
");
            }
            finally
            {
                session.CloseAsync();
            }
        }
    }
}
