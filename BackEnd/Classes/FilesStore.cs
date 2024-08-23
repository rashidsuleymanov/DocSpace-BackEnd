namespace BackEnd
{
    public static class FilesStore
    {
        public static List<AvailableFile> AvailableFiles { get; set; } = new List<AvailableFile>();
        public class AvailableFile
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string WebUrl { get; set; }
        }

    }
}
