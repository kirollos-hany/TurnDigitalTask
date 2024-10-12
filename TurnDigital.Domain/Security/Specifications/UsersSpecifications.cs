using TurnDigital.Domain.Security.Entities;

namespace TurnDigital.Domain.Security.Specifications;

public static class UsersSpecifications
{
    public static IQueryable<User> ByEmail(this IQueryable<User> queryable, string email) =>
        queryable.Where(user => user.Email!.Equals(email, StringComparison.OrdinalIgnoreCase));
    
    public static IQueryable<User> ById(this IQueryable<User> queryable, int id) =>
        queryable.Where(user => user.Id == id);
}