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

        public void Serialize(string filename, object obj)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (File.Exists(path + filename)) File.Delete(path + filename);
                var serializer = new XmlSerializer(_type);
                using (var stream = File.OpenWrite(path + filename))
                {
                    serializer.Serialize(stream, obj);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.InnerException.ToString()); }

        }

        public T Deserialize(string filename)
        {

                T XMLOBJ ; 

                var serializer = new XmlSerializer(_type);
                using (var stream = File.OpenRead(path + filename))
                {
                    XMLOBJ = (T)serializer.Deserialize(stream);
                }
                return XMLOBJ;
            


        }

    }
}
