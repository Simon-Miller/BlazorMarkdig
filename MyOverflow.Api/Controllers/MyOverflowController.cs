using Azure.Storage.Blobs;
using BlazorMarkdig.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using MyOverflow.DataAccess.Blob;
using MyOverflow.Shared;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyOverflow.Api.Controllers
{
    public class MyOverflowController : Controller
    {
        #region constructor

        public MyOverflowController(IQAContext documentStore, IBlobContext azureBlobContext)
        {
            this.documentStore = documentStore;
            this.azureBlobContext = azureBlobContext;
        }

        #endregion

        private readonly IQAContext documentStore;

        //private static List<ImageFile> filesStore = new();
        private readonly IBlobContext azureBlobContext;


        // GOTCHA!!  Does support wild card.  So "ab/cd/ef.jpg" will work. 
        [Route("MyOverflow/GetFile/{*identifier}")]
        [HttpGet]
        public ActionResult GetFile(string identifier)
        {
            //// NOTE: Since we can predict the URL from Blob storage, we don't need this method any more.
            ///----------------------------------------------

            //string rawIdentifier = System.Web.HttpUtility.UrlDecode(identifier);

            //var file = filesStore.FirstOrDefault(f => f.FileName.ToLower() == rawIdentifier.Trim().ToLower());

            //if (file is not null)
            //{
            //    return File(file.RawData, file.MimeType);
            //}

            return NotFound();
        }

        /// <summary>
        /// As we're not base64 encoding our data, we can't pretend JSON serialization will work at all.
        /// So it seems the best approach I can find is to consider the content of the posted form (body) to be binary,
        /// and extract that out.  Given it is a stream, we could support that directly in ImageFile for improvements!
        /// </summary>
        [Route("MyOverflow/StoreFile")]
        [HttpPost]
        public ActionResult StoreFile()
        {
            using (var ms = new MemoryStream((int)Request.ContentLength))
            {
                ////await Request.Body.CopyToAsync(ms);



                //var imageFile = ImageFile.Deserialize(Request.BodyReader.AsStream());

                ////filesStore.Add(imageFile);

                //this.azureBlobContext.SetupBlobContainerClient("images");

                // NOTE: USE OTHER METHOD

                return new OkResult();
            }
        }



        [Route("MyOverflow/StoreImageFile")]
        [HttpPost]
        public ActionResult StoreImageFile([FromBody] ImageFile data) // FromBody is needed to instruct model binding
        {
            if (data != null)
            {
                // how do you get across the message that this is a static property?
                //MyOverflowController.filesStore.Add(data);

                var task = Task.Run(async () =>
                {
                    var client = await azureBlobContext.SetupBlobContainerClient("images"); // images Folder in URI path. });

                    using (var memStream = new MemoryStream(data.RawData))
                    {
                        
                        var result = await azureBlobContext.Upload(client, data.FileName, memStream);

                        // NOTE: Don't need any stored info.
                    }
                });

                task.Wait(); // JOIN thread.  Yuck!  Don't do this in production, please!!

                return new OkResult(); // NOTE: You really should check for exceptions before returning an OK...
            }

            return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError);
        }

        // TODO: Write code to call this from the UI side.
        [Route("MyOverflow/StoreQuestion")]
        [HttpPost]
        public async Task<ActionResult> StoreQuestion([FromBody] Question question)
        {
            await this.documentStore.StoreQuestion(question);

            return Ok();
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
