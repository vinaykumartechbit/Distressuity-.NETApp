using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.DAL.Interfaces
{
    public interface ILogInfo
    {
        //DateTime CreatedOn { get; set; }
        int CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
        Nullable<DateTime> UpdatedDate { get; set; }
        Nullable<int> UpdatedBy { get; set; }
    }
}
