using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService
{
    #region Exceptions
    [Serializable]
    public class StorageException : Exception
    {
        public StorageException(string message) : base(message) { }
    };

    [Serializable]
    public class ResourceExistsException : StorageException
    {
        public ResourceExistsException(string resourceId) : base(String.Format("resource already exists: %0", resourceId)) { ResourceId = resourceId; }

        public string ResourceId { get; internal set; }
    }
    #endregion



    public interface IStorage
    {
        string Upload(Stream stream, string filepath);
        Stream Download(string filename);

    }

    public abstract class AbstractStorage
    {
        protected abstract string GetResourceId(Stream stream, string filepath);
        protected abstract bool ResourceExists(string resourceId);

        protected abstract void InternalUpload(Stream stream, string resourceId);
        public abstract Stream InternalDownload(string resourceId);

        private bool CompareStreams(Stream a, Stream b)
        {
            if (a == null &&
                b == null)
                return true;
            if (a == null ||
                b == null)
            {
                throw new ArgumentNullException(
                    a == null ? "a" : "b");
            }

            if (a.Length < b.Length)
                return false;
            if (a.Length > b.Length)
                return false;


            a.Position = 0;
            b.Position = 0;

            for (int i = 0; i < a.Length; i++)
            {
                int aByte = a.ReadByte();
                int bByte = b.ReadByte();
                if (aByte.CompareTo(bByte) != 0)
                    return false;
            }

            return true;
        }



        public virtual string Upload(Stream stream, string filepath)
        {
            if (stream.Length == 0) throw new StorageException("stream is empty");

            var id = GetResourceId(stream, filepath);

            //Добавляем в id =  Path.Combine(текущий год/текущий месяц,id);
            //DateTime localDate = DateTime.Now;
            //id = Path.Combine(localDate.Year.ToString() + "/" + localDate.Month.ToString(), id);



            if (ResourceExists(id)) throw new ResourceExistsException(id);

            InternalUpload(stream, id);

            using (var checkStream = Download(id))
            {
                if (CompareStreams(stream, checkStream) == false) throw new StorageException("check download failed");
            }


            return id;
        }

        public virtual Stream Download(string resourceId)
        {
            if (!ResourceExists(resourceId)) throw new StorageException(String.Format("file does not exist: %0",resourceId));

            return InternalDownload(resourceId);

        }
    }

    public class LocalDirectoryStorage : AbstractStorage, IStorage
    {
        private string directory;
        private IDirectoryStrategy directoryStrategy;

        public LocalDirectoryStorage(string directory, IDirectoryStrategy directoryStrategy)
        {
            this.directory = directory;
            this.directoryStrategy = directoryStrategy;
        }


        public override Stream InternalDownload(string resourceId)
        {
            var stream = new MemoryStream();
            using (var fs = new FileStream(resourceId, FileMode.Open))
            {
                fs.CopyTo(stream);
            }

            return stream;
        }

        protected override string GetResourceId(Stream stream, string filepath)
        {
            //return Path.Combine(directory, directoryStrategy.DefinePath(stream,filepath));
            DateTime localDate = DateTime.Now;
            return Path.Combine(filepath, Path.Combine(localDate.Year.ToString() + "/" + localDate.Month.ToString(), directoryStrategy.DefinePath(stream, filepath)));
        }

        protected override void InternalUpload(Stream stream, string resourceId)
        {
            var d = Path.GetDirectoryName(Path.Combine(directory, resourceId));
            if (!Directory.Exists(d)) Directory.CreateDirectory(d);

            using (var m = new FileStream(Path.Combine(directory, resourceId), FileMode.CreateNew))
            {
                stream.Position = 0;
                stream.CopyTo(m);
            }
        }

        protected override bool ResourceExists(string resourceId)
        {
            return File.Exists(Path.Combine(directory, resourceId));
        }

    }



    public class MemoryStorage : AbstractStorage, IStorage
    {
        Dictionary<string, Stream> files = new Dictionary<string, Stream>();
        IDirectoryStrategy directoryStrategy=new DefaultDirectoryStrategy();

        public int Count { get { return files.Count; } }


        
        public override Stream InternalDownload(string resourceId)
        {

            var s = new MemoryStream();
            files[resourceId].Position = 0;
            files[resourceId].CopyTo(s);


            return s;

        }


        protected override string GetResourceId(Stream stream, string filepath)
        {
            return directoryStrategy.DefinePath(stream, filepath);
        }

        protected override void InternalUpload(Stream stream, string resourceId)
        {
            var m = new MemoryStream();
            stream.Position = 0;
            stream.CopyTo(m);

            files.Add(resourceId, m);

        }

        protected override bool ResourceExists(string resourceId)
        {
            return files.ContainsKey(resourceId);
        }
    }
}
