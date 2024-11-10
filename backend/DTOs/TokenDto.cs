namespace backend.DTOs
{
    public class TokenDto
    {
        public string Id { get; set; }
        public string? Token { get; set; }
        public string? Provider { get; set; }
        public bool Is2FaRequired { get; set; }
    }
}
