namespace BackEnd
{
    public class Common
    {
        
    }
    public static class ColorHelper
    {
        private static readonly string[] Colors = { "#D9534F", // Приглушенный красный
        "#5BC0DE", // Приглушенный синий
        "#F0AD4E", // Приглушенный желтый
        "#C9302C", // Более глубокий красный
        "#0275D8", // Более глубокий синий
        "#F7E7B4"  // Более мягкий желтый
        };

        public static string GetColorForRoom(string roomName)
        {
            int hash = 0;
            for (int i = 0; i < roomName.Length; i++)
            {
                hash = roomName[i] + ((hash << 5) - hash);
            }
            int index = Math.Abs(hash % Colors.Length);
            return Colors[index];
        }
    }
}
