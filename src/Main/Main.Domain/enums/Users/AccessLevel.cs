namespace Main.Domain.enums.Users
{
    public enum AccessLevel
    {
        ReadOnly = 1,      // Только просмотр
        ReadWrite = 2,     // Чтение и запись  
        Admin = 3          // Полный доступ + управление доступом
    }
}
