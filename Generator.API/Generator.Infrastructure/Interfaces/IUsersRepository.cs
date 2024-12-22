using Generator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Infrastructure.Interfaces;

public interface IUsersRepository
{
    void Add(Users user);
    Users GetByUsername(string username);
    bool UserExists(string username);
}
