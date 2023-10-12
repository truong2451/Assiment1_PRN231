using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IMemberService
    {
        Task<Member> Get(int id);
        IEnumerable<Member> GetAll();
        Task<bool> Add(Member member);
        Task<bool> Update(Member member);
        Task<bool> Delete(int id);
        Task<bool> ChangePassword(int id, string oldPassword, string newPassword);
        Task<Member> CheckLogin(string email, string password);
    }
}
