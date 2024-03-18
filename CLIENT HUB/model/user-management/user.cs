using BPOI_HUB.model.admin;
using NPOI.SS.Formula.PTG;

namespace BPOI_HUB.model.user_management
{
    public class User
    {
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? Email { get; set; }
        public string[]? Roles { get; set; }

        public string? Department { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime LastLogin { get; set; }

        public bool IsAdmin { get; set; }

        public User()
        {
            UserName      = "";
            FirstName    = "";
            LastName     = "";
            MiddleName   = "";
            Email         = "";
            Roles          = null;
            Department    = "";
            DateCreated  = DateTime.MinValue;
            LastLogin    = DateTime.MinValue;

        }
        public string GetFullName()
        {
            return (LastName + ", " + FirstName + " " + MiddleName).Trim();
        }

        public string DisplayRoles()
        {
            string displayRole = "";
          
            if (Roles != null)
            {
                RoleManager rm = new();
                int role_count = 0;

                foreach (string roleItem in Roles)
                {
                    role_count++;
                    
                    Role r = new();

                    rm.GetRoleById(ref r, int.Parse(roleItem));


                    if (r.Name != null)
                    {
                        if (role_count < Roles.Length)
                            displayRole += r.Name + " / ";
                        else
                            displayRole += r.Name;
                    }

                }
            }

            return displayRole;
            
        }
    }

    
}
