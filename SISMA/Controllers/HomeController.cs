using Microsoft.AspNetCore.Mvc;
using SISMA.Components;
using SISMA.Models;
using System.Diagnostics;
using System.Net.Mime;
using System.Threading.Tasks;

namespace SISMA.Controllers
{
    /// <summary>
    /// Начален екран
    /// </summary>
    public class HomeController : BaseController
    {

        /// <summary>
        /// Заглавна страница
        /// </summary>
        /// <returns></returns>
        [MenuItem("home")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Обработка на неприхванати грешки
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult GetHelpFile(string chelpName)
        {
            var contentDispositionHeader = new ContentDisposition
            {
                Inline = true,
                FileName = ""
            };

            Response.Headers.Add("Content-Disposition", contentDispositionHeader.ToString());

            return File(Url.Content($"~/chelp/{chelpName.ToLower()}.pdf"), "application/pdf");
        }
    }
}
