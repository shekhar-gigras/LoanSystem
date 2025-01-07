using Microsoft.AspNetCore.Http;

namespace Gigras.Software.General.Helper
{
    public static class CommonHelper
    {
        public static async Task<string> SaveFileAsync(int compid, string oldFile, IFormFile file, string modulename)
        {
            string compFolder = "uploads/" + modulename + "/" + compid + "/";
            if (file.Length > 0)
            {
                if (!string.IsNullOrEmpty(oldFile) && System.IO.File.Exists(Directory.GetCurrentDirectory() + "/wwwroot" + oldFile))
                {
                    System.IO.File.Delete(Directory.GetCurrentDirectory() + "/wwwroot" + oldFile);
                }
                // Define a file path (you can adjust this to your storage solution)
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", compFolder);
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                // Ensure the uploads folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return the file path or URL (if you're serving files through an API)
                return "/" + compFolder + fileName; // Adjust this URL to your actual serving path
            }

            return null;
        }
    }
}