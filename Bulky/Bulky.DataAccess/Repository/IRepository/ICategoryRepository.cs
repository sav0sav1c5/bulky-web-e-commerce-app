using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        // All methods except two are added By IRepository<Category>
        // It will make sure that we have all base methods from IRepository
        // There are two more that need to be added - Update and Save
        void Update(Category obj);
    }
}
