using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace BPOI_HUB.pages.libraries
{
    public class SoftwareModel : PageModel
    {
        public Dictionary<string, Dictionary<string, string>> Folders { get; set; }
        public Dictionary<string, Dictionary<string, string>> Items { get; set; }

        private readonly IWebHostEnvironment _webHostEnvironment;

        public SoftwareModel(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public void OnGet()
        {
            string wwwrootPath = _webHostEnvironment.WebRootPath;
          
			string folderPath = Request.Query["folder"];
            string basePath = wwwrootPath + "\\libraries\\software";
            string hrefPath = "/libraries/software";
            string urlPath = "";


			if (folderPath != null) 
            {
                basePath += "\\" + folderPath;
				urlPath += "\\" + folderPath;
                hrefPath += "/" + folderPath;

			}

			string[] curDirectory = Directory.GetDirectories(basePath);
			string[] files = Directory.GetFiles(basePath);


			if (curDirectory.Length == 0 && files.Length == 0)
            {
                return;
            }

            Folders = new Dictionary<string, Dictionary<string, string>>();
			Items = new Dictionary<string, Dictionary<string, string>>();


            foreach (string directory in curDirectory)
            {
                string folderName = Path.GetFileName(directory);

                DateTime dt = Directory.GetLastWriteTime(directory);
                string[] subDirectory = Directory.GetDirectories(directory);
                string[] dirFiles = Directory.GetFiles(directory);

                Folders[folderName] = new()
                {
                    ["url"] = urlPath + "\\" + folderName,
                    ["folderCount"] = subDirectory.Length.ToString(),
                    ["fileCount"] = dirFiles.Length.ToString(),
                    ["lastWriteTime"] = dt.ToString("MMM dd, yyyy")
                };
            }

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);

                DateTime dt = Directory.GetLastWriteTime(file);

                Items[fileName] = new()
                {
                    ["url"] = hrefPath + "/" + fileName,
                    ["filename"] = fileName,
                    ["lastWriteTime"] = dt.ToString("MMM dd, yyyy"),
                    ["img"] = Path.GetExtension(file).Replace(".", "") + ".png"
                };
            }

        }

		public ActionResult Download(string fileName)
		{
			string wwwrootPath = _webHostEnvironment.WebRootPath;
			string serverFilePath = Path.Combine(wwwrootPath, "libraries/software/", fileName);

			return File(serverFilePath, "application/octet-stream", fileName);
		}

	
	}
}
