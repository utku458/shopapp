using System;
using Microsoft.AspNetCore.Identity;

namespace shopapp.webui.Identity;

public class User:IdentityUser, IEquatable<User>
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    public bool Equals(User? other)
    {
        throw new NotImplementedException();
    }
}