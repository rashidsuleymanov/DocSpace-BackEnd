namespace BackEnd
{
    public static class FileSystem
    {
        public static List<RoomStructure> AvailableFiles { get; set; } = new List<RoomStructure>();
        public class RoomStructure
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string WebUrl { get; set; }
        }
        public class Files
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string WebUrl { get; set; }
        }
        public class Folders
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string WebUrl { get; set; }
        }
    }
}
