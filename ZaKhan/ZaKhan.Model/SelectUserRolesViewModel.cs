﻿using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using ZaKhan.Data;

namespace ZaKhan.Model
{
    public class SelectUserRolesViewModel
    {
        public SelectUserRolesViewModel()
        {
            this.Roles = new List<SelectRoleEditorViewModel>();
        }


        // Enable initialization with an instance of ApplicationUser:
        public SelectUserRolesViewModel(ApplicationUser user)
            : this()
        {
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;

            var Db = new ApplicationDbContext();

            // Add all available roles to the list of EditorViewModels:
            var allRoles = Db.Roles;
            foreach (var role in allRoles)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new SelectRoleEditorViewModel(ConvetRole(role));
                this.Roles.Add(rvm);
            }
            
            // Set the Selected property to true for those roles for 
            // which the current user is a member:
            //foreach (var userRole in user.Roles)
            //{
            //    var checkUserRole =
            //        this.Roles.Find(r => r.RoleName == userRole.Role.Name);
            //    checkUserRole.Selected = true;
            //}
        }

        private static ApplicationRole ConvetRole(IdentityRole r)
        {
            var role = new ApplicationRole
            {
                Id = r.Id,
                Name = r.Name,
            };
            return role;
        }

        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SelectRoleEditorViewModel> Roles { get; set; }
    }
}
