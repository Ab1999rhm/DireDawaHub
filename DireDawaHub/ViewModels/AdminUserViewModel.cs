using System.Collections.Generic;

namespace DireDawaHub.ViewModels;

public class AdminUserViewModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string WorkId { get; set; }
    public IList<string> Roles { get; set; }
}
