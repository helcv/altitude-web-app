namespace backend.DTOs
{
    public class CreateDto
    {
        public string Id { get; set; }
        public IEnumerable<string> Messages { get; set; }
    }
}
