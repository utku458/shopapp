using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace shopapp.webui.Identity;

public class ApplicationContex: IdentityDbContext<User>
{
    public ApplicationContex(DbContextOptions<ApplicationContex> options): base(options)
    {
        
    }
}