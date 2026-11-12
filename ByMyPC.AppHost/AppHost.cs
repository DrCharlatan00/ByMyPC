var builder = DistributedApplication.CreateBuilder(args);

var postgresPass = builder.AddParameter("pg-password",
                                        true);
var redisPass = builder.AddParameter("redis-password",
                                      true);

var postgresUser = builder.AddParameter("postgres-user", "drcharlatan");

var pgsql = builder.AddPostgres("postgres")
                   .WithUserName(postgresUser)
                   .WithPassword(postgresPass)
                   .WithDataVolume("bymypc-postgres-data");

var database = pgsql.AddDatabase("postgresMainDb");

var redis = builder.AddRedis("RedisMain")
                   .WithPassword(redisPass)
                   .WithDataVolume("bymypc-redis-data");

var api = builder.AddProject<Projects.ByMyPC>("api")
    .WithReference(database)
    .WithReference(redis)
    .WaitFor(database)
    .WaitFor(redis);

builder.Build().Run();
