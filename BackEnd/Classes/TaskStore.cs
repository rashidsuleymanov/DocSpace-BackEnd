namespace BackEnd.Pages
{
    public static class TaskStore
    {
        
        public static List<Task> Tasks { get; set; }
        public class Task
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Priority { get; set; }
            public List<Attachment> Attachments { get; set; } = new List<Attachment>();
        }
        public class Attachment
        {
            public int FileId { get; set; }
            public string FileName { get; set; }
            public string WebUrl { get; set; }
            // Другие поля, если необходимо
        }

        static TaskStore()
        {
            // Заглушка: создаем рандомные задачи
            Tasks = new List<Task>
            {
                new Task { Id = 1, Name = "Follow up with Leads", Description = "Contact potential leads to discuss product offerings and schedule meetings.", Priority = "High" },
                new Task { Id = 2, Name = "Prepare Sales Report", Description = "Compile weekly sales data and prepare a detailed report for the management meeting.", Priority = "Medium" },
                new Task { Id = 3, Name = "Update CRM", Description = "Ensure all customer interactions are recorded accurately in the CRM system.", Priority = "Low" }
            };
        }
    }
}
