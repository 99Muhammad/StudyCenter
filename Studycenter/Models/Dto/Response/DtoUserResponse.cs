namespace SCMS_back_end.Models.Dto.Response
{
    public class DtoUserResponse
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public IList<string> Roles { get; set; }
        public string Message { get; set; }            
    }
}
