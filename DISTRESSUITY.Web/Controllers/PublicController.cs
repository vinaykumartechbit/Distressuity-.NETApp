using DISTRESSUITY.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using DISTRESSUITY.Common.Constants;
using DISTRESSUITY.Common.Utility;


namespace DISTRESSUITY.Web.Controllers
{
    [RoutePrefix("api/Public")]
    public class PublicController : ApiController
    {
        #region Parameters
        private IUserService _userService;
        #endregion

        #region Constructors
        public PublicController(IUserService UserService)
        {
            _userService = UserService;
        }
        #endregion

        #region HelperMethods

        public class FileResult : IHttpActionResult
        {
            private readonly string filePath;
            private readonly string contentType;
            private readonly int width;
            private readonly int height;

            public FileResult(string filePath, int width, int height, string contentType = null)
            {
                this.filePath = filePath;
                this.contentType = contentType;
                this.width = width;
                this.height = height;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.Run(() =>
                {
                    var result = getResizedImage(filePath, width, height);
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(result)
                    };

                    var contentType = this.contentType ?? MimeMapping.GetMimeMapping(Path.GetExtension(filePath));
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                    return response;
                }, cancellationToken);
            }
        }
        static byte[] getResizedImage(String path, int width, int height)
        {
            try
            {
                using (Bitmap imgIn = new Bitmap(path))
                {
                    System.IO.MemoryStream outStream = new System.IO.MemoryStream();
                    if (imgIn.Width < width)
                    {
                        using (Bitmap imgOut = new Bitmap(width, height))
                        {
                            int xValue = (int)(width - imgIn.Width) / 2;
                            int yValue = (int)(height - imgIn.Height) / 2;
                            Graphics g = Graphics.FromImage(imgOut);
                            g.Clear(Color.White);
                            g.DrawImage(imgIn, new Rectangle(xValue,yValue, width, height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
                            imgOut.Save(outStream, getImageFormat(path));

                            return outStream.ToArray();
                        }
                        //imgIn.Save(outStream, getImageFormat(path));
                        //return outStream.ToArray();
                    }

                    double y = imgIn.Height;
                    double x = imgIn.Width;

                    double factor = 1;
                    if (width > 0)
                    {
                        factor = width / x;
                    }
                    else if (height > 0)
                    {
                        factor = height / y;
                    }

                    //using (Bitmap imgOut = new Bitmap((int)(x * factor), (int)(y * factor)))
                    //{
                    //    Graphics g = Graphics.FromImage(imgOut);
                    //    g.Clear(Color.White);
                    //    g.DrawImage(imgIn, new Rectangle(0, 0, (int)(factor * x), (int)(factor * y)), new Rectangle(0, 0, (int)x, (int)y), GraphicsUnit.Pixel);
                    //    imgOut.Save(outStream, getImageFormat(path));

                    //    return outStream.ToArray();
                    //}
                    using (Bitmap imgOut = new Bitmap(width, height))
                    {
                        Graphics g = Graphics.FromImage(imgOut);
                        g.Clear(Color.White);
                        g.DrawImage(imgIn, new Rectangle(0, 0, width, height), new Rectangle(0, 0, (int)x, (int)y), GraphicsUnit.Pixel);
                        imgOut.Save(outStream, getImageFormat(path));

                        return outStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
           
            //outStream.Seek(0, SeekOrigin.Begin);
            //System.Web.Mvc.FileStreamResult fileStreamResult = new System.Web.Mvc.FileStreamResult(outStream, "image/png");
            //return fileStreamResult;
        }

        string getContentType(String path)
        {
            switch (Path.GetExtension(path))
            {
                case ".bmp": return "Image/bmp";
                case ".gif": return "Image/gif";
                case ".jpg": return "Image/jpeg";
                case ".png": return "Image/png";
                default: break;
            }
            return "";
        }

        static ImageFormat getImageFormat(String path)
        {
            switch (Path.GetExtension(path))
            {
                case ".bmp": return ImageFormat.Bmp;
                case ".gif": return ImageFormat.Gif;
                case ".jpg": return ImageFormat.Jpeg;
                case ".png": return ImageFormat.Png;
                default: break;
            }
            return ImageFormat.Png;
        }



        #endregion

        #region GetMethods

        [AllowAnonymous]
        [HttpGet]
        [Route("GetFileStream")]
        public IHttpActionResult GetFileStream(string filename, int id = 0, int w = 0, int h = 0)
        {
            var imagePath = string.Empty;
            imagePath = HttpContext.Current.Server.MapPath(@filename);
            if (File.Exists(imagePath))
                return new FileResult(imagePath, w, h, "image/jpeg");

            return BadRequest("Image doesnot exist");
        }

        [HttpGet]
        public IHttpActionResult DownloadDocument(string fileName)
        {
            var stream = new MemoryStream();
            //int projectId = id.Decrypt();
            //var path = HttpContext.Current.Server.MapPath(@ImagePathConstants.ProjectFiles_Folder + projectId + "/" + fileName);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(stream.GetBuffer())
            };
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("")
            {
                FileName = "test.pdf"
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = ResponseMessage(result);

            return response;
        }

        #endregion

    }
}
