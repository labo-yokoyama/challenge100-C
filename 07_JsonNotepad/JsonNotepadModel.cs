using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Livet;

namespace JsonNotepad
{
    public class JsonNotepadModel : NotificationObject
    {
        public NoteItem LoadFile(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath, Encoding.UTF8);
                return JsonConvert.DeserializeObject<NoteItem>(json);
            }
            catch
            {
                return null;
            }
        }

        public void SaveFile(string directory, NoteItem note)
        {
            string filePath = Path.Combine(directory, note.Title + ".json");
            string json = JsonConvert.SerializeObject(note);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        public void DeleteFile(string directory, string title)
        {
            string filePath = Path.Combine(directory, title + ".json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public void InitializeDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public string[] GetNoteFiles(string directory)
        {
            return Directory.GetFiles(directory, "*.json");
        }
    }
}