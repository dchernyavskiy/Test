using System.Text;
using Microsoft.AspNetCore.Mvc;
using Test.Exceptions;
using Test.Helpers;
using Test.Services.Contracts;

namespace Test.Controllers;

public class FolderController : Controller
{
    private readonly IFolderService _folderService;
    private readonly ILogger<FolderController> _logger;

    public FolderController(ILogger<FolderController> logger, IFolderService folderService)
    {
        _logger = logger;
        _folderService = folderService;
    }

    public async Task<IActionResult> Index([FromRoute] string segments)
    {
        try
        {
            var folder = await _folderService.FindByPathAsync(segments);
            return View(folder);
        }
        catch (FolderNotFoundException e)
        {
            return View("Error");
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to find folder. Error message: " + e.Message);
            return View("Error");
        }
    }

    public async Task<IActionResult> Parents()
    {
        return View(await _folderService.GetAllParentFoldersAsync());
    }
    
    [HttpPost]
    public async Task<IActionResult> ImportFromOs()
    {
        await _folderService.ImportFromOS();
        return View("Error");
    }

    [HttpPost]
    public async Task<IActionResult> ImportFromFile(IFormFile jsonFile)
    {
        var json = await JsonHelper.ExtractJson(jsonFile);
        await _folderService.ImportFromFile(json);
        return View("Error");
    }

    [HttpPost]
    public async Task<IActionResult> Export()
    {
        var content = Encoding.UTF8.GetBytes(await _folderService.ExportToFileAsync());
        var type = "text/json";
        var name = "folders.json";
        return File(content, type, name);
    }
}