using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Request
{
    public class AvailableRoomRequest
    {
        [Required(ErrorMessage = "Ngày check in không được trống!")]

        public DateTime checkInDate { get; set; }
        [Required(ErrorMessage = "Ngày check out không được trống!")]

        public DateTime checkOutDate { get; set; }

        public int adult { get; set; }
        public int children { get; set; }
    }
}
