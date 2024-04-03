using Microsoft.AspNetCore.Mvc;
using OfficeSearch.Models;
using OfficeSearch.Services.Indexing;
using OfficeSearch.Services.Querying;
using System.Diagnostics;

namespace OfficeSearch.Controllers;

public class HomeController(IQueryService queryService, IIndexBuilder indexBuilder) : Controller
{
    private readonly bool _optimizeBatchSize = false;

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Index(OfficeSearchData model)
    {
        try
        {
            model.SearchText ??= "";
            await queryService.RunQueryAsync(model);
            return View(model);
        }

        catch
        {
            return View("Error", new ErrorViewModel { RequestId = "1" });
        }
    }

    [HttpGet]
    public IActionResult BuildIndex()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Generate()
    {
        if (_optimizeBatchSize)
        {
            await indexBuilder.OptimizeBatchSize();
            ViewBag.Message = "Check the Console to determine optimal batch size.";
        }
        else
        {
            await indexBuilder.Build();
            ViewBag.Message = "Index complete!";
        }
        return View("BuildIndex");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Privacy()
    {
        return View();
    }
}