namespace ExamSystem.Web.Data.Entities;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required InventoryStatus InventoryStatus { get; set; }
    public required string Genre { get; set; }
    public required string Author { get; set; }
    public required int Price { get; set; }
}
