namespace MyWebApi.Services.Interfaces
{
    public interface IUserAccessor
    {
        string GetCurrentUserId();
        string GetCurrentRole();
    }
}
