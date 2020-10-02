using System;
using System.IO;
using System.Threading.Tasks;
using EmployeeChatBot.Models;

namespace URMC.ActiveDirectory {
    public class ActiveDirectory: IActiveDirectory {
        private readonly string _fulldomain;
        private readonly int _port;
        private readonly string _searchBase;

        public ActiveDirectory(ActiveDirectoryOptions options)
        {
            _fulldomain = options.URL; _port = options.Port; _searchBase = options.DirectoryClasses;
        }

        public IActiveDirectorySearch DirectorySearch(Credentials credentials = null)
        {
            if (credentials == null) throw new UnauthorizedAccessException();
            return new ActiveDirectorySearch(credentials, _fulldomain, _port, _searchBase);
        }

        public ActiveDirectoryUser GetUserByUsername(string username, string password = null)
        {
            Credentials credentials = password == null ? null : new Credentials() { Username = username, Password = password };
            using (var search = DirectorySearch(credentials))
            {
                return search.GetUser();
            }
        }

        public Task<ActiveDirectoryUser> AuthenticateAsync(Credentials loginModel)
        {
            return Task<ActiveDirectoryUser>.Run<ActiveDirectoryUser>(() => GetUserByUsername(loginModel.Username, loginModel.Password));
        }
    }
}
