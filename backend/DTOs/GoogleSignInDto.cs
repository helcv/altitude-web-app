namespace backend.DTOs
{
    public class GoogleSignInDto
    {
        public string Id { get; set; }
        public IEnumerable<string> Messages { get; set; }
        public string Token { get; set; }
    }
}
