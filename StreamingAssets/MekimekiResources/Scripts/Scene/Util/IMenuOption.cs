public interface IMenuOption
{
    string Name { get; }
    string BuildText();
    void Invoke(int dir);
    object Reference { get; set; }
}