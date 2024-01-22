## NBL proof of concept. 

### Uruchomienie projektu

Projekt jest konteneryzowany, wykorzystuje `docker-compose` jako orkiestrator kontenerów. 

Aby uruchomić projekt należy przejść do głownego katalogu folderu i użyc komendy `docker-compose up --build`.

### Struktura Projektu
Projekt na ten moment jest monolitem, wykorzystującym model `multi-tenant` w wariancie `database per tenant`, model ten charakteryzuje się podziałem fizycznym bazy danych dla każdego z tenantów. 

Głownym determinatorem, tego która z baz danych będzie wykorzystywana przy obsłudze requestu jest middleware
```c#
builder.Services.AddScoped<ITenant>(sp =>
{
    var tenantIdString = sp.GetRequiredService<IHttpContextAccessor>().HttpContext?.Request.Headers["X-TenantId"]; 
    return !string.IsNullOrEmpty(tenantIdString) && int.TryParse(tenantIdString, out var tenantId)
        ? new TenantData(tenantId)
        : null;
});
```  
`ITenant` pózniej jest wykorzystywany w `TodoDbContextScopedFactory`
do ustalenia connection stringa z którego dany `DbContext` będzie korzystał.

![Tenant](https://i.ibb.co/QjW5tXQ/Tenant-arch-v3.png)  

Dane każdego z tenantów, sa przechowywane w bazie danych `Tenants`, więcej informacji można znaleść w pliku `TenantsDbContext` oraz `TenantsService`.

Do połaczenia z bazą danych i robienia zapytań wykorzystywany jest Entity Framework Core 8.0 wraz z context poolingiem,
```c#
builder.Services.AddPooledDbContextFactory<TodoDbContext>(options =>
{
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
		o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "todos"));
});
```

Do stworzenia api publicznego, wykorzystywane są kontrolery z wzorca MVC,
endpointy są wersjonowane przy pomocy biblioteki `Asp.Versioning.Mvc`.
```c#
builder.Services.AddApiVersioning(options =>
{
	options.DefaultApiVersion = ApiVersioning.V1;
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.ReportApiVersions = true;
	options.ApiVersionReader = new HeaderApiVersionReader("api-version");
	
}).AddMvc(options =>
{
	options.Conventions.Controller<TodoController>().HasApiVersion(ApiVersioning.V1);
	options.Conventions.Controller<TenantController>().HasApiVersion(ApiVersioning.V1);
});
```
### Testy wydajnościowe
Wydajność aplikacji jest testowana przy pomocy narzędzia `K6`,
projekt zawiera 5 rodzajów testów wydajnościowych
- Load
- Soak
- Spike
- Stress
- Smoke

### Benchmark 
Benchmark zapytań bazodanowych został przeprowadzony przy pomocy biblioteki `Benchmarkdotnet`, na tem moment benchmark ten testuje proste zapytania do bazy danych używając zarówno `Entity Framework Core` jak i również dla `Dapper`.

Aby uruchomić benchmark należy wejsc do folderu `NBL_PoC_Benchmarks` i uruchomić benchmark przy pomocy polecenia `dotnet run -c Release`.







