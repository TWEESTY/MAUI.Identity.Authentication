namespace MyApplication.MAUI.Services.Interfaces;

public interface IUserService
{
    string? IdAccount { get; set;}
    bool? IsExternal { get; set; }

    void ClearAllInformation();
}
