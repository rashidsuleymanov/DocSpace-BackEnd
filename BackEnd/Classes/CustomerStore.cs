using static BackEnd.CustomerStore;

namespace BackEnd
{
    public static class CustomerStore
    {
        public static List<Customer> Customers { get; set; }
        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string RoomName { get; set; }
            public int RoomId { get; set; }
        }
        static CustomerStore()
        {
            // Заглушка: создаем рандомные задачи
            Customers = new List<Customer>
{
    new Customer { Id = 1, Name = "Tech Solutions Inc.", Description = "A mid-sized IT service provider looking to enhance their client management process.", RoomId = 337898},
    new Customer { Id = 2, Name = "HealthCare Co.", Description = "A healthcare service provider aiming to streamline patient management and improve communication."},
    new Customer { Id = 3, Name = "EduWorld", Description = "An educational institution seeking to manage student information and improve administrative efficiency."},
    new Customer { Id = 4, Name = "RetailMart", Description = "A retail chain interested in improving customer relationship management and sales tracking."},
    new Customer { Id = 5, Name = "FinancePro Ltd.", Description = "A financial advisory firm needing a robust system to manage client portfolios and interactions."},
    new Customer { Id = 6, Name = "LogisticsCorp", Description = "A logistics company looking to optimize their customer service and order tracking processes."},
    new Customer { Id = 7, Name = "TravelExperts", Description = "A travel agency aiming to better manage customer bookings and travel itineraries."},
    new Customer { Id = 8, Name = "RealEstate Solutions", Description = "A real estate firm looking to enhance their client and property management capabilities."},
    new Customer { Id = 9, Name = "MarketingGurus", Description = "A marketing agency seeking to improve their client campaign management and reporting."},
    new Customer { Id = 10, Name = "ManufactureMax", Description = "A manufacturing company in need of a system to manage their supplier and client relationships.",RoomId=280258,RoomName="Testing"}
};
        }
    }
}
