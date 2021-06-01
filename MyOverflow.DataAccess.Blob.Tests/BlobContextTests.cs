using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyOverflow.DataAccess.Blob.Tests
{
    [TestClass]
    public class BlobContextTests
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            try
            {
                var context = new BlobContext();
                var client = await context.SetupBlobContainerClient("images");

                string fileName = "s miller signature 2021.png";


                using (var fs = new StreamReader(@"C:\Users\Simon\OneDrive\Desktop\" + fileName))
                {
                    // NOTE: inherits from TextReader, but internal implementation must use some kind of Stream.
                    var result = await context.Upload(client, fileName, fs.BaseStream);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
