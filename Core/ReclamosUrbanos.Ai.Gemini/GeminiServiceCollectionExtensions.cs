using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReclamosUrbanos.BusinessLogic.Services;

namespace ReclamosUrbanos.Ai.Gemini;

public static class GeminiServiceCollectionExtensions
{
    public static IServiceCollection AddGeminiAiService(
        this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(GeminiOptions.SectionName);
        services.Configure<GeminiOptions>(section);

        var options = section.Get<GeminiOptions>() ?? new GeminiOptions();

        services.AddHttpClient<IAiService, GeminiAiService>(client =>
        {
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }
}
