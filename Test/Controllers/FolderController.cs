using Microsoft.AspNetCore.Mvc;
using Test.Exceptions;
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
}