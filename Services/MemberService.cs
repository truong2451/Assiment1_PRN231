using BusinessObject.Model;
using Repositories.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository repository;

        public MemberService(IMemberRepository repository)
        {
            this.repository = repository;
        }

        public async Task<bool> Add(Member member)
        {
            try
            {
                return await repository.Add(member);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  async Task<bool> Delete(int id)
        {
            try
            {
                var check = await repository.Get(id);
                if (check != null)
                {
                    return await repository.Delete(id);
                }
                else
                {
                    throw new Exception("Not Found Member");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  async Task<Member> Get(int id)
        {
            try
            {
                var check = await repository.Get(id);
                if (check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("Not Found Member");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Member> GetAll()
        {
            try
            {
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Update(Member member)
        {
            try
            {
                var check = await repository.Get(member.MemberId);
                if (check != null)
                {
                    check.Email = member.Email;
                    check.CompanyName = member.CompanyName;
                    check.City = member.City;
                    check.Country = member.Country;

                    return await repository.Update(member.MemberId, check);
                }
                else
                {
                    throw new Exception("Not Found Member");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ChangePassword(int id, string oldPassword, string newPassword)
        {
            try
            {
                var check = await repository.Get(id);
                if (check.Password != oldPassword)
                {
                    check.Password = newPassword;
                    return await repository.Update(id, check);
                }
                else
                {
                    throw new Exception("Password incorrect!!!");
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Member> CheckLogin(string email, string password)
        {
            try
            {
                var account = repository.GetAllWithCondition(x => x.Email == email).FirstOrDefault();

                if (account != null)
                {
                    if(account.Password != password)
                    {
                        throw new Exception("Password incorrect!!!");
                    }
                }
                else
                {
                    throw new Exception("Not Found Account!!!");
                }
                return account;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
