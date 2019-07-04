using FileService;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace FileServer.Controllers
{
    public class FileController : ApiController
    {
        Logger log = LogManager.GetCurrentClassLogger();

        public HttpResponseMessage Get([FromUri]string filename)
        {
            string codstr = filename.Substring(0, filename.IndexOf("/"));
            string filenameobr = filename.Substring(filename.IndexOf("/") + 1);
            string folderbase;
            switch (codstr)
            {
                case "1":
                    folderbase = "E:/NFILE/Eskiz";
                    break;
                case "2":
                    folderbase = "E:/NFILE/Tr_Sources";
                    break;
                case "3":
                    folderbase = "E:/NFILE/ACY_TP";
                    break;
                case "4":
                    folderbase = "E:/NFILE/ZZ/Eskiz";
                    break;
                case "5":
                    folderbase = "E:/NFILE/FAI";
                    break;
                case "0":
                    folderbase = "E:/NFILE/TEST";
                    break;
                default:
                    folderbase = "E:/NFILE/default_storage";
                    break;
            }

            string path = Path.Combine(folderbase, filenameobr);
            if (!File.Exists(path))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            try
            {

                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("HELLO2"));
                //IStorage storage;
                //if (codstr == "5")
                //{
                //    storage = new LocalDirectoryStorage(folderbase, new DefaultDirectoryStrategy());
                //}
                //else
                //{
                //    storage = new LocalDirectoryStorage(folderbase, new HashDirectoryStrategy(2));
                //}
                IStorage storage = new LocalDirectoryStorage(folderbase, new HashDirectoryStrategy(2));
                //IStorage storage = new LocalDirectoryStorage("c:/temp/storage", new DefaultDirectoryStrategy());



                stream = storage.Download(path);
                bool fullContent = true;
                stream.Position = 0;

                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent;

                response.Content = new StreamContent(stream);
                return response;
            }
            catch (IOException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        public string Post([FromUri]string filename)
        {
            try
            {
                if (this.Request.Content.Headers.ContentLength == 0)
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
                var task = this.Request.Content.ReadAsStreamAsync();
                task.Wait();
                Stream requestStream = task.Result;

                var f = "";
                //try
                //{
                string codstr = filename.Substring(0, filename.IndexOf("/"));
                string filenameobr = filename.Substring(filename.IndexOf("/") + 1);
                string folderbase;
                switch (codstr)
                {
                    case "1":
                        folderbase = "E:/NFILE/Eskiz";
                        break;
                    case "2":
                        folderbase = "E:/NFILE/Tr_Sources";
                        break;
                    case "3":
                        folderbase = "E:/NFILE/ACY_TP";
                        break;
                    case "4":
                        folderbase = "E:/NFILE/ZZ/Eskiz";
                        break;
                    case "5":
                        folderbase = "E:/NFILE/FAI";
                        break;
                    case "0":
                        folderbase = "E:/NFILE/TEST";
                        break;
                    default:
                        folderbase = "E:/NFILE/default_storage";
                        break;
                }
                //IStorage storage;
                //if (codstr == "5")
                //{
                //     storage = new LocalDirectoryStorage(folderbase, new DefaultDirectoryStrategy());
                //}
                //else
                //{
                //     storage = new LocalDirectoryStorage(folderbase, new HashDirectoryStrategy(2));
                //}
                IStorage storage = new LocalDirectoryStorage(folderbase, new HashDirectoryStrategy(2));
                //IStorage storage = new LocalDirectoryStorage("c:/temp/storage", new DefaultDirectoryStrategy());


                try
                {
                    f = storage.Upload(requestStream, folderbase);
                    log.Info(" Метод Upload   STORAGE: " + f);
                }
                catch (ResourceExistsException e)
                {


                    f = e.ResourceId;
                }

                f = f.Substring(f.IndexOf("\\") + 1);
                
                //}
                //catch (IOException)
                //{
                //    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                //}
                requestStream.Close();
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.Created;
                return f;
            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
                
            

        }

        public string Put([FromUri]string filename)
        {
            string codstr = filename.Substring(0, filename.IndexOf("/"));
            string filenameobr = filename.Substring(filename.IndexOf("/") + 1);
            string folderbase;
            switch (codstr)
            {
                case "1":
                    folderbase = "E:/NFILE/Eskiz";
                    break;
                case "2":
                    folderbase = "E:/NFILE/Tr_Sources";
                    break;
                case "3":
                    folderbase = "E:/NFILE/ACY_TP";
                    break;
                case "4":
                    folderbase = "E:/NFILE/ZZ/Eskiz";
                    break;
                case "5":
                    folderbase = "E:/NFILE/FAI";
                    break;
                case "0":
                    folderbase = "E:/NFILE/TEST";
                    break;
                default:
                    folderbase = "E:/NFILE/default_storage";
                    break;
            }

            string path = Path.Combine(folderbase, filenameobr);
            if (!File.Exists(path))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            try
            {
                File.Delete(path);
                log.Info(" Метод Delete   STORAGE: " + folderbase + "   URL: " + filenameobr);

                //Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("HELLO2"));
                //IStorage storage = new LocalDirectoryStorage(folderbase, new HashDirectoryStrategy(2));
                //IStorage storage = new LocalDirectoryStorage("c:/temp/storage", new DefaultDirectoryStrategy());



                //stream = storage.Download(path);
                //bool fullContent = true;
                //stream.Position = 0;

                HttpResponseMessage response = new HttpResponseMessage();
                //response.StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent;
                response.StatusCode = HttpStatusCode.Created;
                //response.Content = new StreamContent(stream);
                return "OK";
            }
            catch (IOException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
    }
}
