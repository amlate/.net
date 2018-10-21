using System;
using System.IO;
using System.Web;

namespace Project.Web.Provider
{
    public class FileUploadHelper
    {
        public static string ProcessUpload(HttpPostedFileBase file, string root)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            if (file?.ContentLength > 0)
            {
                string path = Path.Combine(root, fileName);
                file.SaveAs(path);

                return fileName;
            }

            return string.Empty;
        }

        public static string Process(HttpPostedFileBase file, string root, string name)
        {
            var fileName = $"{name}{Path.GetExtension(file.FileName)}";

            if (file?.ContentLength > 0)
            {
                string path = Path.Combine(root, fileName);
                file.SaveAs(path);

                return fileName;
            }

            return string.Empty;
        }
    }
}