using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using OfficeSearch.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using OfficeSearch.Services.Querying;
using OfficeSearch.Services.Indexing;

namespace OfficeSearch.Controllers;

public class HomeController : Controller
{
    private readonly QueryService _queryService;
    private readonly Indexer _indexer;

    public HomeController(QueryService queryService, Indexer indexer)
    {
        _queryService = queryService;
        _indexer = indexer;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Index(OfficeSearchData model)
    {
        try
        {
            // Check for a search string
            if (model.searchText == null)
            {
                model.searchText = "";
            }

            // Send the query to Search.
            await _queryService.RunQueryAsync(model);
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
        await _indexer.Build();
        ViewBag.Message = "Indexing complete!";
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