using Assignment1_API.ViewModel;
using Assignment1_PRN231.Models;
using AutoMapper;
using BusinessObject.Model;

namespace Assignment1_API
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Product, ProductVM>();
            CreateMap<ProductVM, Product>();

            CreateMap<Product, ProductUpdateVM>(); 
            CreateMap<ProductUpdateVM, Product>();


            CreateMap<Member, MemberVM>();
            CreateMap<MemberVM, Member>();
            
            CreateMap<Member, MemberUpdateVM>();
            CreateMap<MemberUpdateVM, Member>();
        }
    }
}
