using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.DTO.AppRoleDTOs
{
  public class CreateAppRoleDTO
  {
    public string Name { get; set; }
    public string? Description { get; set; }
  }
}
