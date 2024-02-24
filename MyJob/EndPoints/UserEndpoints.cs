using MyJob.Database;
using MyJob.DTOs;
using MyJob.Models;

namespace MyJob.EndPoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/User").WithTags(nameof(User));

        LoginEndPoint(group);
    }

    private static void LoginEndPoint(RouteGroupBuilder group)
    {
        group.MapPost("/Login", (LoginUserDTO loginUser, MyJobContext db) =>
        {
            return db.Users.Any(user => user.Email == loginUser.Email &&
                             user.Password == loginUser.Password)
            ? Results.Ok()
            : Results.BadRequest("The email or password is incorrect, please try again");
        })
       .WithName("Login")
       .WithOpenApi();
    }
}
