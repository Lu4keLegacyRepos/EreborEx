using Phoenix;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EreborPhoenixExtension
{
    public static class XmlSerializeHelper<T>
    {
        private static string path = Core.Directory + @"\Profiles\XML\";
        public static Type _type = typeof(T);


        public static void Save(string filename, object obj)
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
                    File.Delete(path + filename);
                }
                var serializer = new XmlSerializer(_type);
                using (var stream = File.Open(path + filename,FileMode.CreateNew,FileAccess.ReadWrite,FileShare.None))
                {
                    serializer.Serialize(stream, obj);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.InnerException.ToString()); }

        }
        private static bool IsExist(string filename)
        {
            return(File.Exists(path + filename));
        }

        public static void Load(string filename, out T obj)
        {

            if(IsExist(filename))
            {
                obj= Deserialize(filename);
                return;
            }
            obj = default(T);
        }
        private static T Deserialize(string filename)
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
