using MediLink.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MediLink.Controllers
{
    /// Handles drug information search requests and displays medication-related data
    public class DrugInfoController : Controller
    {
        // Service responsible for retrieving drug-related information
        private readonly IDrugInfoService _drugInfoService;


        /// Initializes the drug information controller with the required service.
        /// <param name="drugInfoService">Service used to search and retrieve drug information.</param>
        public DrugInfoController(IDrugInfoService drugInfoService)
        {
            _drugInfoService = drugInfoService;
        }

        /// Displays the main drug search page.
        public IActionResult Index()
        {
            return View();
        }

        /// Processes the submitted drug search request.
        /// <param name="drugName">Name of the drug entered by the user.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string drugName)
        {
            // Validates that a drug name has been entered before searching
            if (string.IsNullOrWhiteSpace(drugName))
            {
                ModelState.AddModelError(string.Empty, "Please enter a drug name.");
                return View("Index");
            }

            try
            {
                // Retrieves drug information based on the entered drug name
                var results = await _drugInfoService.SearchDrugInfoAsync(drugName);

                // Stores the search term for display purposes in the results view
                ViewBag.SearchTerm = drugName;
                return View("Results", results);
            }
            catch (Exception ex)
            {
                // Displays any errors encountered during the search process
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Index");
            }
        }
    }
}