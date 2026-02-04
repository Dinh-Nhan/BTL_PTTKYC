namespace backend.Dtos.Response
{
    public class ClientResponse
    {
        public int ClientId { get; set; }
        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

    }
}
