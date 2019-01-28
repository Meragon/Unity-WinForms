namespace System.Windows.Forms
{
    public interface IDataObject
    {
        object GetData(string format);
        object GetData(Type format);
        object GetData(string format, bool autoConvert);
        bool GetDataPresent(string format);
        bool GetDataPresent(Type format);
        bool GetDataPresent(string format, bool autoConvert);
        string[] GetFormats();
        string[] GetFormats(bool autoConvert);
        void SetData(object data);
        void SetData(string format, object data);
        void SetData(Type format, object data);
        void SetData(string format, bool autoConvert, object data);
    }
}
