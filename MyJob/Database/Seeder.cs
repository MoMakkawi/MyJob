using MyJob.Models;

namespace MyJob.Database;

internal static class Seeder
{
    internal static void AdminSeeder(this MyJobContext myJobContext)
    {
        var admin = new User()
        {
            FirstName = "MyJob",
            LastName = "Admin",
            Email = "Admin@MyJob.com",
            Password = "MoMakkawi951753.",
            About = "Admin Account",
            PhoneNumber = "1234567890",
            Specialty = "Administration"
        };

        if (myJobContext.Users.Any(u => u.Email == admin.Email)) return;

        myJobContext.Users.Add(admin);
        myJobContext.SaveChanges();
    }
}
