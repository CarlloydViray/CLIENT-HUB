using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;

namespace BPOI_HUB.Pages.Shared.Core
{
    public class DownloadFileModel : PageModel
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        [TempData]
        public string TempDataValue { get; set; }

        public DownloadFileModel(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public void OnGet()
        {
            TempData.TryGetValue("Title", out var title2);
            TempData.TryGetValue("FileName", out var fname2);

            if(title2 != null && fname2 != null)
                TempData["TempDataValue"] = title2 + "|" + fname2;


        }
        public IActionResult OnPostDownloadFile(string file)
        {
            // Generate the file path on the server.
            string serverFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot/downloads/",file);
            string filename = Path.GetFileName(file);

            string tempDataValue = TempData["TempDataValue"] as string;
            string[] tempDataArr = null;

            if (tempDataValue != null)
                tempDataArr = tempDataValue.Split("|");


            // Check if the file exists.
            if (!System.IO.File.Exists(serverFilePath))
            {
                if (tempDataValue != null)
                {
                    TempData["Title"] = tempDataArr[0];
                    TempData["FileName"] = tempDataArr[1];
                }
                

                TempData["ErrorMessage"] = "Error : File Not Found.";
                return RedirectToPage();
            }


            // Return the file for download.
            var fileStream = new FileStream(serverFilePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/octet-stream", filename);
        }
    }
}
