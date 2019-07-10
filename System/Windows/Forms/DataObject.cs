namespace System.Windows.Forms
{
    public class DataObject : IDataObject
    {
        private readonly object data;

        public DataObject() { }
        public DataObject(object data) { this.data = data; }
        public DataObject(string format, object data)
        {
            this.data = data;
        }

        public virtual bool ContainsAudio() { return false; }
        public virtual bool ContainsFileDropList() { return false; }
        public virtual bool ContainsImage() { return false; }
        public virtual bool ContainsText() { return false; }
        //public virtual bool ContainsText(TextDataFormat format);
        //public virtual Stream GetAudioStream();
        public virtual object GetData(string format) { return data; }
        public virtual object GetData(Type format) { return data; }
        public virtual object GetData(string format, bool autoConvert) { return data; }
        public virtual bool GetDataPresent(string format) { return false; }
        public virtual bool GetDataPresent(Type format)
        {
            return format.IsInstanceOfType(data);
        }
        public virtual bool GetDataPresent(string format, bool autoConvert) { return false; }
        //public virtual StringCollection GetFileDropList();
        public virtual string[] GetFormats() { return null; }
        public virtual string[] GetFormats(bool autoConvert) { return null; }
        //public virtual Image GetImage();
        public virtual string GetText() { return ""; }
        //public virtual string GetText(TextDataFormat format);
        public virtual void SetAudio(byte[] audioBytes) { }
        //public virtual void SetAudio(Stream audioStream);
        public virtual void SetData(object data) { }
        public virtual void SetData(string format, object data) { }
        public virtual void SetData(Type format, object data) { }
        public virtual void SetData(string format, bool autoConvert, object data) { }
        //public virtual void SetFileDropList(StringCollection filePaths);
        //public virtual void SetImage(Image image);
        public virtual void SetText(string textData) { }
        //public virtual void SetText(string textData, TextDataFormat format);
    }
}
