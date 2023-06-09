using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Common.Enums
{
    public class CommonEnums
    {
        public enum Months
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }

        public enum ProjectTabEnums
        {
            BasicDetail = 1,
            IdeaAndInvestment = 2,
            Contacts = 3
        }

        public enum ProjectStatusEnum
        {
            Inactive = 1,
            Declined = 2,
            Active = 3,
            WaitingForApproaval = 4,
            BackedFunding = 5,
            Approved = 6,
            Funded = 7,
            Failed=8
        }

        public enum GetProjectByType {
            Payments = 1,
            EditProject = 2,
            ViewProject = 3
        }
        public enum PaypalAccountStatus
        {
            VERIFIED = 1,
            UNVERIFIED = 2
        }
        public enum TransactionStatus
        {
            created = 1,
            approved = 2,
            failed = 3
        }
        public enum PaymentTransferStatus
        {
            created = 1,
            completed = 2
        }
    }
}
