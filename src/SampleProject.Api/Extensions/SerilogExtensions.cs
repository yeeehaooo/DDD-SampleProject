using Serilog;

namespace SampleProject.Api.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        // 從設定檔讀取 Serilog 設定
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        // 設定 Serilog 為主要的日誌提供者
        builder.Host.UseSerilog(logger);

        return builder;
    }
}
