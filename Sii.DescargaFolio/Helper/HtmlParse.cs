using AngleSharp;
using AngleSharp.Dom;

namespace Sii.DescargaFolio.Helper;

public static class HtmlParse
{
    public static async Task<Dictionary<string, object>> GetFromDom(
        this HttpResponseMessage? msg,
        string tag
    )
    {
        await GuardarHtml(msg);
        Dictionary<string, object> dics = [];
        IBrowsingContext context = BrowsingContext.New(Configuration.Default);
        IDocument document = await context.OpenAsync(async req =>
            req.Content(await msg!.Content.ReadAsStringAsync())
        );

        IHtmlCollection<IElement> result = document.QuerySelectorAll(tag);
        if (result.Any())
            foreach (IElement item in result)
                if (item.GetType().Name == "HtmlInputElement")
                    dics.TryAdd(
                        item.GetAttribute("name")?.Trim() ?? string.Empty,
                        item.GetAttribute("value")?.Trim() ?? string.Empty
                    );
                else if (item.LocalName == "select")
                {
                    string selectName = item.GetAttribute("name")?.Trim() ?? string.Empty;
                    if (!string.IsNullOrEmpty(selectName))
                    {
                        var options = item.QuerySelectorAll("option")
                            .Select(option => new
                            {
                                Text = option.TextContent.Trim(),
                                Value = option.GetAttribute("value")?.Trim() ?? string.Empty,
                            })
                            .ToList();
                        dics.TryAdd(selectName, options);
                    }
                }
                else if (item.LocalName == "table")
                {
                    IHtmlCollection<IElement>? headers = item.QuerySelectorAll("tr")
                        .FirstOrDefault()
                        ?.QuerySelectorAll("td, th");
                    if (
                        headers != null
                        && headers.Any(header =>
                            header.TextContent.Contains(
                                "Actividades",
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                    )
                    {
                        List<List<string>> rows = item.QuerySelectorAll("tr")
                            .Skip(1)
                            .Select(row =>
                                row.QuerySelectorAll("td")
                                    .Select(td => td.TextContent.Trim())
                                    .ToList()
                            )
                            .ToList();

                        if (rows.Any())
                            dics.TryAdd("Actividades", rows);
                    }
                    else
                    {
                        List<List<string>> genericRows = item.QuerySelectorAll("tr")
                            .Skip(1)
                            .Select(row =>
                                row.QuerySelectorAll("td")
                                    .Select(td => td.TextContent.Trim())
                                    .ToList()
                            )
                            .ToList();

                        if (genericRows.Any())
                            dics.TryAdd($"Table{dics.Count}", genericRows);
                    }
                }
                else if (item.LocalName == "td")
                {
                    IHtmlCollection<IElement> td = item.QuerySelectorAll("td");
                    if (td.Length > 1)
                        dics.TryAdd(td[0].TextContent.Trim(), td[1].TextContent.Trim());
                }

        return dics;
    }

    private static async Task GuardarHtml(HttpResponseMessage? msg)
    {
        await File.WriteAllBytesAsync(
            $"{Path.GetTempPath()}Html_resultado_{DateTime.Now:dd-MM-HH-mm-ss}.html",
            await msg!.Content.ReadAsByteArrayAsync()
        );
    }
}
