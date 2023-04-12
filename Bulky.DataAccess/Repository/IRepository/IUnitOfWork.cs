using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        CategoryRepository Category { get; }
        ProductRepository Product { get; }
        CompanyRepository Company { get; }
        void Save();
    }
}
