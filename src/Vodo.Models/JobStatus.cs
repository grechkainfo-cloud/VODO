namespace Vodo.Models
{
    public enum JobStatus
    {
        Planned = 10, // Запланирована
        InProgress = 20, // В работе
        Suspended = 30, // Приостановлена
        Completed = 40, // Завершена
        Overdue = 50 // Просрочена
    }
}
