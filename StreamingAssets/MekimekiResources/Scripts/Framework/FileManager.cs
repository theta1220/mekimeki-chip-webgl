    using System.IO;

    public class FileManager : Singleton<FileManager>
    {
        public bool SaveToJsonWithPanel<T>(T obj, string extension = "")
        {
            var path = Sirius.Engine.Framework.FileUtil.Manager.SaveFilePanel(extension);
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            File.WriteAllText(path, json);
            return true;
        }

        public T LoadFromJsonWithPanel<T>(string extension = "") where T : class
        {
            var path = Sirius.Engine.Framework.FileUtil.Manager.LoadFilePanel(extension);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var json = File.ReadAllText(path);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
    }
