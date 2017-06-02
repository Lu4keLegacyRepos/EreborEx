using Phoenix;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EreborPhoenixExtension
{
    public class XmlSerializeHelper<T>
    {
        private string path = Core.Directory + @"\Profiles\XML\";
        public Type _type;

        public XmlSerializeHelper()
        {
            _type = typeof(T);
        }

        public void Save(string filename, object obj)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (File.Exists(path + filename))
                {
                    if (File.Exists(path + filename + "_backup"))
                        File.Delete(path + filename + "_backup");
                    File.Copy(path + filename, path + filename + "_backup");
                }
                var serializer = new XmlSerializer(_type);
                using (var stream = File.Open(path + filename,FileMode.CreateNew,FileAccess.Write,FileShare.None))
                {
                    serializer.Serialize(stream, obj);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.InnerException.ToString()); }

        }
        private bool IsExist(string filename)
        {
            return(File.Exists(path + filename));
        }

        public bool Load(string filename, out T obj)
        {

            if(IsExist(filename))
            {
                obj = Deserialize(filename);
                return true;
            }
            obj = default(T);
            return false;
        }
        private T Deserialize(string filename)
        {

            T XMLOBJ;
            var serializer = new XmlSerializer(_type);
            using (var stream = File.OpenRead(path + filename))
            {
                XMLOBJ = (T)serializer.Deserialize(stream);
            }
            return XMLOBJ;



        }

    }
}
