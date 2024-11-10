namespace backend.Helpers
{
    public class FilterParams
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool? IsVerified { get; set; } = null;
    }
}
