using Microsoft.AspNetCore.Identity;

namespace GrpcDemoC.Authentication;

public class ApplicationUser : IdentityUser
{

    public string Address { get; set; } = null!;

}


