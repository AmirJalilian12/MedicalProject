namespace Store.Domain.Entities;

public class Disease
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PersianName { get; set; } = string.Empty;
    public string Symptoms { get; set; } = string.Empty; // علائم
    public string Treatment { get; set; } = string.Empty; // درمان پیشنهادی
}
