namespace Avhrm.Application.Client.Features;
public class GetChildUsersDto
{
    public string Id { get; set; }
    public string PersianName { get; set; }
    public string UserName { get; set; }
    public string DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public string ParentId { get; set; }
    public List<GetChildUsersDto> Children { get; set; }
}
