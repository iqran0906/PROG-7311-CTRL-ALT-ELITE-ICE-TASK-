using MediLink.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MediLink.Controllers
{
    public class DrugInfoController : Controller
    {
        private readonly IDrugInfoService _drugInfoService;

        public DrugInfoController(IDrugInfoService drugInfoService)
        {
            _drugInfoService = drugInfoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string drugName)
        {
            if (string.IsNullOrWhiteSpace(drugName))
            {
                ModelState.AddModelError(string.Empty, "Please enter a drug name.");
                return View("Index");
            }

            try
            {
                var results = await _drugInfoService.SearchDrugInfoAsync(drugName);
                ViewBag.SearchTerm = drugName;
                return View("Results", results);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Index");
            }
        }
    }
}