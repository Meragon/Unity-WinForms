namespace System.Windows.Forms
{
    public enum AutoCompleteSource
    {
        FileSystem = 1,
        HistoryList = 2,
        RecentlyUsedList = 4,
        AllUrl = 6,
        AllSystemSources = 7,
        FileSystemDirectories = 32,
        CustomSource = 64,
        None = 128,
        ListItems = 256,
    }
}
