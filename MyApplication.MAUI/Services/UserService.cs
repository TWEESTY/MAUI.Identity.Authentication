using MyApplication.MAUI.Services.Interfaces;

namespace MyApplication.MAUI.Services;

public class UserService : IUserService
{
    public string? IdAccount { get; set; }
    public bool? IsExternal { get; set; }

    public void ClearAllInformation()
    {
        IdAccount = null;
        IsExternal = null;
    }
}
