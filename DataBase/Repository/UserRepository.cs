using DataBase.BD;
using System.Security.Cryptography;
using System.Text;

namespace DataBase.Repository
{
    public class UserRepository : IUserRepository
    {
        public void UserAdd(string name, string password, RoleId roleId)
        {
            using (var context = new UserContext())
            {
                if (roleId == RoleId.Admin)
                {
                    var adminCount = context.Users.Count(x => x.RoleId == RoleId.Admin);

                    if (adminCount > 0)
                    {
                        throw new Exception("Администратор может быть только один");
                    }
                }

                var user = new User
                {
                    Name = name,
                    RoleId = roleId,
                    Salt = new byte[16]
                };
                new Random().NextBytes(user.Salt);

                var data = Encoding.ASCII.GetBytes(password).Concat(user.Salt).ToArray();

                using SHA512 shaM = new SHA512Managed();

                user.Password = shaM.ComputeHash(data);

                context.Add(user);

                context.SaveChanges();
            }
        }

        public RoleId UserCheck(string name, string password)
        {
            using (var context = new UserContext())
            {
                var user = context.Users.FirstOrDefault(x => x.Name == name);

                if (user == null)
                {
                    throw new Exception("Пользователь не найден");
                }

                var data = Encoding.ASCII.GetBytes(password).Concat(user.Salt).ToArray();
                using SHA512 shaM = new SHA512Managed();
                var hashedPassword = shaM.ComputeHash(data);

                if (user.Password.SequenceEqual(hashedPassword))
                {
                    return user.RoleId;
                }
                else
                {
                    throw new Exception("Неверный пароль");
                }
            }
        }

        public List<User> GetAllUsers()
        {
            using var context = new UserContext();
            return context.Users.ToList();
        }

        public void DeleteUser(string name)
        {
            using (var context = new UserContext())
            {
                var user = context.Users.FirstOrDefault(x => x.Name == name);

                if (user == null)
                {
                    throw new Exception("Пользователь не найден");
                }

                if (user.Name == "admin")
                {
                    throw new Exception("Админ не может удалить себя");
                }

                try
                {
                    context.Users.Remove(user);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }
    }
}
