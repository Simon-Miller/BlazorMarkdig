using Azure.Storage.Blobs;
using BlazorMarkdig.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using MyOverflow.DataAccess.Blob;
using MyOverflow.Shared;
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyOverflow.Api.Controllers
{
    // this attribute doesn't seem to help at all - in actually seems to make things worse!
    //[ApiController]
    public class MyOverflowController : Controller
    {
        #region constructor

        public MyOverflowController(IQAContext documentStore, IBlobContext azureBlobContext)
        {
            this.documentStore = documentStore;
            this.azureBlobContext = azureBlobContext;
        }

        #endregion


        /// <summary>
        /// 1MB expressed as bytes.
        /// </summary>
        const long MAX_file_size = 1024 * 1024; // 


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
        [RequestSizeLimit(MAX_file_size)]
        [RequestFormLimits(MultipartBodyLengthLimit = MAX_file_size, BufferBodyLengthLimit = MAX_file_size)]
        [Route("MyOverflow/StoreFile")]
        [HttpPost]
        public async Task<ActionResult> StoreFile()
        {
            try
            {
                //var rawContent = new List<byte>(); // I've no idea how big, so needs to be able to grow.  yeuck!
                // this stream doesn't tell us the content length.  So we have to go without.  argh!!
                //while(Request.Body.CanRead)
                //{
                // complans it has to ber async!
                //var nextByte = (byte)Request.Body.ReadByte();

                //var mem = new Memory<byte>();
                //var bytesRead = await Request.Body.ReadAsync(mem);

                //rawContent.Add(nextByte);
                //}


                var readResult = await Request.BodyReader.ReadAsync();

                Func<ReadOnlySequence<byte>, byte[]> fn = seq => 
                {
                    var buffer = new List<byte>();
                    var sr = new SequenceReader<byte>(seq);
                    while(sr.TryRead(out byte value))
                    {
                        buffer.Add(value);
                    }
                    return buffer.ToArray();
                };

                var bytes = fn(readResult.Buffer);

                                   

                var jsonContent = System.Text.UTF8Encoding.UTF8.GetString(bytes);
                var imageFileFromJson = System.Text.Json.JsonSerializer.Deserialize<ImageFile>(jsonContent);

                using (var ms = new MemoryStream((int)Request.ContentLength))
                {
                    await Request.Body.CopyToAsync(ms);
                    var json = System.Text.UTF8Encoding.UTF8.GetString(ms.GetBuffer());

                    // ah!  We've got raw data, but how to deserialize that as Json?
                    System.Text.Json.JsonSerializer.Deserialize<ImageFile>(json);

                    var imageFile = ImageFile.Deserialize(Request.BodyReader.AsStream());

                    ////filesStore.Add(imageFile);

                    //this.azureBlobContext.SetupBlobContainerClient("images");

                    // NOTE: USE OTHER METHOD

                    return new OkResult();
                }

            }
            catch (Exception ex)
            {
                return new ForbidResult();
            }
        }

        /// <summary>
        /// Ensure there's plenty of data allowed in configuration of the service.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("MyOverflow/StoreImageFile")]
        [HttpPost]
        public ActionResult StoreImageFile([FromBody]ImageFile data) // FromBody is needed to instruct model binding
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

        [Route("MyOverflow/StoreImageFile2")]
        [HttpPost]
        [DisableRequestSizeLimit]
        [RequestSizeLimit(MAX_file_size)]
        [RequestFormLimits(MultipartBodyLengthLimit = MAX_file_size, BufferBodyLengthLimit = MAX_file_size)]        
        public ActionResult StoreImageFile2([FromBody] JsonElement body) // FromBody is needed to instruct model binding
        {
            var json = System.Text.Json.JsonSerializer.Serialize(body);
            
            // NOTE:  I don't know why this doesn't work with Microsoft's new JSON deserializer, but that is likely what serialized it?
            //        Visual Studio debugger allows you view 'body' as JSON, and doesn't have a problem with it.
            //        This gives us back a NULL object for the entire json.
            //var dto = System.Text.Json.JsonSerializer.Deserialize<ImageFileDTO>(json, new JsonSerializerOptions() { DefaultBufferSize = 1024 * 1024 });
            
            // SO, BACK TO NEWTONSOFT JSON!
            var dto = JsonConvert.DeserializeObject<ImageFileDTO>(json); 

            var data = ImageFile.From(dto);

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
