namespace Domain.Models
{
    public class Player
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public Player() { }

        public Player(int id, string name, int number)
        {
            Id = id;
            Name = name;
            Number = number;
        }
    }
}
