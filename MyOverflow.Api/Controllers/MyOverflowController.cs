using BlazorMarkdig.Shared.Models;
using BlazorMarkdig.Shared.Proxies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MyOverflow.Api.Controllers
{
    public class MyOverflowController : Controller
    {
        private static List<ImageFile> filesStore = new();

        // GOTCHA!!  Does support wild card.  So "ab/cd/ef.jpg" will work. 
        [Route("MyOverflow/GetFile/{*identifier}", Name = "default")]
        [HttpGet]
        public ActionResult GetFile(string identifier)
        {
            string rawIdentifier = System.Web.HttpUtility.UrlDecode(identifier);

            var file = filesStore.FirstOrDefault(f => f.FileName.ToLower() == rawIdentifier.Trim().ToLower());

            if (file is not null)
            {
                return File(file.RawData, file.MimeType);
            }

            return NotFound();
        }

        /// <summary>
        /// Note that as a HTTP protocol handler, I can't expect a non-json serializable type to work, therefore I need a byte[].
        /// </summary>
        [HttpPost]
        public ActionResult StoreFile(byte[] imageFileBytes)
        {
            var file = ImageFile.Deserialize(imageFileBytes);

            filesStore.Add(file);
            return new OkResult();
        }



        //// GET: MyOverflowController
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //// GET: MyOverflowController/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: MyOverflowController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: MyOverflowController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: MyOverflowController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: MyOverflowController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: MyOverflowController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: MyOverflowController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
