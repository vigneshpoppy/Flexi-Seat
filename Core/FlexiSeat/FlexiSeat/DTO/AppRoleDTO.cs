using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.DTO
{
    public class AppRoleDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
