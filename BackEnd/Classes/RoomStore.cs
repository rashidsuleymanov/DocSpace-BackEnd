namespace BackEnd
{
    public class RoomStore
    {
        public static List<Room> RoomsList { get; set; } = new List<Room>();
        public class Room
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
