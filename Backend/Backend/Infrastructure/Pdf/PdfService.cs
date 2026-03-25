using System.IO;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace Backend.Infrastructure.Pdf;

public interface IPdfService
{
    Task<byte[]> HtmlToPdfAsync(string html, CancellationToken cancellationToken);
}

public sealed class PdfService : IPdfService
{
    private static readonly SemaphoreSlim BrowserLock = new(1, 1);
    private static IBrowser? SharedBrowser;

    public async Task<byte[]> HtmlToPdfAsync(string html, CancellationToken cancellationToken)
    {
        var browser = await GetBrowserAsync(cancellationToken);
        await using var page = await browser.NewPageAsync();
        await page.SetContentAsync(html, new NavigationOptions { WaitUntil = [WaitUntilNavigation.Networkidle0] });

        return await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            MarginOptions = new MarginOptions { Top = "12mm", Bottom = "12mm", Left = "10mm", Right = "10mm" }
        });
    }

    private static async Task<IBrowser> GetBrowserAsync(CancellationToken cancellationToken)
    {
        if (SharedBrowser is not null) return SharedBrowser;

        await BrowserLock.WaitAsync(cancellationToken);
        try
        {
            if (SharedBrowser is not null) return SharedBrowser;

            var browserPath = Path.Combine(Path.GetTempPath(), "BillBookPuppeteer");

            var fetcher = new BrowserFetcher(new BrowserFetcherOptions
            {
                Path = browserPath
            });

            var installedBrowser = await fetcher.DownloadAsync();

            SharedBrowser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = installedBrowser.GetExecutablePath(),
                Args = new[] { "--no-sandbox", "--disable-dev-shm-usage" }
            });

            return SharedBrowser;
        }
        finally
        {
            BrowserLock.Release();
        }
    }
}
