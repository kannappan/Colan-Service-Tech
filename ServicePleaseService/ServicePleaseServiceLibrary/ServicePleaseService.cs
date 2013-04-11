using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web.Security;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using ServicePleaseServiceLibrary.Interfaces;
using ServicePleaseServiceLibrary.Model;
using ServicePleaseService.Helpers;
using System.Configuration;
using System.Collections.Specialized;
using ServicePleaseServiceLibrary.Helpers;
using System.IO;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Web.Hosting;
using PushSharp;
using PushSharp.Common;
using PushSharp.Apple;

namespace ServicePleaseServiceLibrary
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[ServiceKnownType(typeof(BlobPacket))]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
	public class ServicePleaseService : IServicePleaseService
	{
        #region IServicePleaseService Members
        #region Role Methods
        public List<Role> GetRoles()
        {
            List<Role> roleList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var roles = svcPlsContext.Roles.OrderBy(r => r.Name);

                    if (roles != null)
                    {
                        roleList = roles.ToList<Role>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetRoles().  Error: {0}\n{1}", ex.Message,
                             ex.InnerException != null ? ex.InnerException.Message : string.Empty));

                roleList = null;

            }

            return roleList;
        }

        public Role AddRole(Role role)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.Roles.InsertOnSubmit(role);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddRole().  Error: {0}", ex.Message);

                role = null;
            }

            return role;
        }

        public Role GetRoleForUser(Guid userId)
        {
            Role foundRole = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var role = from roleEntry in svcPlsContext.Roles
                               from userRole in svcPlsContext.UserRoles
                               where userRole.UserId == userId &&
                                     roleEntry.RoleId == userRole.RoleId
                               select roleEntry;

                    if (role != null)
                    {
                        foundRole = role.FirstOrDefault<Role>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetRoleForUser().  Error: {0}\n{1}", ex.Message,
                             ex.InnerException != null ? ex.InnerException.Message : string.Empty));

                foundRole = null;
            }

            return foundRole;
        }

        //public UserRole AddUserRole(UserRole userRole)
        //{
        //    try
        //    {
        //        using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
        //        {
        //            svcPlsContext.UserRoles.InsertOnSubmit(userRole);

        //            svcPlsContext.SubmitChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Write("Error in AddUserRole().  Error: {0}", ex.Message);

        //        userRole = null;
        //    }

        //    return userRole;
        //}

        public FeatureRole AddFeatureRole(FeatureRole featureRole)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.FeatureRoles.InsertOnSubmit(featureRole);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddFeatureRole().  Error: {0}", ex.Message);

                featureRole = null;
            }

            return featureRole;
        }

        public bool IsUserAuthorizedForFeature(Guid featureId, Guid roleId)
        {
            bool userIsAuthorized = false;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var featureRoles = svcPlsContext.FeatureRoles.Where(f => f.FeatureId == featureId &&
                                                                             f.RoleId == roleId);

                    if (featureRoles != null && featureRoles.Count() > 0)
                    {
                        userIsAuthorized = true;
                    }
                    else
                    {
                        userIsAuthorized = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in IsUserAuthorizedForFeature().  Error: {0}\n{1}", ex.Message,
                             ex.InnerException != null ? ex.InnerException.Message : string.Empty));

                userIsAuthorized = false;
            }

            return userIsAuthorized;
        }
        #endregion Role Methods

        #region ServicePlan Methods
        /// <summary>
        /// List<ServicePlanType> GetServicePlanTypes()
        /// </summary>
        /// <returns></returns>
        public List<ServicePlanType> GetServicePlanTypes()
        {
            List<ServicePlanType> servicePlanTypes = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var svcPlanTypes = svcPlsContext.ServicePlanTypes.OrderBy(s => s.Name);

                    if (svcPlanTypes != null)
                    {
                        servicePlanTypes = svcPlanTypes.ToList<ServicePlanType>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetServicePlanTypes().  Error: {0}\n{1}", ex.Message,
                             ex.InnerException != null ? ex.InnerException.Message : string.Empty));

                servicePlanTypes = null;
            }

            return servicePlanTypes;
        }

        /// <summary>
        /// List<ServicePlanInfo> GetServicePlanInfo()
        /// </summary>
        /// <returns></returns>
        public List<ServicePlanInfo> GetServicePlanInfo()
        {
            List<ServicePlanInfo> servicePlanInfos = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var svcPlanInfos = svcPlsContext.ServicePlanInfos.OrderBy(s => s.SubscriptionEnd);

                    if (svcPlanInfos != null)
                    {
                        servicePlanInfos = svcPlanInfos.ToList<ServicePlanInfo>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetServicePlanInfo().  Error: {0}\n{1}", ex.Message,
                             ex.InnerException != null ? ex.InnerException.Message : string.Empty));

                servicePlanInfos = null;
            }

            return servicePlanInfos;
        }

        /// <summary>
        /// List<ServicePlanInfo> GetServicePlanInfoByLocation()
        /// </summary>
        /// <returns></returns>
        public List<ServicePlanInfo> GetServicePlanInfoByLocation(Guid locationId)
        {
            List<ServicePlanInfo> servicePlanInfos = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var svcPlanInfos = from location in svcPlsContext.Locations
                                       from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                       where location.LocationId == locationId &&
                                             location.ServicePlanInfoId == servicePlanInfo.ServicePlanInfoId
                                       select servicePlanInfo;

                    if (svcPlanInfos != null)
                    {
                        servicePlanInfos = svcPlanInfos.ToList<ServicePlanInfo>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetServicePlanInfoByLocation(Guid locationId).  Error: {0}\n{1}", ex.Message,
                             ex.InnerException != null ? ex.InnerException.Message : string.Empty));

                servicePlanInfos = null;
            }

            return servicePlanInfos;
        }

        #endregion ServicePlan Methods

        #region User Methods
        /// <summary>
        /// bool ValidateUser(string userId, string password)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidateUser(string userId, string password)
        {
            bool isUserValid = false;

            try
            {
                string hashedPassword = Cryptography.GetChecksum(password);

                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var user = svcPlsContext.Users.Where(u => u.UserName.ToLower() == userId.ToLower() &&
                                                              u.HashedPassword == hashedPassword);

                    if (user != null && user.Count() > 0)
                    {
                        isUserValid = true;
                    }
                    else
                    {
                        isUserValid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in ValidateUser().  Error: {0}", ex.Message);

                isUserValid = false;
            }

            return isUserValid;
        }

        /// <summary>
        /// User CreateUser(string userId, string password, string firstName, string middleName, string lastName, Guid locationId)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="firstName"></param>
        /// <param name="middleName"></param>
        /// <param name="lastName"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public User CreateUser(string userId, string password, string firstName, string middleName, string lastName, Guid locationId)
        {
            User newUser = null;

            try
            {
                newUser = new User()
                {
                    UserId = Guid.NewGuid(),
                    UserName = userId,
                    HashedPassword = Cryptography.GetChecksum(password),
                    FirstName = firstName,
                    MiddleName = middleName ?? string.Empty,
                    LastName = lastName,
                    CreateDate = DateTime.Now.ToString("yyyy-mm-dd HH:MM:ss"),
                    EditDate = DateTime.Now.ToString("yyyy-mm-dd HH:MM:ss")
                };

                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.Users.InsertOnSubmit(newUser);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in CreateUser().  Error: {0}", ex.Message);

                newUser = null;
            }

            return newUser;
        }

        /// <summary>
        /// User AddUser(User user)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User AddUser(User user)
        {
            string message = string.Empty;
            User newUser = null;
            UserOrganization uo = null;
            try
            {
                newUser = new User()
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    HashedPassword = Cryptography.GetChecksum(user.HashedPassword),
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName ?? string.Empty,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    CreateDate = user.CreateDate,
                    EditDate = user.EditDate,
                    Address1 = user.Address1,
                    Address2 = user.Address2,
                    Address3 = user.Address3,
                    Status = user.Status,
                    UserRoles = user.UserRoles
                };

                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.Users.InsertOnSubmit(newUser);

                    svcPlsContext.SubmitChanges();

                    //Add UserOrganization
                    uo = new UserOrganization()
                    {
                        UserOrganizationId = Guid.NewGuid(),
                        UserId = user.UserId,
                        OrganizationId = Guid.Parse("F2D74EDB-A257-4FD5-A693-002FA24CD882"),
                        CreateDate = user.CreateDate,
                        EditDate = user.EditDate
                    };
                    svcPlsContext.UserOrganizations.InsertOnSubmit(uo);
                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddUser().  Error: {0}", ex.Message);

                user = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "AddUser";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);

            }

            return newUser;
        }

        /// <summary>
        /// User GetUser(string userName)
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public User GetUser(string userName)
        {
            User user = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var users = svcPlsContext.Users.Where(u => u.UserName == userName);

                    if (users != null)
                    {
                        user = users.FirstOrDefault();

                        var urole = svcPlsContext.UserRoles.Where(u => u.UserId == user.UserId);

                        if (urole != null)
                        {                            
                            user.UserRoles.Assign(urole);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetUser(string userName).  Error: {0}", ex.Message);

                user = null;
            }

            return user;
        }

        /// <summary>
        /// List<User> GetUsers()
        /// </summary>
        /// <returns></returns>
        public List<User> GetUsers()
        {
            List<User> userList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var users = svcPlsContext.Users.OrderBy(u => u.UserName);

                    if (users != null)
                    {
                        userList = users.ToList<User>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetUsers().  Error: {0}", ex.Message);

                userList = null;
            }

            return userList;
        }

        /// <summary>
        /// User UpdateUser(Guid userId , User user)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User UpdateUser(Guid userId, User user)
        {
            User us = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific User from Database
                    us = (from u in svcPlsContext.Users
                          where u.UserId == userId
                          select u).FirstOrDefault();

                    if (us != null)
                    {
                        var userRole = (from ur in svcPlsContext.UserRoles
                                        where ur.UserId == user.UserId
                                        select ur);

                        if (userRole != null)
                        {
                            // Delete the userROle details based on userId
                            svcPlsContext.UserRoles.DeleteAllOnSubmit(userRole.ToList<UserRole>());
                            svcPlsContext.SubmitChanges();
                        }

                        //us.UserName = user.UserName;
                        //us.HashedPassword = Cryptography.GetChecksum(user.HashedPassword);
                        us.FirstName = user.FirstName;
                        us.MiddleName = user.MiddleName;
                        us.LastName = user.LastName;
                        us.Phone = user.Phone;
                        us.Email = user.Email;
                        us.Address1 = user.Address1;
                        us.Address2 = user.Address2;
                        us.Address3 = user.Address3;
                        us.EditDate = user.EditDate;
                        us.Status = user.Status;
                        us.UserRoles = user.UserRoles;

                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateUser().  Error: {0}", ex.Message);

                us = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "UpdateUser";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

            }
            return us;
        }

        /// <summary>
        /// string String DeleteUser(Guid userId)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string DeleteUser(Guid userId)
        {
            User us = null;
            Problem pro = null;
            Ticket tic = null;

            RecapSettingUser rsu = null;
            LikeUnlike lu = null;
            Snooze sno = null;
            Solution sol = null;
            string message = string.Empty;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific organization from Database
                    tic = (from t in svcPlsContext.Tickets
                           where t.UserId == userId
                           select t).FirstOrDefault();
                    pro = (from p in svcPlsContext.Problems
                           where p.UserId == userId
                           select p).FirstOrDefault();
                    sol = (from s in svcPlsContext.Solutions
                           where s.UserId == userId
                           select s).FirstOrDefault();
                    sno = (from s in svcPlsContext.Snoozes
                           where s.UserId == userId
                           select s).FirstOrDefault();
                    lu = (from l in svcPlsContext.LikeUnlikes
                          where l.UserId == userId
                          select l).FirstOrDefault();
                    rsu = (from r in svcPlsContext.RecapSettingUsers
                           where r.UserId == userId
                           select r).FirstOrDefault();


                    if (tic != null)
                    {
                        message = "This User is used by one of the Ticket in TicketMonitorRows.";
                    }
                    else if (pro != null)
                    {
                        message = "This User is used by one of the Problem.";
                    }
                    else if (sol != null)
                    {
                        message = "This User is used by one of the Solution.";
                    }
                    else if (sno != null)
                    {
                        message = "This User is used by one of the Snooze.";
                    }
                    else if (lu != null)
                    {
                        message = "This User is used by one of the LikeUnlike.";
                    }
                    else if (rsu != null)
                    {
                        message = "This User is used by one of the RecapSettingUser.";
                    }
                    else
                    {
                        //Get the specific User from Database
                        us = (from u in svcPlsContext.Users
                              where u.UserId == userId
                              select u).FirstOrDefault();

                        if (us != null)
                        {
                            //Delete UserRole
                            var delUserRole = (from ur in svcPlsContext.UserRoles
                                               where ur.UserId == userId
                                               select ur).FirstOrDefault();

                            if (delUserRole != null)
                            {
                                svcPlsContext.UserRoles.DeleteOnSubmit(delUserRole);
                                svcPlsContext.SubmitChanges();
                            }

                            //Delete UserOrganization
                            var delUsrOrg = (from u in svcPlsContext.UserOrganizations
                                             where u.UserId == userId
                                             select u).FirstOrDefault();

                            if (delUsrOrg != null)
                            {
                                svcPlsContext.UserOrganizations.DeleteOnSubmit(delUsrOrg);
                                svcPlsContext.SubmitChanges();
                            }

                            //Delete DeviceDetail
                            var devDetail = (from dd in svcPlsContext.DeviceDetails
                                             where dd.UserId == userId
                                             select dd).ToList<DeviceDetail>();

                            if (devDetail != null)
                            {
                                svcPlsContext.DeviceDetails.DeleteAllOnSubmit(devDetail);
                                svcPlsContext.SubmitChanges();
                            }

                            // Delete the User
                            svcPlsContext.Users.DeleteOnSubmit(us);
                            svcPlsContext.SubmitChanges();

                            message = "Selected User Deleted Successfully";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteUser().  Error: {0}", ex.Message);

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "DeleteUser";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {

                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();


                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }
                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }

        /// <summary>
        /// Organization GetUserOrganization(Guid userId)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Organization GetUserOrganization(Guid userId)
        {
            Organization organization = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var userOrgz = from userOrg in svcPlsContext.UserOrganizations
                                   from org in svcPlsContext.Organizations
                                   where userOrg.UserId == userId &&
                                         org.OrganizationId == userOrg.OrganizationId
                                   select userOrg;

                    if (userOrgz != null)
                    {
                        organization = userOrgz.FirstOrDefault().Organization;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetUserOrganization().  Error: {0}", ex.Message);

                organization = null;
            }

            return organization;
        }
        #endregion UserMethods

        #region Organization Methods
        /// <summary>
        /// List<Organization> GetOrganizations()
        /// </summary>
        /// <returns></returns>
        public List<Organization> GetOrganizations()
        {
            List<Organization> organizationList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var organizations = svcPlsContext.Organizations.OrderBy(o => o.OrganizationName);

                    if (organizations != null)
                    {
                        organizationList = organizations.ToList<Organization>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetOrganizations().  Error: {0}", ex.Message);

                organizationList = null;
            }

            return organizationList;
        }

        /// <summary>
        /// Organization AddOrganization(Organization organization)
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        public Organization AddOrganization(Organization organization)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.Organizations.InsertOnSubmit(organization);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddOrganization().  Error: {0}", ex.Message);

                organization = null;
            }

            return organization;
        }

        /// <summary>
        /// Organization UpdateOrganization(Organization organization, Guid organizationId)
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public Organization UpdateOrganization(Organization organization, Guid organizationId)
        {
            Organization org = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific organization from Database
                    org = (from o in svcPlsContext.Organizations
                           where o.OrganizationId == organizationId
                           select o).FirstOrDefault();

                    if (org != null)
                    {
                        org.OrganizationName = organization.OrganizationName;
                        //org.CreateDate = organization.CreateDate;
                        org.EditDate = organization.EditDate;

                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateOrganization().  Error: {0}", ex.Message);

                org = null;
            }

            return org;
        }

        /// <summary>
        /// bool DeleteOrganization(Guid organizationId)
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public bool DeleteOrganization(Guid organizationId)
        {
            bool result = false;

            Organization org = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific organization from Database
                    org = (from o in svcPlsContext.Organizations
                           where o.OrganizationId == organizationId
                           select o).FirstOrDefault();

                    if (org != null)
                    {
                        // Delete the organization
                        svcPlsContext.Organizations.DeleteOnSubmit(org);

                        // Save to database
                        svcPlsContext.SubmitChanges();

                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteOrganization().  Error: {0}", ex.Message);

                result = false;
            }

            return result;
        }
        #endregion Organization Methods

        #region Category Methods
        /// <summary>
        /// List<Category> GetCategories()
        /// </summary>
        /// <returns></returns>
        public List<Category> GetCategories()
        {
            List<Category> categoryList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var categories = svcPlsContext.Categories.OrderBy(c => c.CategoryName);

                    if (categories != null)
                    {
                        categoryList = categories.ToList<Category>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetCategories().  Error: {0}", ex.Message);

                categoryList = null;
            }

            return categoryList;
        }

        /// <summary>
        /// Category GetCategory(string categoryId)
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public Category GetCategory(string categoryId)
        {
            Category category = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var categories = svcPlsContext.Categories.Where(c => c.CategoryId == Guid.Parse(categoryId));

                    if (categories != null)
                    {
                        category = categories.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetCategory(string categoryId).  Error: {0}", ex.Message);

                category = null;
            }

            return category;
        }

        /// <summary>
        /// List<Category> GetCategoriesByOrganization(string organizationId)
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public List<Category> GetCategoriesByOrganization(string organizationId)
        {
            List<Category> categoryList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var categories = svcPlsContext.Categories.Where(c => c.OrganizationId == Guid.Parse(organizationId)).OrderBy(c => c.CategoryName);

                    if (categories != null)
                    {
                        categoryList = categories.ToList<Category>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetCategories().  Error: {0}", ex.Message);

                categoryList = null;
            }

            return categoryList;
        }

        /// <summary>
        /// string AddCategory(Category category)
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public string AddCategory(Category category)
        {
            string message = string.Empty;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.Categories.InsertOnSubmit(category);

                    svcPlsContext.SubmitChanges();
                    message = "New Category Added Successfully";
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddCategory().  Error: {0}", ex.Message);

                category = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "AddCategory";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();
                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }

                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);

            }

            return message;
        }

        /// <summary>
        /// string UpdateCategory(Category category, Guid categoryId)
        /// </summary>
        /// <param name="category"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public string UpdateCategory(Category category, Guid categoryId)
        {
            Category cat = null;
            string message = string.Empty;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific organization from Database
                    cat = (from c in svcPlsContext.Categories
                           where c.CategoryId == categoryId
                           select c).FirstOrDefault();

                    if (cat != null)
                    {
                        cat.CategoryIcon = category.CategoryIcon;
                        cat.CategoryName = category.CategoryName;
                        cat.EditDate = category.EditDate;
                        cat.Description = category.Description;
                        //Save to database
                        svcPlsContext.SubmitChanges();
                        message = "Selected Category Updated Successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateCategory().  Error: {0}", ex.Message);

                cat = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "UpdateCategory";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {

                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();


                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }
                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }

        /// <summary>
        /// string DeleteCategory(Guid categoryId)
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public string DeleteCategory(Guid categoryId)
        {
            Category cat = null;
            Ticket tic = null;
            RecapSettingCategory rsc = null;
            string message = string.Empty;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific organization from Database
                    tic = (from t in svcPlsContext.Tickets
                           where t.CategoryId == categoryId
                           select t).FirstOrDefault();

                    rsc = (from r in svcPlsContext.RecapSettingCategories
                           where r.CategoryId == categoryId
                           select r).FirstOrDefault();

                    if (tic != null)
                    {
                        message = "This Category is used by one of the Ticket in TicketMonitorRows.";

                    }

                    else if (rsc != null)
                    {
                        message = "This Category is used by one of the RecapSettingCategory.";
                    }
                    else
                    {
                        //Get the specific organization from Database
                        cat = (from c in svcPlsContext.Categories
                               where c.CategoryId == categoryId
                               select c).FirstOrDefault();

                        if (cat != null)
                        {
                            // Delete the organization
                            svcPlsContext.Categories.DeleteOnSubmit(cat);

                            // Save to database
                            svcPlsContext.SubmitChanges();

                            message = "Selected Category Deleted Successfully";
                        }
                    }
                    //cat = (from c in svcPlsContext.Categories
                    //       where c.CategoryId == categoryId
                    //       select c).FirstOrDefault();

                    //if (cat != null)
                    //{
                    //    // Delete the organization
                    //    svcPlsContext.Categories.DeleteOnSubmit(cat);

                    //    // Save to database
                    //    svcPlsContext.SubmitChanges();

                    //    result = true;
                    //}
                    //else
                    //{
                    //    result = false;
                    //}
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteCategory().  Error: {0}", ex.Message);

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "DeleteCategory";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {

                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();


                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }
                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }
        #endregion Category Methods

        #region Location Methods
        /// <summary>
        /// List<Location> GetLocations()
        /// </summary>
        /// <returns></returns>
        public List<Location> GetLocations()
        {
            List<Location> locationList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var locations = svcPlsContext.Locations.OrderBy(l => l.LocationName);

                    if (locations != null)
                    {
                        locationList = locations.ToList<Location>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetLocations().  Error: {0}", ex.Message);

                locationList = null;
            }

            return locationList;
        }

        /// <summary>
        /// List<Location> GetLocationByOrganization(Guid organizationId)
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public List<Location> GetLocationByOrganization(Guid organizationId)
        {
            List<Location> locationList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var locations = svcPlsContext.Locations.Where(l => l.OrganizationId == organizationId).OrderBy(l => l.LocationName);

                    if (locations != null)
                    {
                        locationList = locations.ToList<Location>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetLocationsByOrganization(Guid organizationId).  Error: {0}", ex.Message);

                locationList = null;
            }

            return locationList;
        }

        /// <summary>
        /// Location GetLocation(Guid locationId)
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public Location GetLocation(Guid locationId)
        {
            Location location = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var locations = svcPlsContext.Locations.Where(l => l.LocationId == locationId);

                    if (locations != null)
                    {
                        location = locations.FirstOrDefault<Location>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetLocation(Guid locationId).  Error: {0}", ex.Message);

                location = null;
            }

            return location;
        }

        /// <summary>
        /// Location AddLocation(Location location)
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Location AddLocation(Location location)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.Locations.InsertOnSubmit(location);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddLocation().  Error: {0}", ex.Message);

                location = null;
            }

            return location;
        }        

        /// <summary>
        /// Location AddLocationBackGround(Location location)
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Location AddLocationBackGround(Location location)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var locAdd = (from lo in svcPlsContext.Locations
                                  join loinfo in svcPlsContext.LocationInfos on lo.LocationInfoId equals loinfo.LocationInfoId
                                  where lo.LocationId == location.LocationId
                                  select new
                                  {
                                      lo.LocationName,
                                      loinfo.Address1,
                                      loinfo.City,
                                      loinfo.State
                                  }).FirstOrDefault();

                    if (locAdd != null)
                    {
                        //Send Email to Support
                        NewLocationEmail(locAdd.LocationName, locAdd.Address1, locAdd.City, locAdd.State);
                    }

                    //Send APNS to the Devices.
                    var devicedetails = (from dd in svcPlsContext.DeviceDetails
                                         select new
                                         {
                                             dd.DeviceToken
                                         }).Distinct();

                    if (devicedetails != null)
                    {
                        foreach (var token in devicedetails)
                        {
                            string deviceId = token.DeviceToken.ToString();
                            string locMsg = "New Location '" + locAdd.LocationName + "' has been added into the system";
                            PushNotificationMethod(deviceId, locMsg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddLocationBackGround().  Error: {0}", ex.Message);

                location = null;
            }

            return location;
        }

        /// <summary>
        /// Location UpdateLocation(Location location, Guid locationId)
        /// </summary>
        /// <param name="location"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public Location UpdateLocation(Location location, Guid locationId)
        {
            Location loc = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific location from Database
                    loc = (from l in svcPlsContext.Locations
                           where l.LocationId == locationId
                           select l).FirstOrDefault();

                    if (loc != null)
                    {
                        loc.LocationInfoId = location.LocationInfoId;
                        loc.LocationName = location.LocationName;
                        //loc.CreateDate = location.CreateDate;
                        loc.EditDate = location.EditDate;

                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateLocation().  Error: {0}", ex.Message);

                loc = null;
            }

            return loc;
        }

        /// <summary>
        /// string DeleteLocation(Guid locationId)
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public string DeleteLocation(Guid locationId)
        {
            Ticket tic = null;
            Contact con = null;
            RecapSettingLocation rsl = null;
            Location loc = null;
            string message = string.Empty;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    tic = (from t in svcPlsContext.Tickets
                           where t.LocationId == locationId
                           select t).FirstOrDefault();

                    con = (from c in svcPlsContext.Contacts
                           where c.LocationId == locationId
                           select c).FirstOrDefault();

                    rsl = (from r in svcPlsContext.RecapSettingLocations
                           where r.LocationId == locationId
                           select r).FirstOrDefault();

                    if (tic != null)
                    {
                        message = "This Location is used by one of the Ticket in TicketMonitorRows.";

                    }

                    else if (con != null)
                    {
                        message = "This Location is used by one of the Contact.";
                    }
                    else if (rsl != null)
                    {
                        message = "This Location is used by one of the RecapSettingLocation.";
                    }
                    else
                    {

                        //Get the specific Location from Database
                        loc = (from l in svcPlsContext.Locations
                               where l.LocationId == locationId
                               select l).FirstOrDefault();

                        if (loc != null)
                        {
                            // Delete the Location
                            svcPlsContext.Locations.DeleteOnSubmit(loc);

                            // Save to database
                            svcPlsContext.SubmitChanges();

                            message = "Selected Location Deleted Successfully";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteLocation().  Error: {0}", ex.Message);
                //Pass error messages to ErrorMethod for save in Database
                string methodName = "DeleteLocation";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {

                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();


                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }
                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }


        public string DelLocation(Guid locationId)
        {
            Ticket tic = null;
            Contact con = null;
            RecapSettingLocation rsl = null;
            Location loc = null;
            string message = string.Empty;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    

                        //Get the specific Location from Database
                        loc = (from l in svcPlsContext.Locations
                               where l.LocationId == locationId
                               select l).FirstOrDefault();

                        if (loc != null)
                        {
                            var locinfo = (from l in svcPlsContext.Contacts
                                           where l.LocationId == locationId
                                           select l).FirstOrDefault();
                            if (locinfo!=null)
                            {
                                svcPlsContext.Contacts.DeleteOnSubmit(locinfo);
                                svcPlsContext.SubmitChanges();
                            }

                            var reloc = (from l in svcPlsContext.RecapSettingLocations
                                           where l.LocationId == locationId
                                           select l).FirstOrDefault();
                            if (reloc != null)
                            {
                                svcPlsContext.RecapSettingLocations.DeleteOnSubmit(reloc);
                                svcPlsContext.SubmitChanges();
                            }

                            var tick = (from l in svcPlsContext.Tickets
                                         where l.LocationId == locationId
                                         select l).FirstOrDefault();
                            if (tick != null)
                            {
                                svcPlsContext.Tickets.DeleteOnSubmit(tick);
                                svcPlsContext.SubmitChanges();
                            }



                            // Delete the Location
                            svcPlsContext.Locations.DeleteOnSubmit(loc);

                            // Save to database
                            svcPlsContext.SubmitChanges();

                            message = "Selected Location Deleted Successfully";
                        }
                    
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteLocation().  Error: {0}", ex.Message);
                //Pass error messages to ErrorMethod for save in Database
                string methodName = "DeleteLocation";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {

                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();


                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }
                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }
        #endregion Location Methods

        #region LocationInfo Methods
        /// <summary>
        /// List<LocationInfo> GetLocationInfo()
        /// </summary>
        /// <returns></returns>
        public List<LocationInfo> GetLocationInfoList()
        {
            List<LocationInfo> locationInfoList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var locationInfo = svcPlsContext.LocationInfos.OrderBy(l => l.LocationInfoId);

                    if (locationInfo != null)
                    {
                        locationInfoList = locationInfo.ToList<LocationInfo>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetLocationInfoList().  Error: {0}", ex.Message);

                locationInfoList = null;
            }

            return locationInfoList;
        }

        /// <summary>
        /// LocationInfo GetLocationInfo(Guid locationInfoId)
        /// </summary>
        /// <param name="locationInfoId"></param>
        /// <returns></returns>
        public LocationInfo GetLocationInfo(Guid locationInfoId)
        {
            LocationInfo locationInfo = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var locationInfoEntry = svcPlsContext.LocationInfos.Where(l => l.LocationInfoId == locationInfoId);

                    if (locationInfoEntry != null)
                    {
                        locationInfo = locationInfoEntry.FirstOrDefault<LocationInfo>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetLocationInfo().  Error: {0}", ex.Message);

                locationInfo = null;
            }

            return locationInfo;
        }

        /// <summary>
        /// LocationInfo AddLocationInfo(LocationInfo locationInfo)
        /// </summary>
        /// <param name="locationInfo"></param>
        /// <returns></returns>
        public LocationInfo AddLocationInfo(LocationInfo locationInfo)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.LocationInfos.InsertOnSubmit(locationInfo);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddLocationInfo().  Error: {0}", ex.Message);

                locationInfo = null;
            }

            return locationInfo;
        }

        /// <summary>
        /// LocationInfo UpdateLocationInfo(LocationInfo locationInfo, Guid locationInfoId)
        /// </summary>
        /// <param name="locationInfo"></param>
        /// <param name="locationInfoId"></param>
        /// <returns></returns>
        public LocationInfo UpdateLocationInfo(LocationInfo locationInfo, Guid locationInfoId)
        {
            LocationInfo locInfo = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific LocationInfo from Database
                    locInfo = (from l in svcPlsContext.LocationInfos
                               where l.LocationInfoId == locationInfoId
                               select l).FirstOrDefault();

                    if (locInfo != null)
                    {
                        locInfo.Address1 = locationInfo.Address1;
                        locInfo.Address2 = locationInfo.Address2;
                        locInfo.BusinessPhone = locationInfo.BusinessPhone;
                        locInfo.City = locationInfo.City;
                        locInfo.Country = locationInfo.Country;
                        //locInfo.CreateDate = locationInfo.CreateDate;
                        locInfo.EditDate = locationInfo.EditDate;
                        locInfo.Email1 = locationInfo.Email1;
                        locInfo.Email2 = locationInfo.Email2;
                        locInfo.Fax = locationInfo.Fax;
                        locInfo.HomePhone = locationInfo.HomePhone;
                        locInfo.MobilePhone = locationInfo.MobilePhone;
                        locInfo.PostalCode = locationInfo.PostalCode;
                        locInfo.State = locationInfo.State;
                        locInfo.Website = locationInfo.Website;

                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateLocationInfo().  Error: {0}", ex.Message);

                locInfo = null;
            }

            return locInfo;
        }

        /// <summary>
        /// string DeleteLocationInfo(Guid locationInfoId)
        /// </summary>
        /// <param name="locationInfoId"></param>
        /// <returns></returns>
        public string DeleteLocationInfo(Guid locationInfoId)
        {
            Location loc = null;
            LocationInfo locInfo = null;
            string message = string.Empty;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    loc = (from l in svcPlsContext.Locations
                           where l.LocationInfoId == locationInfoId
                           select l).FirstOrDefault();
                    if (loc != null)
                    {
                        message = "This LocationInfo is used by one of the Location.";
                    }
                    else
                    {
                        //Get the specific LocationInfo object from Database
                        locInfo = (from l in svcPlsContext.LocationInfos
                                   where l.LocationInfoId == locationInfoId
                                   select l).FirstOrDefault();

                        if (locInfo != null)
                        {
                            // Delete the LocationInfo object
                            svcPlsContext.LocationInfos.DeleteOnSubmit(locInfo);

                            // Save to database
                            svcPlsContext.SubmitChanges();

                            message = "Selected LocationInfo Deleted Successfully";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteLocationInfo().  Error: {0}", ex.Message);
                //Pass error messages to ErrorMethod for save in Database
                string methodName = "DeleteLocationInfo";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {

                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();


                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }
                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }
        #endregion LocationInfo Methods

        #region Contact Methods
        /// <summary>
        /// List<Contact> GetContacts()
        /// </summary>
        /// <returns></returns>
        public List<Contact> GetContacts()
        {
            List<Contact> contactList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var contacts = svcPlsContext.Contacts.OrderBy(c => c.ContactName);

                    if (contacts != null)
                    {
                        contactList = contacts.ToList<Contact>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetContacts().  Error: {0}", ex.Message);

                contactList = null;
            }

            return contactList;
        }

        /// <summary>
        /// List<Contact> GetContactsByLocation(Guid locationId)
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public List<Contact> GetContactsByLocation(Guid locationId)
        {
            List<Contact> contactList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var contacts = svcPlsContext.Contacts.Where(c => c.LocationId == locationId).OrderBy(c => c.ContactName);

                    if (contacts != null)
                    {
                        contactList = contacts.ToList<Contact>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetContactsByLocation(Guid locationId).  Error: {0}", ex.Message);

                contactList = null;
            }

            return contactList;
        }

        /// <summary>
        /// Contact GetContact(Guid contactId)
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public Contact GetContact(Guid contactId)
        {
            Contact contact = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var contacts = svcPlsContext.Contacts.Where(c => c.ContactId == contactId);

                    if (contacts != null)
                    {
                        contact = contacts.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetContact(Guid contactId).  Error: {0}", ex.Message);

                contact = null;
            }

            return contact;
        }

        /// <summary>
        /// string AddContact(Contact contact)
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public string AddContact(Contact contact)
        {
            string message = string.Empty;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var con = (from c in svcPlsContext.Contacts
                               where c.LocationId == contact.LocationId && c.FirstName == contact.FirstName &&
                               c.MiddleName == contact.MiddleName && c.LastName == contact.LastName
                               select c).FirstOrDefault();

                    if (con != null)
                    {
                        var loc = (from l in svcPlsContext.Locations
                                   where l.LocationId == contact.LocationId
                                   select l).FirstOrDefault();

                        message = "The contact already exist in the Location: " + loc.LocationName;

                    }
                    else
                    {

                        svcPlsContext.Contacts.InsertOnSubmit(contact);
                        message = "Success";
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddContact().  Error: {0}", ex.Message);

                contact = null;
            }

            return message;
        }

        /// <summary>
        /// Contact UpdateContact(Contact contact, Guid contactId)
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public Contact UpdateContact(Contact contact, Guid contactId)
        {
            Contact con = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific Contact from Database
                    con = (from c in svcPlsContext.Contacts
                           where c.ContactId == contactId
                           select c).FirstOrDefault();

                    if (con != null)
                    {
                        con.ContactName = contact.ContactName;
                        con.FirstName = contact.FirstName;
                        con.MiddleName = contact.MiddleName;
                        con.LastName = contact.LastName;
                        con.LocationId = contact.LocationId;
                        con.CallbackNumber = contact.CallbackNumber;
                        con.Email = contact.Email;
                        con.OrganizationId = contact.OrganizationId;
                        con.Phone = contact.Phone;
                        //con.CreateDate = contact.CreateDate;
                        con.EditDate = contact.EditDate;
                        con.JobTitle = contact.JobTitle;
                        con.Cell = contact.Cell;
                        con.Notes = contact.Notes;
                        con.CallbackExtension = contact.CallbackExtension;
                        con.PhoeExtension = contact.PhoeExtension;
                        con.CellExtension = contact.CellExtension;

                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateContact().  Error: {0}", ex.Message);

                con = null;
            }

            return con;
        }

        /// <summary>
        /// string DeleteContact(Guid contactId)
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public string DeleteContact(Guid contactId)
        {
            Ticket tic = null;
            Contact contact = null;
            string message = string.Empty;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    tic = (from t in svcPlsContext.Tickets
                           where t.ContactId == contactId
                           select t).FirstOrDefault();
                    if (tic != null)
                    {
                        message = "This Contact is used by one of the Ticket in TicketMonitorRows.";

                    }
                    else
                    {
                        //Get the specific Contact object from Database
                        contact = (from c in svcPlsContext.Contacts
                                   where c.ContactId == contactId
                                   select c).FirstOrDefault();

                        if (contact != null)
                        {
                            // Delete the Contact object
                            svcPlsContext.Contacts.DeleteOnSubmit(contact);

                            // Save to database
                            svcPlsContext.SubmitChanges();

                            message = "Selected Contact Deleted Successfully";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteContact().  Error: {0}", ex.Message);

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "DeleteContact";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();
                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }

                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }
        #endregion Contact Methods

        #region Ticket Methods
        /// <summary>
        /// List<Ticket> GetTickets()
        /// </summary>
        /// <returns></returns>
        public List<Ticket> GetTickets()
        {
            List<Ticket> ticketList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var tickets = svcPlsContext.Tickets.OrderBy(t => t.TicketName);

                    if (tickets != null)
                    {
                        ticketList = tickets.ToList<Ticket>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTickets().  Error: {0}", ex.Message);

                ticketList = null;
            }

            return ticketList;
        }

        /// <summary>
        /// List<Ticket> GetTicketsByHourElapsed(string sortOrder = "Oldest");
        /// </summary>
        /// <param name="sortOrder">Oldest or Newest</param>
        /// <returns></returns>
        public List<Ticket> GetTicketsByHourElapsed(string sortOrder = "Oldest")
        {
            List<Ticket> ticketList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    if (sortOrder.ToLower() == "oldest" ||
                        (sortOrder.ToLower().Equals("oldest") == false &&
                         sortOrder.ToLower().Equals("newest") == false))
                    {
                        var tickets = svcPlsContext.Tickets.OrderBy(t => t.CreateDate);

                        if (tickets != null)
                        {
                            ticketList = tickets.ToList<Ticket>();
                        }
                    }
                    else if (sortOrder.ToLower() == "newest")
                    {
                        var tickets = svcPlsContext.Tickets.OrderByDescending(t => t.CreateDate);

                        if (tickets != null)
                        {
                            ticketList = tickets.ToList<Ticket>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTickets().  Error: {0}", ex.Message);

                ticketList = null;
            }

            return ticketList;
        }

        /// <summary>
        /// List<Ticket> GetTicketsByCategory(Guid categoryId)
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public List<Ticket> GetTicketsByCategory(Guid categoryId)
        {
            List<Ticket> ticketList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var tickets = svcPlsContext.Tickets.Where(t => t.CategoryId == categoryId).OrderBy(t => t.TicketName);

                    if (tickets != null)
                    {
                        ticketList = tickets.ToList<Ticket>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketsByCategory().  Error: {0}", ex.Message);

                ticketList = null;
            }

            return ticketList;
        }

        /// <summary>
        /// List<Ticket> GetTicketsByCategoryAndLocation(Guid categoryId, Guid locationId)
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public List<Ticket> GetTicketsByCategoryAndLocation(Guid categoryId, Guid locationId)
        {
            List<Ticket> ticketList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var tickets = svcPlsContext.Tickets.Where(t => t.CategoryId == categoryId &&
                                                                   t.LocationId == locationId).OrderBy(t => t.TicketName);

                    if (tickets != null)
                    {
                        ticketList = tickets.ToList<Ticket>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketsByCategoryAndLocation(Guid categoryId, Guid locationId).  Error: {0}", ex.Message);

                ticketList = null;
            }

            return ticketList;
        }

        /// <summary>
        /// List<Ticket> GetTicketsByLocation(Guid locationId, Guid? contactId = null)
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public List<Ticket> GetTicketsByLocation(Guid locationId, Guid contactId)
        {
            List<Ticket> ticketList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    if (contactId != Guid.Empty)
                    {
                        var query = from ticket in svcPlsContext.Tickets
                                    from contact in svcPlsContext.Contacts
                                    from location in svcPlsContext.Locations
                                    where ticket.LocationId == locationId &&
                                          location.LocationId == locationId &&
                                          contact.LocationId == locationId &&
                                          contact.ContactId == contactId
                                    select ticket;

                        if (query != null)
                        {
                            ticketList = query.OrderBy(t => t.TicketName).ToList<Ticket>();
                        }
                    }
                    else
                    {
                        ticketList = svcPlsContext.Tickets.Where(t => t.LocationId == locationId).OrderBy(t => t.TicketName).ToList<Ticket>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketsByLocation(Guid locationId, Guid? contactId = null).  Error: {0}", ex.Message);

                ticketList = null;
            }

            return ticketList;
        }

        /// <summary>
        /// List<Ticket> GetTicketsByLocations(List<Guid> locationIds)
        /// </summary>
        /// <param name="locationIds"></param>
        /// <returns></returns>
        public List<Ticket> GetTicketsByLocations(List<Guid> locationIds)
        {
            List<Ticket> ticketList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    foreach (Guid locationId in locationIds)
                    {
                        List<Ticket> ticketLocationList = null;

                        ticketLocationList = svcPlsContext.Tickets.Where(t => t.LocationId == locationId).OrderBy(t => t.TicketName).ToList<Ticket>();

                        if (ticketLocationList != null)
                        {
                            if (ticketList == null)
                            {
                                ticketList = new List<Ticket>();
                            }

                            ticketList.AddRange(ticketLocationList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketsByLocations(List<Guid> locationIds).  Error: {0}", ex.Message);

                ticketList = null;
            }

            return ticketList.OrderBy(t => t.LocationId).ThenBy(t => t.TicketId).ToList<Ticket>();
        }

        /// <summary>
        /// List<Ticket> GetTicketsByStatus(string status)
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<Ticket> GetTicketsByStatus(string status)
        {
            List<Ticket> ticketList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    if (status.ToLower() != "all")
                    {
                        ticketList = svcPlsContext.Tickets.Where(t => t.Status.ToLower() == status.ToLower()).ToList<Ticket>();
                    }
                    else if (status.ToLower().Equals("all"))
                    {
                        ticketList = svcPlsContext.Tickets.ToList<Ticket>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketsByStatus(string status).  Error: {0}", ex.Message);

                ticketList = null;
            }

            return ticketList.OrderBy(t => t.LocationId).ThenBy(t => t.TicketId).ToList<Ticket>();
        }

        /// <summary>
        /// Ticket GetTicket(string ticketId)
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        public Ticket GetTicket(Guid ticketId)
        {
            Ticket ticket = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var tickets = svcPlsContext.Tickets.Where(t => t.TicketId == ticketId);

                    if (tickets != null)
                    {
                        ticket = tickets.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicket(Guid ticketId).  Error: {0}", ex.Message);

                ticket = null;
            }

            return ticket;
        }

        /// <summary>
        /// Ticket AddTicket(Ticket ticket)
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        //public Ticket AddTicket(Ticket ticket)
        //{
        //    string ph = string.Empty;
        //    string usrName = string.Empty;
        //    string message = string.Empty;
        //    SnoozedTicket snoozeTick = new SnoozedTicket();
        //    Guid userId = Guid.Empty;
        //    Guid ticketId = Guid.Empty;
        //    Guid snozId = Guid.Empty;
        //    bool isCreation = false;
        //    try
        //    {
        //        using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
        //        {
        //            svcPlsContext.Tickets.InsertOnSubmit(ticket);
        //            svcPlsContext.SubmitChanges();

        //            //Send Email to the Tech,When New ticket is Created
        //            var contact = (from c in svcPlsContext.Contacts
        //                           where c.ContactId == ticket.ContactId
        //                           select c).FirstOrDefault();

        //            if (contact != null)
        //            {
        //                NewTicketEmail(contact.Email, ticket.TicketNum, contact.FirstName);
        //            }


        //            //Check wheather the User in Snooze or not
        //            var snooze = (from s in svcPlsContext.Snoozes
        //                          where s.UserId == ticket.UserId && DateTime.Now >= Convert.ToDateTime(s.StartDate) && DateTime.Now <= Convert.ToDateTime(s.EndDate)
        //                          select s).FirstOrDefault();

        //            if (snooze != null)
        //            {
        //                snoozeTick.SnoozedTicketId = Guid.NewGuid();
        //                snoozeTick.SnoozeId = snooze.SnoozeId;
        //                snoozeTick.TicketId = ticket.TicketId;
        //                snoozeTick.CreateDate = ticket.CreateDate;
        //                snoozeTick.EditDate = ticket.EditDate;
        //                svcPlsContext.SnoozedTickets.InsertOnSubmit(snoozeTick);
        //                svcPlsContext.SubmitChanges();

        //                //Send Email to the User
        //                userId = (Guid)ticket.UserId;
        //                ticketId = (Guid)ticket.TicketId;
        //                snozId = (Guid)snooze.SnoozeId;
        //                EmailBody(userId, ticketId, snozId, isCreation);

        //                //Get the UserDetails from user table
        //                var usr = (from u in svcPlsContext.Users
        //                           where u.UserId == userId
        //                           select u).FirstOrDefault();
        //                if (usr != null)
        //                {
        //                    ph = usr.Phone;
        //                    usrName = usr.FirstName;
        //                    message = "Hey " + usrName + ", Please accept this ticket once you're back.";
        //                    SendSMS(ph, message);
        //                }
        //            }
        //            else
        //            {
        //                Guid usrId = new Guid("6FB353AC-A2C3-4438-B930-5AC3E2925053");
        //                if ((Guid)ticket.UserId != usrId)
        //                {

        //                    AssignEmailBody((Guid)ticket.UserId, (Guid)ticket.TicketId);

        //                    //Get the UserDetails from user table
        //                    var usr = (from u in svcPlsContext.Users
        //                               where u.UserId == (Guid)ticket.UserId
        //                               select u).FirstOrDefault();
        //                    if (usr != null)
        //                    {
        //                        ph = usr.Phone;
        //                        usrName = usr.FirstName;
        //                        message = "Hey " + usrName + ", The following ticket is assigned to you, please check it out.";
        //                        SendSMS(ph, message);
        //                    }                            
        //                }
        //            }

        //            //Send APNS
        //            var apnusr = (from d in svcPlsContext.DeviceDetails
        //                          where d.UserId == (Guid)ticket.UserId
        //                          select new
        //                          {
        //                              d.DeviceToken
        //                          }).Distinct();

        //            if (apnusr != null)
        //            {
        //                foreach (var apn in apnusr)
        //                {
        //                    string deviceId = apn.DeviceToken.ToString();
        //                    string mess = "The ticket '" + ticket.TicketNumber + "' is assigned to you, Please check it out.";
        //                    PushNotificationMethod(deviceId, mess);
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Write("Error in AddTicket().  Error: {0}", ex.Message);

        //        ticket = null;
        //    }

        //    return ticket;
        //}

        public Ticket AddTicket(Ticket ticket)
        {
            Problem pblm = new Problem();
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.Tickets.InsertOnSubmit(ticket);
                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddTicket().  Error: {0}", ex.Message);

                ticket = null;
            }

            return ticket;
        }

        /// <summary>
        /// Ticket AddTicketBackGround(Ticket ticket)
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public Ticket AddTicketBackGround(Ticket ticket)
        {
            string message = string.Empty;
            string contactEmail = string.Empty;
            SnoozedTicket snoozeTick = new SnoozedTicket();
            Guid ticketId = (Guid)ticket.TicketId;
            Guid snozId = Guid.Empty;
            bool isCreation = false;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var addTick = (from t in svcPlsContext.Tickets
                                   join u in svcPlsContext.Users on t.UserId equals u.UserId
                                   join c in svcPlsContext.Contacts on t.ContactId equals c.ContactId
                                   join l in svcPlsContext.Locations on t.LocationId equals l.LocationId
                                   where t.TicketId == ticket.TicketId && l.LocationId == ticket.LocationId
                                   select new
                                   {
                                       u.UserId,
                                       u.Email,
                                       u.UserName,
                                       u.Phone,
                                       t.TicketNum,
                                       t.TicketName,
                                       t.Status,
                                       t.CreateDate,
                                       t.CloseDate,
                                       t.ResponseTimeStamp,
                                       t.IsHelpDocShare,
                                       l.LocationName,
                                       c.FirstName,
                                       c.CallbackNumber,
                                       contactEmail = c.Email
                                   }).FirstOrDefault();

                    if (addTick != null)
                    {
                        if (!string.IsNullOrEmpty(contactEmail))
                        {
                            NewTicketEmail(contactEmail, ticket.TicketNum, addTick.FirstName);
                        }

                        Guid usrId = new Guid("6FB353AC-A2C3-4438-B930-5AC3E2925053");// check no-tech
                        if (addTick.UserId != usrId)
                        {
                            //Check wheather the User in Snooze or not
                            var snooze = (from s in svcPlsContext.Snoozes
                                          where s.UserId == addTick.UserId && DateTime.Now >= Convert.ToDateTime(s.StartDate) && DateTime.Now <= Convert.ToDateTime(s.EndDate)
                                          select s).FirstOrDefault();

                            if (snooze != null)
                            {
                                snoozeTick.SnoozedTicketId = Guid.NewGuid();
                                snoozeTick.SnoozeId = snooze.SnoozeId;
                                snoozeTick.TicketId = ticket.TicketId;
                                snoozeTick.CreateDate = ticket.CreateDate;
                                snoozeTick.EditDate = ticket.EditDate;
                                svcPlsContext.SnoozedTickets.InsertOnSubmit(snoozeTick);
                                svcPlsContext.SubmitChanges();

                                //Send Email to the User when snooze status
                                snozId = (Guid)snooze.SnoozeId;
                                EmailBody(ticketId, snozId, isCreation);

                                //Send SMS to the user when snooze status
                                message = "Hey " + addTick.UserName + ", Please accept this ticket once you're back.";
                                SendSMS(addTick.Phone, message);
                            }
                            else
                            {
                                //Send Email to the User when not in snooze status
                                AssignEmailBody(ticketId);

                                //Send SMS to the user when not in snooze status
                                message = "Hey " + addTick.UserName + ", The following ticket is assigned to you, Please check it out.";
                                SendSMS(addTick.Phone, message);
                            }

                            //send apns to the corresponding tech
                            var apnusr = (from d in svcPlsContext.DeviceDetails
                                          where d.UserId == addTick.UserId
                                          select d).FirstOrDefault();

                            if (apnusr != null)
                            {
                                string addMsg = "The ticket '" + ticket.TicketNum + "','" + addTick.LocationName + "','" + addTick.TicketName + "','" + addTick.FirstName + "','" + addTick.CallbackNumber + "' is assigned to you, Please check it out.";
                                PushNotificationMethod(apnusr.DeviceToken, addMsg);
                            }


                            //send apns to emy,sarah,hector,heidi
                            List<Guid> useridss = new List<Guid>();
                            char[] separator = new char[] { ',' };
                            string connectionInfo = ConfigurationManager.AppSettings["UserID"];
                            string[] strSplitArr = connectionInfo.Split(separator);

                            foreach (var arrStr in strSplitArr)
                            {
                                useridss.Add(Guid.Parse(arrStr));
                            }

                            foreach (var uids in useridss)
                            {
                                //Send APNS to the tech,When New ticket is Created
                                var apnusrs = (from d in svcPlsContext.DeviceDetails
                                               where d.UserId == (Guid)uids
                                               select new
                                               {
                                                   d.DeviceToken
                                               }).Distinct();

                                if (apnusrs != null && apnusrs.Any())
                                {
                                    foreach (var api in apnusrs)
                                    {
                                        string deviceId = api.DeviceToken.ToString();
                                        string addMsg = "The ticket '" + ticket.TicketNum + "','" + addTick.LocationName + "','" + addTick.TicketName + "','" + addTick.FirstName + "','" + addTick.CallbackNumber + "' is assigned to '" + addTick.UserName + "' Please check it out.";
                                        PushNotificationMethod(deviceId, addMsg);
                                    }
                                }
                            }

                        }
                        //else 
                        //{ 
                        //    //Send APNS to all users if the userid in No-Tech.
                        //    var devicedetails = (from dd in svcPlsContext.DeviceDetails
                        //                         select new
                        //                         {
                        //                             dd.DeviceToken
                        //                         }).Distinct();

                        //    if (devicedetails != null)
                        //    {
                        //        foreach (var token in devicedetails)
                        //        {
                        //            string deviceId = token.DeviceToken.ToString();
                        //            string apnsMsg = "New Ticket'" + ticket.TicketNum + "','" + addTick.LocationName + "','" + addTick.TicketName + "','" + addTick.FirstName + "','" + addTick.CallbackNumber + "' is created.";
                        //            PushNotificationMethod(deviceId, apnsMsg);
                        //        }
                        //    }
                        //}

                        //check the ticket is shared as helpdoc
                        if (ticket.IsHelpDocShare == true)
                        {
                            //Send apns to all users.                        
                            var devicedetails = (from dd in svcPlsContext.DeviceDetails
                                                 select new
                                                 {
                                                     dd.DeviceToken
                                                 }).Distinct();

                            if (devicedetails != null)
                            {
                                foreach (var token in devicedetails)
                                {
                                    string deviceId = token.DeviceToken.ToString();
                                    string msgHelpDoc = "New Ticket'" + ticket.TicketNum + "','" + addTick.LocationName + "','" + addTick.TicketName + "','" + addTick.FirstName + "','" + addTick.CallbackNumber + "' is saved as HelpDoc for future references.";
                                    PushNotificationMethod(deviceId, msgHelpDoc);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddTicketBackGround().  Error: {0}", ex.Message);

                ticket = null;
            }

            return ticket;
        }

        /// <summary>
        /// Ticket UpdateTicket(Ticket ticket, Guid ticketId)
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        public Ticket UpdateTicket(Guid ticketId, Ticket ticket)
        {
            Ticket tick = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific Ticket from Database
                    tick = (from t in svcPlsContext.Tickets
                            where t.TicketId == ticketId
                            select t).FirstOrDefault();

                    if (tick != null)
                    {
                        tick.TicketName = ticket.TicketName;
                        tick.CategoryId = ticket.CategoryId;
                        tick.IsHelpDoc = ticket.IsHelpDoc;

                        if (ticket.Status == "C" && tick.Status != "C")
                        {
                            tick.CloseDate = ticket.CloseDate;
                        }

                        else if (ticket.Status != "C" && tick.Status == "C")
                        {
                            tick.CloseDate = ticket.CloseDate;
                        }
                        tick.Status = ticket.Status;
                        tick.UserId = ticket.UserId;
                        tick.LocationId = ticket.LocationId;
                        tick.EditDate = ticket.EditDate;
                        tick.ContactId = ticket.ContactId;
                        tick.IsHelpDocShare = ticket.IsHelpDocShare;

                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateTicket().  Error: {0}", ex.Message);

                tick = null;
            }

            return tick;
        }

        /// <summary>
        /// Ticket UpdateTicketBackGround(Guid ticketId,Ticket ticket)
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public Ticket UpdateTicketBackGround(Guid ticketId, Ticket ticket)
        {
            Ticket tick = null;
            string apnsMsg = string.Empty;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var upTick = (from t in svcPlsContext.Tickets
                                  join u in svcPlsContext.Users on t.UserId equals u.UserId
                                  join c in svcPlsContext.Contacts on t.ContactId equals c.ContactId
                                  join l in svcPlsContext.Locations on t.LocationId equals l.LocationId
                                  where t.TicketId == ticketId
                                  select new
                                  {
                                      u.UserId,
                                      u.Email,
                                      u.UserName,
                                      t.TicketNum,
                                      t.TicketName,
                                      t.Status,
                                      t.CreateDate,
                                      t.CloseDate,
                                      t.ResponseTimeStamp,
                                      t.IsHelpDocShare,
                                      l.LocationName,
                                      c.FirstName,
                                      c.CallbackNumber
                                  }).FirstOrDefault();

                    if (upTick != null)
                    {

                        if (upTick.Status == "C" && Convert.ToDateTime(upTick.CloseDate).Date == DateTime.Now.Date)
                        {
                            //send apns & email to the corresponding tech, when ticket is closed.

                            //Send Email
                            ClosedTicketEmail(ticketId);

                            //send Apns
                            apnsMsg = "The following ticket '" + upTick.TicketNum + "','" + upTick.LocationName + "','" + upTick.TicketName + "','" + upTick.FirstName + "','" + upTick.CallbackNumber + "' is closed.";
                            UpdateTicketApns(upTick.UserId, apnsMsg);

                        }
                        else
                        {
                            //send Apns
                            apnsMsg = "The following ticket '" + upTick.TicketNum + "' is updated.";
                            UpdateTicketApns(upTick.UserId, apnsMsg);

                        }

                        if (ticket.IsHelpDocShare == true && upTick.IsHelpDocShare == true)
                        {
                            //Send apns to all users.
                            var devicedetails = (from dd in svcPlsContext.DeviceDetails
                                                 select new
                                                 {
                                                     dd.DeviceToken
                                                 }).Distinct();

                            if (devicedetails != null)
                            {
                                foreach (var token in devicedetails)
                                {
                                    string deviceId = token.DeviceToken.ToString();
                                    string msgHelpDoc = "The following ticket '" + upTick.TicketNum + "','" + upTick.LocationName + "','" + upTick.TicketName + "','" + upTick.FirstName + "','" + upTick.CallbackNumber + "' is updated as HelpDoc for future references.";
                                    PushNotificationMethod(deviceId, msgHelpDoc);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateTicketBackGround().  Error: {0}", ex.Message);

                tick = null;
            }
            return ticket;
        }

        public void UpdateTicketApns(Guid userId, string apnsMsg)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Send APNS
                    List<Guid> useridss = new List<Guid>();
                    useridss.Add(userId);
                    char[] separator = new char[] { ',' };
                    string connectionInfo = ConfigurationManager.AppSettings["UserID"];
                    string[] strSplitArr = connectionInfo.Split(separator);

                    foreach (var arrStr in strSplitArr)
                    {
                        useridss.Add(Guid.Parse(arrStr));
                    }

                    foreach (var uids in useridss)
                    {
                        //Send APNS to the tech,When the ticket is updated
                        var apnusr = (from d in svcPlsContext.DeviceDetails
                                      where d.UserId == (Guid)uids
                                      select new
                                      {
                                          d.DeviceToken
                                      }).Distinct();

                        if (apnusr != null && apnusr.Any())
                        {
                            foreach (var api in apnusr)
                            {
                                string deviceId = api.DeviceToken.ToString();
                                PushNotificationMethod(deviceId, apnsMsg);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateTicketApns().  Error: {0}", ex.Message);
            }

        }

        /// <summary>
        /// Ticket AssignTicket(Ticket ticket)
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public Ticket AssignTicket(Ticket ticket)
        {
            SnoozedTicket snoozeTick = new SnoozedTicket();
            Guid ticketId = ticket.TicketId;
            Guid userId = (Guid)ticket.UserId;
            Guid snozId = Guid.Empty;
            Ticket tick = null;
            bool isCreation = true;
            string message = string.Empty;
            string usrph = string.Empty;
            string usrEmail = string.Empty;
            string usrName = string.Empty;

            if (ticketId == Guid.Empty)
            {
                Logger.Write("Missing TicketId.");

                return null;
            }

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {

                    //Get the specific Ticket from Database
                    tick = (from t in svcPlsContext.Tickets
                            where t.TicketId == ticketId
                            select t).FirstOrDefault();

                    if (tick != null)
                    {
                        //Get UserDetails from DB to the corresponding ticket
                        var getUser = (from u in svcPlsContext.Users
                                       where u.UserId == userId
                                       select u).FirstOrDefault();

                        if (getUser != null)
                        {
                            usrName = getUser.FirstName;
                            usrph = getUser.Phone;
                            usrEmail = getUser.Email;
                        }

                        tick.UserId = ticket.UserId;
                        tick.ResponseTimeStamp = ticket.ResponseTimeStamp;
                        tick.EditDate = ticket.EditDate;
                        tick.Comments = ticket.Comments;

                        //Save to database
                        svcPlsContext.SubmitChanges();

                        //check the ticketid is already in snoozedticket
                        var delsnoozetik = (from st in svcPlsContext.SnoozedTickets
                                            where st.TicketId == ticketId
                                            select st).FirstOrDefault();

                        if (delsnoozetik != null)
                        {
                            svcPlsContext.SnoozedTickets.DeleteOnSubmit(delsnoozetik);
                            svcPlsContext.SubmitChanges();
                        }

                        //Check the Snooze Table whether snooze created or not to the Tech
                        var snooze = (from s in svcPlsContext.Snoozes
                                      where s.UserId == userId && DateTime.Now >= Convert.ToDateTime(s.StartDate) && DateTime.Now <= Convert.ToDateTime(s.EndDate)
                                      select s).FirstOrDefault();

                        if (snooze != null)
                        {
                            snoozeTick.SnoozedTicketId = Guid.NewGuid();
                            snoozeTick.SnoozeId = snooze.SnoozeId;
                            snoozeTick.TicketId = ticketId;
                            snoozeTick.CreateDate = ticket.CreateDate;
                            snoozeTick.EditDate = ticket.EditDate;
                            svcPlsContext.SnoozedTickets.InsertOnSubmit(snoozeTick);
                            svcPlsContext.SubmitChanges();

                            snozId = (Guid)snooze.SnoozeId;
                            //Send Email when user in Snooze
                            EmailBody(ticketId, snozId, isCreation);
                        }
                        else
                        {
                            Guid usrId = new Guid("6FB353AC-A2C3-4438-B930-5AC3E2925053");
                            if (userId != usrId)
                            {
                                //Send Email when user not in Snooze
                                AssignEmailBody(ticketId);

                                //Get the UserDetails from user table
                                message = "Hey " + usrName + ", The following ticket is assigned to you, Please check it out.";
                                SendSMS(usrph, message);
                            }
                        }
                    }
                    else
                    {
                        Logger.Write("Ticket not found.");

                        tick = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AssignTicket().  Error: {0}", ex.Message);

                tick = null;
            }

            return tick;
        }

        /// <summary>
        /// Ticket AssignTicketBackGround(Ticket ticket)
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public Ticket AssignTicketBackGround(Ticket ticket)
        {
            Guid ticketId = ticket.TicketId;
            Guid userId = (Guid)ticket.UserId;
            string uname = string.Empty;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //get username from user table
                    var usr = (from d in svcPlsContext.Users
                               where d.UserId == userId
                               select d).FirstOrDefault();
                    if (usr != null)
                    {
                        uname = usr.UserName;
                    }

                    //send apns to the corresponding tech
                    var apnusr = (from d in svcPlsContext.DeviceDetails
                                  where d.UserId == userId
                                  select d).FirstOrDefault();

                    if (apnusr != null)
                    {
                        string mess = "The ticket '" + ticket.TicketNum + "' is assigned to you, Please check it out.";
                        PushNotificationMethod(apnusr.DeviceToken, mess);
                    }

                    //Send apns to emy,sarah,hector,heidi
                    List<Guid> useridss = new List<Guid>();
                    char[] separator = new char[] { ',' };
                    string connectionInfo = ConfigurationManager.AppSettings["UserID"];
                    string[] strSplitArr = connectionInfo.Split(separator);

                    foreach (var arrStr in strSplitArr)
                    {
                        useridss.Add(Guid.Parse(arrStr));
                    }

                    foreach (var uids in useridss)
                    {
                        //Send APNS to the tech,When the ticket is assigned
                        var apnusrs = (from d in svcPlsContext.DeviceDetails
                                       where d.UserId == (Guid)uids
                                       select new
                                       {
                                           d.DeviceToken
                                       }).Distinct();

                        if (apnusrs != null && apnusrs.Any())
                        {
                            foreach (var api in apnusrs)
                            {
                                string deviceId = api.DeviceToken.ToString();
                                string mess = "The ticket '" + ticket.TicketNum + "' is assigned to '" + uname + "', Please check it out.";
                                PushNotificationMethod(deviceId, mess);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AssignTicketBackGround().  Error: {0}", ex.Message);

                ticket = null;
            }

            return ticket;
        }

        /// <summary>
        /// string DeleteTicket(Guid ticketId)
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        public string DeleteTicket(Guid ticketId)
        {
            string message = string.Empty;
            Ticket ticket = null;
            List<ProblemBlob> problemBlobs = null;
            List<Problem> problems = null;
            List<SolutionBlob> solutionBlobs = null;
            List<Solution> solutions = null;
            List<LikeUnlike> likeUnlike = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    // Delete ProblemBlobs that belong to Problems that 
                    // belong to this ticket.
                    problemBlobs = (from pb in svcPlsContext.ProblemBlobs
                                    from p in svcPlsContext.Problems
                                    where pb.ProblemId == p.ProblemId &&
                                          p.TicketId == ticketId
                                    select pb).ToList<ProblemBlob>();

                    if (problemBlobs != null)
                    {
                        // Delete the ProblemBlob objects
                        svcPlsContext.ProblemBlobs.DeleteAllOnSubmit(problemBlobs);

                        // Save to database
                        svcPlsContext.SubmitChanges();
                    }

                    // Delete Problems that belong to this ticket.
                    problems = (from p in svcPlsContext.Problems
                                where p.TicketId == ticketId
                                select p).ToList<Problem>();

                    if (problems != null)
                    {
                        // Delete the Problem objects
                        svcPlsContext.Problems.DeleteAllOnSubmit(problems);

                        // Save to database
                        svcPlsContext.SubmitChanges();
                    }

                    // Delete SolutionBlobs that belong to Solutions that 
                    // belong to this ticket.
                    solutionBlobs = (from sb in svcPlsContext.SolutionBlobs
                                     from s in svcPlsContext.Solutions
                                     where sb.SolutionId == s.SolutionId &&
                                           s.TicketId == ticketId
                                     select sb).ToList<SolutionBlob>();

                    if (solutionBlobs != null)
                    {
                        // Delete the SolutionBlob objects
                        svcPlsContext.SolutionBlobs.DeleteAllOnSubmit(solutionBlobs);

                        // Save to database
                        svcPlsContext.SubmitChanges();
                    }

                    // Delete LikeUnlikes that belong to this ticket & Solution.
                    likeUnlike = (from l in svcPlsContext.LikeUnlikes
                                  from s in svcPlsContext.Solutions
                                  where l.SolutionId == s.SolutionId &&
                                        s.TicketId == ticketId
                                  select l).ToList<LikeUnlike>();

                    if (likeUnlike != null)
                    {
                        // Delete the LikeUnlike objects
                        svcPlsContext.LikeUnlikes.DeleteAllOnSubmit(likeUnlike);

                        // Save to database
                        svcPlsContext.SubmitChanges();
                    }

                    // Delete Solutions that belong to this ticket.
                    solutions = (from s in svcPlsContext.Solutions
                                 where s.TicketId == ticketId
                                 select s).ToList<Solution>();

                    if (solutions != null)
                    {
                        // Delete the Solution objects
                        svcPlsContext.Solutions.DeleteAllOnSubmit(solutions);

                        // Save to database
                        svcPlsContext.SubmitChanges();
                    }

                    //check the ticketId from snoozedticket table
                    var delsnoozeticket = (from st in svcPlsContext.SnoozedTickets
                                           where st.TicketId == ticketId
                                           select st).FirstOrDefault();

                    if (delsnoozeticket != null)
                    {
                        svcPlsContext.SnoozedTickets.DeleteOnSubmit(delsnoozeticket);
                        svcPlsContext.SubmitChanges();
                    }

                    //check the ticketId from ticketcomment table
                    var delticketcomment = (from tc in svcPlsContext.TicketComments
                                            where tc.TicketId == ticketId
                                            select tc).ToList<TicketComment>();

                    if (delticketcomment != null)
                    {
                        svcPlsContext.TicketComments.DeleteAllOnSubmit(delticketcomment);
                        svcPlsContext.SubmitChanges();
                    }

                    // Get the specific Ticket object from Database
                    ticket = (from t in svcPlsContext.Tickets
                              where t.TicketId == ticketId
                              select t).FirstOrDefault();

                    if (ticket != null)
                    {
                        // Delete the Ticket object
                        svcPlsContext.Tickets.DeleteOnSubmit(ticket);

                        // Save to database
                        svcPlsContext.SubmitChanges();

                        message = "Selected Ticket Deleted Successfully";
                    }
                    else
                    {
                        message = "Selected Ticket Not Deleted Successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteTicket().  Error: {0}", ex.Message);

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "DeleteTicket";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();
                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }

                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);

            }

            return message;
        }

        #endregion Ticket Methods

        #region TicketMonitor Methods
        public List<TicketMonitorRow> GetTicketMonitorRows()
        {
            List<TicketMonitorRow> ticketMonitorList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {

                    var query = from ticket in svcPlsContext.Tickets
                                join st in svcPlsContext.SnoozedTickets
                                    on ticket.TicketId equals st.TicketId into ticket_st
                                from snoozedTicket in ticket_st.DefaultIfEmpty()
                                from user in svcPlsContext.Users
                                from category in svcPlsContext.Categories
                                from contact in svcPlsContext.Contacts
                                from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                from servicePlanType in svcPlsContext.ServicePlanTypes
                                from location in svcPlsContext.Locations
                                where ticket.LocationId == location.LocationId &&
                                        category.CategoryId == ticket.CategoryId &&
                                        ticket.UserId == user.UserId &&
                                        servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                        servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId &&
                                        ticket.ContactId == contact.ContactId
                                select new
                                {
                                    TicketNumber = ticket.TicketNum,
                                    TicketId = ticket.TicketId,
                                    CategoryId = category.CategoryId,
                                    ContactId = contact.ContactId,
                                    LocationId = location.LocationId,
                                    OrganizationId = ticket.OrganizationId,
                                    UserId = user.UserId,
                                    Location = location.LocationName ?? string.Empty,
                                    Contact = contact.ContactName ?? string.Empty,
                                    Time = ticket.CreateDate,
                                    Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                    Description = ticket.TicketName ?? string.Empty,
                                    Category = category.CategoryName ?? string.Empty,
                                    Tech = user.UserName ?? string.Empty,
                                    Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                    ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                    SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                };

                    if (query != null)
                    {
                        ticketMonitorList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRows().  Error: {0}", ex.Message);

                ticketMonitorList = null;
            }

            return ticketMonitorList;
        }

        public List<TicketMonitorRow> GetTicketMonitorRowsByElapsedTime(string sortOrder = "Oldest")
        {
            List<TicketMonitorRow> ticketMonitorList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var query = from ticket in svcPlsContext.Tickets
                                join st in svcPlsContext.SnoozedTickets
                                    on ticket.TicketId equals st.TicketId into ticket_st
                                from snoozedTicket in ticket_st.DefaultIfEmpty()
                                from user in svcPlsContext.Users
                                from category in svcPlsContext.Categories
                                from contact in svcPlsContext.Contacts
                                from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                from servicePlanType in svcPlsContext.ServicePlanTypes
                                from location in svcPlsContext.Locations
                                where ticket.LocationId == location.LocationId &&
                                        category.CategoryId == ticket.CategoryId &&
                                        ticket.UserId == user.UserId &&
                                        servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                        servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId &&
                                        ticket.ContactId == contact.ContactId
                                select new
                                {
                                    TicketNumber = ticket.TicketNum,
                                    TicketId = ticket.TicketId,
                                    CategoryId = category.CategoryId,
                                    ContactId = contact.ContactId,
                                    LocationId = location.LocationId,
                                    OrganizationId = ticket.OrganizationId,
                                    UserId = user.UserId,
                                    Location = location.LocationName ?? string.Empty,
                                    Contact = contact.ContactName ?? string.Empty,
                                    Time = ticket.CreateDate,
                                    Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                    Description = ticket.TicketName ?? string.Empty,
                                    Category = category.CategoryName ?? string.Empty,
                                    Tech = user.UserName ?? string.Empty,
                                    Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                    ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                    SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                };

                    if (query != null)
                    {
                        ticketMonitorList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;

                        if (sortOrder.ToLower() == "oldest" ||
                            (sortOrder.ToLower().Equals("oldest") == false &&
                             sortOrder.ToLower().Equals("newest") == false))
                        {
                            ticketMonitorList = ticketMonitorList.OrderBy(t => t.Elapsed).ToList<TicketMonitorRow>();
                        }
                        else if (sortOrder.ToLower() == "newest")
                        {
                            ticketMonitorList = ticketMonitorList.OrderByDescending(t => t.Elapsed).ToList<TicketMonitorRow>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRowsByElapsedTime(string sortOrder = \"Oldest\").  Error: {0}", ex.Message);

                ticketMonitorList = null;
            }

            return ticketMonitorList;
        }

        public List<TicketMonitorRow> GetTicketMonitorRowsByCategory(Guid categoryId)
        {
            List<TicketMonitorRow> ticketMonitorList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var query = from ticket in svcPlsContext.Tickets
                                join st in svcPlsContext.SnoozedTickets
                                    on ticket.TicketId equals st.TicketId into ticket_st
                                from snoozedTicket in ticket_st.DefaultIfEmpty()
                                from user in svcPlsContext.Users
                                from category in svcPlsContext.Categories
                                from contact in svcPlsContext.Contacts
                                from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                from servicePlanType in svcPlsContext.ServicePlanTypes
                                from location in svcPlsContext.Locations
                                where category.CategoryId == categoryId &&
                                        ticket.LocationId == location.LocationId &&
                                        category.CategoryId == ticket.CategoryId &&
                                        ticket.UserId == user.UserId &&
                                        servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                        servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId &&
                                        ticket.ContactId == contact.ContactId
                                select new
                                {
                                    TicketNumber = ticket.TicketNum,
                                    TicketId = ticket.TicketId,
                                    CategoryId = category.CategoryId,
                                    ContactId = contact.ContactId,
                                    LocationId = location.LocationId,
                                    OrganizationId = ticket.OrganizationId,
                                    UserId = user.UserId,
                                    Location = location.LocationName ?? string.Empty,
                                    Contact = contact.ContactName ?? string.Empty,
                                    Time = ticket.CreateDate,
                                    Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                    Description = ticket.TicketName ?? string.Empty,
                                    Category = category.CategoryName ?? string.Empty,
                                    Tech = user.UserName ?? string.Empty,
                                    Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                    ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                    SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                };

                    if (query != null)
                    {
                        ticketMonitorList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRowsByCategory(Guid categoryId).  Error: {0}", ex.Message);

                ticketMonitorList = null;
            }

            return ticketMonitorList;
        }

        public List<TicketMonitorRow> GetTicketMonitorRowsByCategoryAndLocation(Guid categoryId, Guid locationId)
        {
            List<TicketMonitorRow> ticketMonitorList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var query = from ticket in svcPlsContext.Tickets
                                join st in svcPlsContext.SnoozedTickets
                                    on ticket.TicketId equals st.TicketId into ticket_st
                                from snoozedTicket in ticket_st.DefaultIfEmpty()
                                from user in svcPlsContext.Users
                                from category in svcPlsContext.Categories
                                from contact in svcPlsContext.Contacts
                                from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                from servicePlanType in svcPlsContext.ServicePlanTypes
                                from location in svcPlsContext.Locations
                                where category.CategoryId == categoryId &&
                                        location.LocationId == locationId &&
                                        ticket.LocationId == location.LocationId &&
                                        category.CategoryId == ticket.CategoryId &&
                                        ticket.UserId == user.UserId &&
                                        servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                        servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId &&
                                        ticket.ContactId == contact.ContactId
                                select new
                                {
                                    TicketNumber = ticket.TicketNum,
                                    TicketId = ticket.TicketId,
                                    CategoryId = category.CategoryId,
                                    ContactId = contact.ContactId,
                                    LocationId = location.LocationId,
                                    OrganizationId = ticket.OrganizationId,
                                    UserId = user.UserId,
                                    Location = location.LocationName ?? string.Empty,
                                    Contact = contact.ContactName ?? string.Empty,
                                    Time = ticket.CreateDate,
                                    Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                    Description = ticket.TicketName ?? string.Empty,
                                    Category = category.CategoryName ?? string.Empty,
                                    Tech = user.UserName ?? string.Empty,
                                    Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                    ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                    SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                };

                    if (query != null)
                    {
                        ticketMonitorList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRowsByCategoryAndLocation(Guid categoryId, Guid locationId).  Error: {0}", ex.Message);

                ticketMonitorList = null;
            }

            return ticketMonitorList;
        }

        public List<TicketMonitorRow> GetTicketMonitorRowsByLocation(Guid locationId, Guid contactId)
        {
            List<TicketMonitorRow> ticketMonitorList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var query = from ticket in svcPlsContext.Tickets
                                join st in svcPlsContext.SnoozedTickets
                                    on ticket.TicketId equals st.TicketId into ticket_st
                                from snoozedTicket in ticket_st.DefaultIfEmpty()
                                from user in svcPlsContext.Users
                                from category in svcPlsContext.Categories
                                from contact in svcPlsContext.Contacts
                                from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                from servicePlanType in svcPlsContext.ServicePlanTypes
                                from location in svcPlsContext.Locations
                                where location.LocationId == locationId &&
                                        contact.ContactId == contactId &&
                                        ticket.LocationId == location.LocationId &&
                                        category.CategoryId == ticket.CategoryId &&
                                        ticket.UserId == user.UserId &&
                                        servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                        servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId &&
                                        ticket.ContactId == contact.ContactId
                                select new
                                {
                                    TicketNumber = ticket.TicketNum,
                                    TicketId = ticket.TicketId,
                                    CategoryId = category.CategoryId,
                                    ContactId = contact.ContactId,
                                    LocationId = location.LocationId,
                                    OrganizationId = ticket.OrganizationId,
                                    UserId = user.UserId,
                                    Location = location.LocationName ?? string.Empty,
                                    Contact = contact.ContactName ?? string.Empty,
                                    Time = ticket.CreateDate,
                                    Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                    Description = ticket.TicketName ?? string.Empty,
                                    Category = category.CategoryName ?? string.Empty,
                                    Tech = user.UserName ?? string.Empty,
                                    Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                    ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                    SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                };

                    if (query != null)
                    {
                        ticketMonitorList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRowsByLocation(Guid locationId, Guid contactId).  Error: {0}", ex.Message);

                ticketMonitorList = null;
            }

            return ticketMonitorList;
        }

        public List<TicketMonitorRow> GetTicketMonitorRowsByLocations(List<Guid> locationIds)
        {
            List<TicketMonitorRow> ticketMonitorList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    foreach (Guid locationId in locationIds)
                    {
                        List<TicketMonitorRow> ticketMonitorLocationList = null;

                        var query = from ticket in svcPlsContext.Tickets
                                    join st in svcPlsContext.SnoozedTickets
                                    on ticket.TicketId equals st.TicketId into ticket_st
                                    from snoozedTicket in ticket_st.DefaultIfEmpty()
                                    from user in svcPlsContext.Users
                                    from category in svcPlsContext.Categories
                                    from contact in svcPlsContext.Contacts
                                    from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                    from servicePlanType in svcPlsContext.ServicePlanTypes
                                    from location in svcPlsContext.Locations
                                    where location.LocationId == locationId &&
                                            ticket.LocationId == location.LocationId &&
                                            category.CategoryId == ticket.CategoryId &&
                                            ticket.UserId == user.UserId &&
                                            servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                            servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId &&
                                            ticket.ContactId == contact.ContactId
                                    select new
                                    {
                                        TicketNumber = ticket.TicketNum,
                                        TicketId = ticket.TicketId,
                                        CategoryId = category.CategoryId,
                                        ContactId = contact.ContactId,
                                        LocationId = location.LocationId,
                                        OrganizationId = ticket.OrganizationId,
                                        UserId = user.UserId,
                                        Location = location.LocationName ?? string.Empty,
                                        Contact = contact.ContactName ?? string.Empty,
                                        Time = ticket.CreateDate,
                                        Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                        Description = ticket.TicketName ?? string.Empty,
                                        Category = category.CategoryName ?? string.Empty,
                                        Tech = user.UserName ?? string.Empty,
                                        Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                        ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                        SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                    };

                        if (query != null)
                        {
                            ticketMonitorLocationList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;
                        }

                        if (ticketMonitorLocationList != null)
                        {
                            if (ticketMonitorList == null)
                            {
                                ticketMonitorList = new List<TicketMonitorRow>();
                            }

                            ticketMonitorList.AddRange(ticketMonitorLocationList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRowsByLocations(List<Guid> locationIds).  Error: {0}", ex.Message);

                ticketMonitorList = null;
            }

            return ticketMonitorList;
        }

        public List<TicketMonitorRow> GetTicketMonitorRowsByContacts(List<Guid> contactIds)
        {
            List<TicketMonitorRow> ticketMonitorList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    foreach (Guid contactId in contactIds)
                    {
                        List<TicketMonitorRow> ticketMonitorContactList = null;

                        var query = from ticket in svcPlsContext.Tickets
                                    join st in svcPlsContext.SnoozedTickets
                                    on ticket.TicketId equals st.TicketId into ticket_st
                                    from snoozedTicket in ticket_st.DefaultIfEmpty()
                                    from user in svcPlsContext.Users
                                    from category in svcPlsContext.Categories
                                    from contact in svcPlsContext.Contacts
                                    from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                    from servicePlanType in svcPlsContext.ServicePlanTypes
                                    from location in svcPlsContext.Locations
                                    where contact.ContactId == contactId &&
                                            ticket.LocationId == location.LocationId &&
                                            category.CategoryId == ticket.CategoryId &&
                                            ticket.UserId == user.UserId &&
                                            servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                            servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId &&
                                            ticket.ContactId == contact.ContactId
                                    select new
                                    {
                                        TicketNumber = ticket.TicketNum,
                                        TicketId = ticket.TicketId,
                                        CategoryId = category.CategoryId,
                                        ContactId = contact.ContactId,
                                        LocationId = location.LocationId,
                                        OrganizationId = ticket.OrganizationId,
                                        UserId = user.UserId,
                                        Location = location.LocationName ?? string.Empty,
                                        Contact = contact.ContactName ?? string.Empty,
                                        Time = ticket.CreateDate,
                                        Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                        Description = ticket.TicketName ?? string.Empty,
                                        Category = category.CategoryName ?? string.Empty,
                                        Tech = user.UserName ?? string.Empty,
                                        Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                        ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                        SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                    };

                        if (query != null)
                        {
                            ticketMonitorContactList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;
                        }

                        if (ticketMonitorContactList != null)
                        {
                            if (ticketMonitorList == null)
                            {
                                ticketMonitorList = new List<TicketMonitorRow>();
                            }

                            ticketMonitorList.AddRange(ticketMonitorContactList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRowsByContacts(List<Guid> contactIds).  Error: {0}", ex.Message);

                ticketMonitorList = null;
            }

            return ticketMonitorList;
        }

        public List<TicketMonitorRow> GetTicketMonitorRowsByStatus(List<string> status)
        {
            List<TicketMonitorRow> ticketMonitorList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {

                    var query = from ticket in svcPlsContext.Tickets
                                join st in svcPlsContext.SnoozedTickets
                                    on ticket.TicketId equals st.TicketId into ticket_st
                                from snoozedTicket in ticket_st.DefaultIfEmpty()
                                from user in svcPlsContext.Users
                                from category in svcPlsContext.Categories
                                from contact in svcPlsContext.Contacts
                                from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                from servicePlanType in svcPlsContext.ServicePlanTypes
                                from location in svcPlsContext.Locations
                                where ticket.LocationId == location.LocationId && status.Contains(ticket.Status) &&
                                        category.CategoryId == ticket.CategoryId &&
                                        ticket.UserId == user.UserId &&
                                        servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                        servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId &&
                                        ticket.ContactId == contact.ContactId
                                select new
                                {
                                    TicketNumber = ticket.TicketNum,
                                    TicketId = ticket.TicketId,
                                    CategoryId = category.CategoryId,
                                    ContactId = contact.ContactId,
                                    LocationId = location.LocationId,
                                    OrganizationId = ticket.OrganizationId,
                                    UserId = user.UserId,
                                    Location = location.LocationName ?? string.Empty,
                                    Contact = contact.ContactName ?? string.Empty,
                                    Time = ticket.CreateDate,
                                    Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                    Description = ticket.TicketName ?? string.Empty,
                                    Category = category.CategoryName ?? string.Empty,
                                    Tech = user.UserName ?? string.Empty,
                                    Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                    ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                    SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                };


                    if (query != null)
                    {
                        ticketMonitorList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRowsByStatus(string status).  Error: {0}", ex.Message);

                ticketMonitorList = null;
            }

            return ticketMonitorList;
        }

        public TicketMonitorRow GetTicketMonitorRow(Guid ticketId)
        {
            TicketMonitorRow ticketMonitorRow = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var query = from ticket in svcPlsContext.Tickets
                                join st in svcPlsContext.SnoozedTickets
                                    on ticket.TicketId equals st.TicketId into ticket_st
                                from snoozedTicket in ticket_st.DefaultIfEmpty()
                                from user in svcPlsContext.Users
                                from category in svcPlsContext.Categories
                                from contact in svcPlsContext.Contacts
                                from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                from servicePlanType in svcPlsContext.ServicePlanTypes
                                from location in svcPlsContext.Locations
                                where ticket.TicketId == ticketId &&
                                        ticket.LocationId == location.LocationId &&
                                        category.CategoryId == ticket.CategoryId &&
                                        ticket.UserId == user.UserId &&
                                        servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                        servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId &&
                                        ticket.ContactId == contact.ContactId
                                select new
                                {
                                    TicketNumber = ticket.TicketNum,
                                    TicketId = ticket.TicketId,
                                    CategoryId = category.CategoryId,
                                    ContactId = contact.ContactId,
                                    LocationId = location.LocationId,
                                    OrganizationId = ticket.OrganizationId,
                                    UserId = user.UserId,
                                    Location = location.LocationName ?? string.Empty,
                                    Contact = contact.ContactName ?? string.Empty,
                                    Time = ticket.CreateDate,
                                    Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                    Description = ticket.TicketName ?? string.Empty,
                                    Category = category.CategoryName ?? string.Empty,
                                    Tech = user.UserName ?? string.Empty,
                                    Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                    ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                    SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                };

                    if (query != null)
                    {
                        ticketMonitorRow = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as TicketMonitorRow;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRow(Guid ticketId).  Error: {0}", ex.Message);

                ticketMonitorRow = null;
            }

            return ticketMonitorRow;
        }

        public List<TicketMonitorRow> GetTicketMonitorRowsByUser(Guid userId)
        {
            List<TicketMonitorRow> ticketMonitorList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var query = from ticket in svcPlsContext.Tickets
                                join st in svcPlsContext.SnoozedTickets
                                    on ticket.TicketId equals st.TicketId into ticket_st
                                from snoozedTicket in ticket_st.DefaultIfEmpty()
                                from user in svcPlsContext.Users
                                from category in svcPlsContext.Categories
                                from contact in svcPlsContext.Contacts
                                from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                from servicePlanType in svcPlsContext.ServicePlanTypes
                                from location in svcPlsContext.Locations
                                where user.UserId == userId &&
                                        location.LocationId == ticket.LocationId &&
                                        contact.ContactId == ticket.ContactId &&
                                        category.CategoryId == ticket.CategoryId &&
                                        ticket.UserId == user.UserId &&
                                        servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                        servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId
                                select new
                                {
                                    TicketNumber = ticket.TicketNum,
                                    TicketId = ticket.TicketId,
                                    CategoryId = category.CategoryId,
                                    ContactId = contact.ContactId,
                                    LocationId = location.LocationId,
                                    OrganizationId = ticket.OrganizationId,
                                    UserId = user.UserId,
                                    Location = location.LocationName ?? string.Empty,
                                    Contact = contact.ContactName ?? string.Empty,
                                    Time = ticket.CreateDate,
                                    Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                    Description = ticket.TicketName ?? string.Empty,
                                    Category = category.CategoryName ?? string.Empty,
                                    Tech = user.UserName ?? string.Empty,
                                    Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                    ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                    SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                };

                    if (query != null)
                    {
                        ticketMonitorList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRowsByUser(Guid userId).  Error: {0}", ex.Message);

                ticketMonitorList = null;
            }

            return ticketMonitorList;
        }

        public List<TicketMonitorRow> GetTicketMonitorRowsByServicePlanType(Guid servicePlanTypeId)
        {
            List<TicketMonitorRow> ticketMonitorList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var query = from ticket in svcPlsContext.Tickets
                                join st in svcPlsContext.SnoozedTickets
                                    on ticket.TicketId equals st.TicketId into ticket_st
                                from snoozedTicket in ticket_st.DefaultIfEmpty()
                                from user in svcPlsContext.Users
                                from category in svcPlsContext.Categories
                                from contact in svcPlsContext.Contacts
                                from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                from servicePlanType in svcPlsContext.ServicePlanTypes
                                from location in svcPlsContext.Locations
                                where servicePlanType.ServicePlanTypeId == servicePlanTypeId &&
                                        servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                        servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId &&
                                        location.LocationId == ticket.LocationId &&
                                        contact.ContactId == ticket.ContactId &&
                                        category.CategoryId == ticket.CategoryId &&
                                        ticket.UserId == user.UserId
                                select new
                                {
                                    TicketNumber = ticket.TicketNum,
                                    TicketId = ticket.TicketId,
                                    CategoryId = category.CategoryId,
                                    ContactId = contact.ContactId,
                                    LocationId = location.LocationId,
                                    OrganizationId = ticket.OrganizationId,
                                    UserId = user.UserId,
                                    Location = location.LocationName ?? string.Empty,
                                    Contact = contact.ContactName ?? string.Empty,
                                    Time = ticket.CreateDate,
                                    Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                    Description = ticket.TicketName ?? string.Empty,
                                    Category = category.CategoryName ?? string.Empty,
                                    Tech = user.UserName ?? string.Empty,
                                    Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                    ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                    SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                };

                    if (query != null)
                    {
                        ticketMonitorList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRowsByServicePlanType(Guid servicePlanTypeId).  Error: {0}", ex.Message);

                ticketMonitorList = null;
            }

            return ticketMonitorList;
        }

        public List<TicketMonitorRow> GetTicketMonitorRowsByServicePlanName(string servicePlanName)
        {
            List<TicketMonitorRow> ticketMonitorList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var query = from ticket in svcPlsContext.Tickets
                                join st in svcPlsContext.SnoozedTickets
                                    on ticket.TicketId equals st.TicketId into ticket_st
                                from snoozedTicket in ticket_st.DefaultIfEmpty()
                                from user in svcPlsContext.Users
                                from category in svcPlsContext.Categories
                                from contact in svcPlsContext.Contacts
                                from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                from servicePlanType in svcPlsContext.ServicePlanTypes
                                from location in svcPlsContext.Locations
                                where servicePlanType.Name == servicePlanName &&
                                        servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                        servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId &&
                                        location.LocationId == ticket.LocationId &&
                                        contact.ContactId == ticket.ContactId &&
                                        category.CategoryId == ticket.CategoryId &&
                                        ticket.UserId == user.UserId
                                select new
                                {
                                    TicketNumber = ticket.TicketNum,
                                    TicketId = ticket.TicketId,
                                    CategoryId = category.CategoryId,
                                    ContactId = contact.ContactId,
                                    LocationId = location.LocationId,
                                    OrganizationId = ticket.OrganizationId,
                                    UserId = user.UserId,
                                    Location = location.LocationName ?? string.Empty,
                                    Contact = contact.ContactName ?? string.Empty,
                                    Time = ticket.CreateDate,
                                    Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                    Description = ticket.TicketName ?? string.Empty,
                                    Category = category.CategoryName ?? string.Empty,
                                    Tech = user.UserName ?? string.Empty,
                                    Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                    ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                    SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                };

                    if (query != null)
                    {
                        ticketMonitorList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRowsByServicePlanName(string servicePlanName).  Error: {0}", ex.Message);

                ticketMonitorList = null;
            }

            return ticketMonitorList;
        }

        public List<TicketMonitorRow> GetTicketMonitorRowsByTechs(List<Guid> techIds)
        {
            List<TicketMonitorRow> ticketMonitorList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    foreach (Guid techIdEntry in techIds)
                    {
                        List<TicketMonitorRow> ticketMonitorTechList = null;

                        var query = from ticket in svcPlsContext.Tickets
                                    join st in svcPlsContext.SnoozedTickets
                                    on ticket.TicketId equals st.TicketId into ticket_st
                                    from snoozedTicket in ticket_st.DefaultIfEmpty()
                                    from user in svcPlsContext.Users
                                    from category in svcPlsContext.Categories
                                    from contact in svcPlsContext.Contacts
                                    from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                    from servicePlanType in svcPlsContext.ServicePlanTypes
                                    from location in svcPlsContext.Locations
                                    where user.UserId == techIdEntry &&
                                            location.LocationId == ticket.LocationId &&
                                            contact.ContactId == ticket.ContactId &&
                                            category.CategoryId == ticket.CategoryId &&
                                            ticket.UserId == user.UserId &&
                                            servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                            servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId
                                    select new
                                    {
                                        TicketNumber = ticket.TicketNum,
                                        TicketId = ticket.TicketId,
                                        CategoryId = category.CategoryId,
                                        ContactId = contact.ContactId,
                                        LocationId = location.LocationId,
                                        OrganizationId = ticket.OrganizationId,
                                        UserId = user.UserId,
                                        Location = location.LocationName ?? string.Empty,
                                        Contact = contact.ContactName ?? string.Empty,
                                        Time = ticket.CreateDate,
                                        Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                        Description = ticket.TicketName ?? string.Empty,
                                        Category = category.CategoryName ?? string.Empty,
                                        Tech = user.UserName ?? string.Empty,
                                        Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                        ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                        SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                    };

                        if (query != null)
                        {
                            ticketMonitorTechList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;
                        }

                        if (ticketMonitorTechList != null)
                        {
                            if (ticketMonitorList == null)
                            {
                                ticketMonitorList = new List<TicketMonitorRow>();
                            }

                            ticketMonitorList.AddRange(ticketMonitorTechList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRowsByTechs(List<Guid> techIds).  Error: {0}", ex.Message);

                ticketMonitorList = null;
            }

            return ticketMonitorList;
        }

        public List<TicketMonitorRow> GetTicketMonitorRowsByCategories(List<Guid> categoryIds)
        {
            List<TicketMonitorRow> ticketMonitorList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    foreach (Guid categoryIdEntry in categoryIds)
                    {
                        List<TicketMonitorRow> ticketMonitorCategoryList = null;

                        var query = from ticket in svcPlsContext.Tickets
                                    join st in svcPlsContext.SnoozedTickets
                                    on ticket.TicketId equals st.TicketId into ticket_st
                                    from snoozedTicket in ticket_st.DefaultIfEmpty()
                                    from user in svcPlsContext.Users
                                    from category in svcPlsContext.Categories
                                    from contact in svcPlsContext.Contacts
                                    from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                    from servicePlanType in svcPlsContext.ServicePlanTypes
                                    from location in svcPlsContext.Locations
                                    where category.CategoryId == categoryIdEntry &&
                                            ticket.LocationId == location.LocationId &&
                                            category.CategoryId == ticket.CategoryId &&
                                            ticket.UserId == user.UserId &&
                                            servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                            servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId &&
                                            ticket.ContactId == contact.ContactId
                                    select new
                                    {
                                        TicketNumber = ticket.TicketNum,
                                        TicketId = ticket.TicketId,
                                        CategoryId = category.CategoryId,
                                        ContactId = contact.ContactId,
                                        LocationId = location.LocationId,
                                        OrganizationId = ticket.OrganizationId,
                                        UserId = user.UserId,
                                        Location = location.LocationName ?? string.Empty,
                                        Contact = contact.ContactName ?? string.Empty,
                                        Time = ticket.CreateDate,
                                        Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                        Description = ticket.TicketName ?? string.Empty,
                                        Category = category.CategoryName ?? string.Empty,
                                        Tech = user.UserName ?? string.Empty,
                                        Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                        ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                        SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                    };

                        if (query != null)
                        {
                            ticketMonitorCategoryList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;
                        }

                        if (ticketMonitorCategoryList != null)
                        {
                            if (ticketMonitorList == null)
                            {
                                ticketMonitorList = new List<TicketMonitorRow>();
                            }

                            ticketMonitorList.AddRange(ticketMonitorCategoryList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRowsByCategories(List<Guid> categoryIds).  Error: {0}", ex.Message);

                ticketMonitorList = null;
            }

            return ticketMonitorList;
        }

        public List<TicketMonitorRow> GetTicketMonitorRowsByServicePlanTypes(List<Guid> servicePlanTypeIds)
        {
            List<TicketMonitorRow> ticketMonitorList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    foreach (Guid servicePlanTypeId in servicePlanTypeIds)
                    {
                        List<TicketMonitorRow> ticketMonitorServicePlanTypeList = null;

                        var query = from ticket in svcPlsContext.Tickets
                                    join st in svcPlsContext.SnoozedTickets
                                    on ticket.TicketId equals st.TicketId into ticket_st
                                    from snoozedTicket in ticket_st.DefaultIfEmpty()
                                    from user in svcPlsContext.Users
                                    from category in svcPlsContext.Categories
                                    from contact in svcPlsContext.Contacts
                                    from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                    from servicePlanType in svcPlsContext.ServicePlanTypes
                                    from location in svcPlsContext.Locations
                                    where servicePlanType.ServicePlanTypeId == servicePlanTypeId &&
                                            ticket.LocationId == location.LocationId &&
                                            category.CategoryId == ticket.CategoryId &&
                                            ticket.UserId == user.UserId &&
                                            servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                            servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId &&
                                            ticket.ContactId == contact.ContactId
                                    select new
                                    {
                                        TicketNumber = ticket.TicketNum,
                                        TicketId = ticket.TicketId,
                                        CategoryId = category.CategoryId,
                                        ContactId = contact.ContactId,
                                        LocationId = location.LocationId,
                                        OrganizationId = ticket.OrganizationId,
                                        UserId = user.UserId,
                                        Location = location.LocationName ?? string.Empty,
                                        Contact = contact.ContactName ?? string.Empty,
                                        Time = ticket.CreateDate,
                                        Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                        Description = ticket.TicketName ?? string.Empty,
                                        Category = category.CategoryName ?? string.Empty,
                                        Tech = user.UserName ?? string.Empty,
                                        Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                        ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                        SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                    };

                        if (query != null)
                        {
                            ticketMonitorServicePlanTypeList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;
                        }

                        if (ticketMonitorServicePlanTypeList != null)
                        {
                            if (ticketMonitorList == null)
                            {
                                ticketMonitorList = new List<TicketMonitorRow>();
                            }

                            ticketMonitorList.AddRange(ticketMonitorServicePlanTypeList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRowsByServicePlanTypes(List<Guid> servicePlanTypeIds).  Error: {0}", ex.Message);

                ticketMonitorList = null;
            }

            return ticketMonitorList;
        }

        public List<TicketMonitorRow> GetTicketMonitorRowsByUserAndIsStatusClosed(Guid userId, bool isStatusClosed)
        {
            List<TicketMonitorRow> ticketMonitorList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    if (isStatusClosed == true)
                    {
                        var query = from ticket in svcPlsContext.Tickets
                                    join st in svcPlsContext.SnoozedTickets
                                        on ticket.TicketId equals st.TicketId into ticket_st
                                    from snoozedTicket in ticket_st.DefaultIfEmpty()
                                    from user in svcPlsContext.Users
                                    from category in svcPlsContext.Categories
                                    from contact in svcPlsContext.Contacts
                                    from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                    from servicePlanType in svcPlsContext.ServicePlanTypes
                                    from location in svcPlsContext.Locations
                                    where user.UserId == userId &&
                                            location.LocationId == ticket.LocationId &&
                                            contact.ContactId == ticket.ContactId &&
                                            category.CategoryId == ticket.CategoryId &&
                                            ticket.UserId == user.UserId && ticket.Status == "C" &&
                                            servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                            servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId
                                    select new
                                    {
                                        TicketNumber = ticket.TicketNum,
                                        TicketId = ticket.TicketId,
                                        CategoryId = category.CategoryId,
                                        ContactId = contact.ContactId,
                                        LocationId = location.LocationId,
                                        OrganizationId = ticket.OrganizationId,
                                        UserId = user.UserId,
                                        Location = location.LocationName ?? string.Empty,
                                        Contact = contact.ContactName ?? string.Empty,
                                        Time = ticket.CreateDate,
                                        Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                        Description = ticket.TicketName ?? string.Empty,
                                        Category = category.CategoryName ?? string.Empty,
                                        Tech = user.UserName ?? string.Empty,
                                        Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                        ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                        SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                    };

                        if (query != null)
                        {
                            ticketMonitorList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;
                        }
                    }
                    else
                    {
                        var query = from ticket in svcPlsContext.Tickets
                                    join st in svcPlsContext.SnoozedTickets
                                        on ticket.TicketId equals st.TicketId into ticket_st
                                    from snoozedTicket in ticket_st.DefaultIfEmpty()
                                    from user in svcPlsContext.Users
                                    from category in svcPlsContext.Categories
                                    from contact in svcPlsContext.Contacts
                                    from servicePlanInfo in svcPlsContext.ServicePlanInfos
                                    from servicePlanType in svcPlsContext.ServicePlanTypes
                                    from location in svcPlsContext.Locations
                                    where user.UserId == userId &&
                                            location.LocationId == ticket.LocationId &&
                                            contact.ContactId == ticket.ContactId &&
                                            category.CategoryId == ticket.CategoryId &&
                                            ticket.UserId == user.UserId && ticket.Status != "C" &&
                                            servicePlanInfo.ServicePlanInfoId == location.ServicePlanInfoId &&
                                            servicePlanType.ServicePlanTypeId == servicePlanInfo.ServicePlanTypeId
                                    select new
                                    {
                                        TicketNumber = ticket.TicketNum,
                                        TicketId = ticket.TicketId,
                                        CategoryId = category.CategoryId,
                                        ContactId = contact.ContactId,
                                        LocationId = location.LocationId,
                                        OrganizationId = ticket.OrganizationId,
                                        UserId = user.UserId,
                                        Location = location.LocationName ?? string.Empty,
                                        Contact = contact.ContactName ?? string.Empty,
                                        Time = ticket.CreateDate,
                                        Elapsed = (DateTime.Now - (Convert.ToDateTime(ticket.CreateDate))).TotalMinutes,
                                        Description = ticket.TicketName ?? string.Empty,
                                        Category = category.CategoryName ?? string.Empty,
                                        Tech = user.UserName ?? string.Empty,
                                        Status = ticket.Status != null ? ticket.Status[0].ToString() : string.Empty,
                                        ServicePlan = servicePlanType.Initial != null ? servicePlanType.Initial.ToString() : string.Empty,
                                        SnoozeTicket = snoozedTicket.SnoozeId != null ? true : false
                                    };

                        if (query != null)
                        {
                            ticketMonitorList = query.ToNonAnonymousList(typeof(TicketMonitorRow)) as List<TicketMonitorRow>;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketMonitorRowsByUserAndStatus(Guid userId, string status).  Error: {0}", ex.Message);

                ticketMonitorList = null;
            }

            return ticketMonitorList;
        }
        #endregion TicketMonitor Methods

        #region Solution Methods
        /// <summary>
        /// List<Solution> GetSolutions()
        /// </summary>
        /// <returns></returns>
        public List<Solution> GetSolutions()
        {
            List<Solution> solutionList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var solutions = svcPlsContext.Solutions.OrderBy(s => s.CreateDate);

                    if (solutions != null)
                    {
                        solutionList = solutions.ToList<Solution>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetSolutions().  Error: {0}", ex.Message);

                solutionList = null;
            }
            return solutionList.OrderByDescending(fp => fp.CreateDate).ToList<Solution>();

        }

        /// <summary>
        /// Solution GetSolution(Guid solutionId)
        /// </summary>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public Solution GetSolution(Guid solutionId)
        {
            Solution solution = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var solutions = svcPlsContext.Solutions.Where(s => s.SolutionId == solutionId);

                    if (solutions != null)
                    {
                        solution = solutions.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetSolution(Guid solutionId).  Error: {0}", ex.Message);

                solution = null;
            }

            return solution;
        }

        /// <summary>
        /// Solution AddSolution(Solution solution)
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        public Solution AddSolution(Solution solution)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.Solutions.InsertOnSubmit(solution);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddSolution().  Error: {0}", ex.Message);

                solution = null;
            }

            return solution;
        }

        /// <summary>
        /// Solution UpdateSolution(Solution solution, Guid solutionId)
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public Solution UpdateSolution(Solution solution, Guid solutionId)
        {
            Solution sol = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific Solution from Database
                    sol = (from s in svcPlsContext.Solutions
                           where s.SolutionId == solutionId
                           select s).FirstOrDefault();

                    if (sol != null)
                    {
                        sol.SolutionShortDesc = solution.SolutionShortDesc;
                        sol.SolutionText = solution.SolutionText;
                        sol.TicketId = solution.TicketId;
                        //sol.CreateDate = solution.CreateDate;
                        sol.EditDate = solution.EditDate;

                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateSolution().  Error: {0}", ex.Message);

                sol = null;
            }

            return sol;
        }

        /// <summary>
        /// bool DeleteSolution(Guid solutionId)
        /// </summary>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public bool DeleteSolution(Guid solutionId)
        {
            bool result = false;

            Solution solution = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific Solution object from Database
                    solution = (from s in svcPlsContext.Solutions
                                where s.SolutionId == solutionId
                                select s).FirstOrDefault();

                    if (solution != null)
                    {
                        // Delete the Solution object
                        svcPlsContext.Solutions.DeleteOnSubmit(solution);

                        // Save to database
                        svcPlsContext.SubmitChanges();

                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteSolution().  Error: {0}", ex.Message);

                result = false;
            }

            return result;
        }


        ///<summary>
        ///List<SolutionsWithBlob> GetSolutionsByTicketId(Guid ticketId)
        ///</summary>
        ///<param name="ticketId"></param>
        ///<returns></returns>
        public List<SolutionsWithBlob> GetSolutionsByTicketId(Guid ticketId)
        {
            List<SolutionsWithBlob> solutionList = new List<SolutionsWithBlob>();
            SolutionsWithBlob sb = new SolutionsWithBlob();
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var solutions = svcPlsContext.SP_SolutionBlobIcon(ticketId);

                    if (solutions != null)
                    {
                        foreach (var soln in solutions)
                        {
                            sb = new SolutionsWithBlob();
                            sb.SolutionId = (Guid)soln.SolutionId;
                            sb.TicketId = (Guid)soln.TicketId;
                            sb.SolutionShortDesc = soln.SolutionShortDesc;
                            sb.SolutionText = soln.SolutionText;
                            sb.LikeCount = soln.LikeCount.ToString();
                            sb.UnlikeCount = soln.UnlikeCount.ToString();
                            sb.CreateDate = soln.CreateDate;
                            sb.EditDate = soln.EditDate;
                            sb.UserId = (Guid)soln.UserId;
                            sb.BlobsIconType = soln.BlobTypeCount.ToString();

                            solutionList.Add(sb);
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetSolutionsByTicketId().  Error: {0}", ex.Message);

                solutionList = null;
            }
            return solutionList.OrderByDescending(pl => Convert.ToDateTime(pl.CreateDate)).ToList<SolutionsWithBlob>();

        }

        #endregion Solution Methods

        #region Problem Methods
        /// <summary>
        /// List<Problem> GetProblems()
        /// </summary>
        /// <returns></returns>
        public List<Problem> GetProblems()
        {
            List<Problem> problemList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var problems = svcPlsContext.Problems.OrderBy(p => p.CreateDate);

                    if (problems != null)
                    {
                        problemList = problems.ToList<Problem>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetProblems().  Error: {0}", ex.Message);

                problemList = null;
            }
            return problemList.OrderByDescending(fp => fp.CreateDate).ToList<Problem>();

        }

        /// <summary>
        /// Problem GetProblem(Guid problemId)
        /// </summary>
        /// <param name="problemId"></param>
        /// <returns></returns>
        public Problem GetProblem(Guid problemId)
        {
            Problem problem = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var problems = svcPlsContext.Problems.Where(p => p.ProblemId == problemId);

                    if (problems != null)
                    {
                        problem = problems.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetProblem(Guid problemId).  Error: {0}", ex.Message);

                problem = null;
            }

            return problem;
        }

        /// <summary>
        /// Problem AddProblem(Problem problem)
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        public Problem AddProblem(Problem problem)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.Problems.InsertOnSubmit(problem);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddProblem().  Error: {0}", ex.Message);

                problem = null;
            }

            return problem;
        }

        /// <summary>
        /// Problem UpdateProblem(Problem problem, Guid problemId)
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="problemId"></param>
        /// <returns></returns>
        public Problem UpdateProblem(Problem problem, Guid problemId)
        {
            Problem prob = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific Problem from Database
                    prob = (from p in svcPlsContext.Problems
                            where p.ProblemId == problemId
                            select p).FirstOrDefault();

                    if (prob != null)
                    {
                        prob.ProblemShortDesc = problem.ProblemShortDesc;
                        prob.ProblemText = problem.ProblemText;
                        prob.TicketId = problem.TicketId;
                        //prob.CreateDate = problem.CreateDate;
                        prob.EditDate = problem.EditDate;

                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateProblem().  Error: {0}", ex.Message);

                prob = null;
            }

            return prob;
        }

        /// <summary>
        /// bool DeleteProblem(Guid problemId)
        /// </summary>
        /// <param name="problemId"></param>
        /// <returns></returns>
        public bool DeleteProblem(Guid problemId)
        {
            bool result = false;

            Problem problem = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific Problem object from Database
                    problem = (from p in svcPlsContext.Problems
                               where p.ProblemId == problemId
                               select p).FirstOrDefault();

                    if (problem != null)
                    {
                        // Delete the Problem object
                        svcPlsContext.Problems.DeleteOnSubmit(problem);

                        // Save to database
                        svcPlsContext.SubmitChanges();

                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteProblem().  Error: {0}", ex.Message);

                result = false;
            }

            return result;
        }

        ///<summary>
        ///List<ProblemsWithBlob> GetProblemsByTicketId(Guid ticketId)
        ///</summary>
        ///<param name="ticketId"></param>
        ///<returns></returns>
        public List<ProblemsWithBlob> GetProblemsByTicketId(Guid ticketId)
        {
            List<ProblemsWithBlob> problemList = new List<ProblemsWithBlob>();
            ProblemsWithBlob pb = new ProblemsWithBlob();
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var problems = svcPlsContext.SP_ProblemBlobIcon(ticketId);

                    if (problems != null)
                    {
                        foreach (var pblm in problems)
                        {
                            pb = new ProblemsWithBlob();
                            pb.ProblemId = (Guid)pblm.ProblemId;
                            pb.TicketId = (Guid)pblm.TicketId;
                            pb.ProblemShortDesc = pblm.ProblemShortDesc;
                            pb.ProblemText = pblm.ProblemText;
                            pb.LikeCount = pblm.LikeCount.ToString();
                            pb.UnlikeCount = pblm.UnlikeCount.ToString();
                            pb.CreateDate = pblm.CreateDate;
                            pb.EditDate = pblm.EditDate;
                            pb.UserId = (Guid)pblm.UserId;
                            pb.BlobsIconType = pblm.BlobTypeCount.ToString();

                            problemList.Add(pb);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetProblemsByTicketId().  Error: {0}", ex.Message);

                problemList = null;
            }
            return problemList.OrderByDescending(pl => Convert.ToDateTime(pl.CreateDate)).ToList<ProblemsWithBlob>();

        }
        #endregion Problem Methods

        #region Solution Blob Methods
        /// <summary>
        /// List<SolutionBlobPacket> GetSolutionBlobs(Guid solutionId)
        /// </summary>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public List<SolutionBlobPacket> GetSolutionBlobs(Guid solutionId)
        {
            List<SolutionBlobPacket> SolutionBlobList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var SolutionBlobs = from si in svcPlsContext.SolutionBlobs
                                        from ie in svcPlsContext.BlobEntries
                                        where si.SolutionId == solutionId &&
                                              si.BlobId == ie.BlobId
                                        orderby ie.CreateDate
                                        select new
                                        {
                                            SolutionBlobId = si.SolutionBlobId,
                                            SolutionId = si.SolutionId,
                                            BlobEntryId = ie.BlobId,
                                            BlobTypeId = ie.BlobTypeId,
                                            BlobBytes = Convert.ToBase64String(ie.Blob.ToArray())
                                        };

                    if (SolutionBlobs != null)
                    {
                        SolutionBlobList = SolutionBlobs.ToNonAnonymousList(typeof(SolutionBlobPacket)) as List<SolutionBlobPacket>;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetSolutionBlobs(Guid solutionId).  Error: {0}", ex.Message);

                SolutionBlobList = null;
            }

            return SolutionBlobList;
        }

        /// <summary>
        /// AddSolutionBlob(BlobPacket blobPacket)
        /// </summary>
        /// <param name="BlobBytes"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public SolutionBlob AddSolutionBlob(BlobPacket blobPacket)
        {
            SolutionBlob newSolutionBlob = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    BlobEntry blobEntry = new BlobEntry()
                    {
                        BlobId = Guid.NewGuid(),
                        BlobTypeId = blobPacket.BlobTypeId,
                        Blob = Convert.FromBase64String(blobPacket.BlobBytes),
                        CreateDate = blobPacket.CreateDate,
                        EditDate = blobPacket.EditDate
                    };

                    svcPlsContext.BlobEntries.InsertOnSubmit(blobEntry);

                    svcPlsContext.SubmitChanges();

                    newSolutionBlob = new SolutionBlob()
                    {
                        SolutionBlobId = Guid.NewGuid(),
                        BlobId = blobEntry.BlobId,
                        SolutionId = blobPacket.EntityId,
                        CreateDate = blobEntry.CreateDate,
                        EditDate = blobEntry.EditDate
                    };

                    svcPlsContext.SolutionBlobs.InsertOnSubmit(newSolutionBlob);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddSolutionBlobs(BlobPacket blobPacket).  Error: {0}", ex.Message);

                newSolutionBlob = null;
            }

            return newSolutionBlob;
        }

        /// <summary>
        /// UpdateSolutionBlob(BlobPacket blobPacket)
        /// </summary>
        /// <param name="BlobBytes"></param>
        /// <param name="SolutionBlobId"></param>
        /// <returns></returns>
        public bool UpdateSolutionBlob(BlobPacket blobPacket)
        {
            bool SolutionBlobUpdated = false;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific SolutionBlob from Database
                    var solutionBlob = (from si in svcPlsContext.SolutionBlobs
                                        where si.SolutionBlobId == blobPacket.EntityId
                                        select si).FirstOrDefault();

                    if (solutionBlob != null)
                    {
                        // Now find the BlobEntry that corresponds to this 
                        // SolutionBlob entry.
                        var blobEntry = (from ie in svcPlsContext.BlobEntries
                                         where ie.BlobId == solutionBlob.BlobId
                                         select ie).FirstOrDefault();

                        blobEntry.Blob = Convert.FromBase64String(blobPacket.BlobBytes);
                        blobEntry.EditDate = blobPacket.EditDate;

                        //Save to database
                        svcPlsContext.SubmitChanges();

                        SolutionBlobUpdated = true;
                    }
                    else
                    {
                        SolutionBlobUpdated = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateSolutionBlob(BlobPacket blobPacket).  Error: {0}", ex.Message);

                SolutionBlobUpdated = false;
            }

            return SolutionBlobUpdated;
        }

        /// <summary>
        /// DeleteSolutionBlob(Guid solutionBlobId)
        /// </summary>
        /// <param name="solutionBlobId"></param>
        /// <returns></returns>
        public bool DeleteSolutionBlob(Guid solutionBlobId)
        {
            bool deleteWasSuccessful = false;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific SolutionBlob object from Database
                    var solutionBlob = (from si in svcPlsContext.SolutionBlobs
                                        where si.SolutionBlobId == solutionBlobId
                                        select si).FirstOrDefault();

                    if (solutionBlob != null)
                    {
                        // Now find the BlobEntry that corresponds to 
                        // the found SolutionBlob.
                        var blobEntry = (from ie in svcPlsContext.BlobEntries
                                         where ie.BlobId == solutionBlob.BlobId
                                         select ie).FirstOrDefault();

                        if (blobEntry != null)
                        {
                            // Delete the BlobEntry object
                            svcPlsContext.BlobEntries.DeleteOnSubmit(blobEntry);

                            // Delete the SolutionBlob object
                            svcPlsContext.SolutionBlobs.DeleteOnSubmit(solutionBlob);

                            // Save to database
                            svcPlsContext.SubmitChanges();

                            deleteWasSuccessful = true;
                        }
                        else
                        {
                            deleteWasSuccessful = false;
                        }
                    }
                    else
                    {
                        deleteWasSuccessful = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteSolutionBlob(Guid solutionBlobId).  Error: {0}", ex.Message);

                deleteWasSuccessful = false;
            }

            return deleteWasSuccessful;
        }
        #endregion Solution Blob Methods

        #region Problem Blob Methods
        /// <summary>
        /// List<BlobEntry> GetProblemBlobs(Guid problemId)
        /// </summary>
        /// <param name="problemId"></param>
        /// <returns></returns>
        public List<ProblemBlobPacket> GetProblemBlobs(Guid problemId)
        {
            List<ProblemBlobPacket> ProblemBlobList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var ProblemBlobs = from pi in svcPlsContext.ProblemBlobs
                                       from ie in svcPlsContext.BlobEntries
                                       where pi.ProblemId == problemId &&
                                             pi.BlobId == ie.BlobId
                                       orderby ie.CreateDate
                                       select new
                                       {
                                           ProblemBlobId = pi.ProblemBlobId,
                                           ProblemId = pi.ProblemId,
                                           BlobEntryId = ie.BlobId,
                                           BlobTypeId = ie.BlobTypeId,
                                           BlobBytes = Convert.ToBase64String(ie.Blob.ToArray())
                                       };

                    if (ProblemBlobs != null)
                    {
                        ProblemBlobList = ProblemBlobs.ToNonAnonymousList(typeof(ProblemBlobPacket)) as List<ProblemBlobPacket>;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in ProblemBlobs(Guid problemId).  Error: {0}", ex.Message);

                ProblemBlobList = null;
            }

            return ProblemBlobList;
        }

        /// <summary>
        /// ProblemBlob AddProblemBlob(BlobPacket blobPacket)
        /// </summary>
        /// <param name="BlobBytes"></param>
        /// <param name="problemId"></param>
        /// <returns></returns>
        public ProblemBlob AddProblemBlob(BlobPacket blobPacket)
        {
            ProblemBlob newProblemBlob = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    BlobEntry blobEntry = new BlobEntry()
                    {
                        BlobId = Guid.NewGuid(),
                        BlobTypeId = blobPacket.BlobTypeId,
                        Blob = Convert.FromBase64String(blobPacket.BlobBytes),
                        CreateDate = blobPacket.CreateDate,
                        EditDate = blobPacket.EditDate
                    };

                    svcPlsContext.BlobEntries.InsertOnSubmit(blobEntry);

                    svcPlsContext.SubmitChanges();

                    newProblemBlob = new ProblemBlob()
                    {
                        ProblemBlobId = Guid.NewGuid(),
                        BlobId = blobEntry.BlobId,
                        ProblemId = blobPacket.EntityId,
                        CreateDate = blobEntry.CreateDate,
                        EditDate = blobEntry.EditDate
                    };

                    svcPlsContext.ProblemBlobs.InsertOnSubmit(newProblemBlob);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddProblemBlob(BlobPacket blobPacket).  Error: {0}", ex.Message);

                newProblemBlob = null;
            }

            return newProblemBlob;
        }

        /// <summary>
        /// bool UpdateProblemBlob(BlobPacket BlobPacket)
        /// </summary>
        /// <param name="BlobBytes"></param>
        /// <param name="ProblemBlobId"></param>
        /// <returns></returns>
        public bool UpdateProblemBlob(BlobPacket blobPacket)
        {
            bool problemBlobUpdated = false;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific ProblemBlob from Database
                    var problemBlob = (from pi in svcPlsContext.ProblemBlobs
                                       where pi.ProblemBlobId == blobPacket.EntityId
                                       select pi).FirstOrDefault();

                    if (problemBlob != null)
                    {
                        // Now find the BlobEntry that corresponds to this 
                        // SolutionBlob entry.
                        var blobEntry = (from ie in svcPlsContext.BlobEntries
                                         where ie.BlobId == problemBlob.BlobId
                                         select ie).FirstOrDefault();

                        blobEntry.Blob = Convert.FromBase64String(blobPacket.BlobBytes);
                        blobEntry.EditDate = blobPacket.EditDate;

                        //Save to database
                        svcPlsContext.SubmitChanges();

                        problemBlobUpdated = true;
                    }
                    else
                    {
                        problemBlobUpdated = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateProblemBlob(BlobPacket blobPacket).  Error: {0}", ex.Message);

                problemBlobUpdated = false;
            }

            return problemBlobUpdated;
        }

        /// <summary>
        /// bool DeleteProblemBlob(Guid problemBlobId)
        /// </summary>
        /// <param name="problemBlobId"></param>
        /// <returns></returns>
        public bool DeleteProblemBlob(Guid problemBlobId)
        {
            bool deleteWasSuccessful = false;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific ProblemBlob object from Database
                    var problemBlob = (from pi in svcPlsContext.ProblemBlobs
                                       where pi.ProblemBlobId == problemBlobId
                                       select pi).FirstOrDefault();

                    if (problemBlob != null)
                    {
                        // Now find the BlobEntry that corresponds to 
                        // the found SolutionBlob.
                        var BlobEntry = (from ie in svcPlsContext.BlobEntries
                                         where ie.BlobId == problemBlob.BlobId
                                         select ie).FirstOrDefault();

                        if (BlobEntry != null)
                        {
                            // Delete the BlobEntry object
                            svcPlsContext.BlobEntries.DeleteOnSubmit(BlobEntry);

                            // Delete the ProblemBlob object
                            svcPlsContext.ProblemBlobs.DeleteOnSubmit(problemBlob);

                            // Save to database
                            svcPlsContext.SubmitChanges();

                            deleteWasSuccessful = true;
                        }
                        else
                        {
                            deleteWasSuccessful = false;
                        }
                    }
                    else
                    {
                        deleteWasSuccessful = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteProblemBlob(Guid problemBlobId).  Error: {0}", ex.Message);

                deleteWasSuccessful = false;
            }

            return deleteWasSuccessful;
        }
        #endregion Problem Blob Methods

        #region Email Methods
        /// <summary>
        /// SendEmail(EmailElements emailElements)
        /// </summary>
        /// <param name="emailElements"></param>
        public void SendEmail(EmailElements emailElements)
        {
            try
            {
                EmailHelper.SendMessageWithAttachment(emailElements.FromAddress,
                                                      emailElements.ToAddresses,
                                                      emailElements.CCAddresses,
                                                      emailElements.BCCAddresses,
                                                      emailElements.SubjectText,
                                                      emailElements.BodyText,
                                                      ConfigurationManager.AppSettings["SMTPServer"].ToString(),
                                                      ConfigurationManager.AppSettings["SMTPUser"].ToString(),
                                                      ConfigurationManager.AppSettings["SMTPPassword"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in SendEmail().  Error: {0}\n{1}",
                                           ex.Message,
                                           ex.InnerException != null ? ex.InnerException.Message : string.Empty));
            }
        }
        #endregion Email Methods

        #region Search Methods
        /// <summary>
        /// List<Problem> SearchProblems(List<string> searchTerms)
        /// </summary>
        /// <param name="searchTerms"></param>
        /// <returns></returns>
        public List<Problem> SearchProblems(List<string> searchTerms)
        {
            List<Problem> foundProblems = new List<Problem>();

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    foreach (string searchTerm in searchTerms)
                    {
                        var problems = from p in svcPlsContext.Problems
                                       where p.ProblemShortDesc.Contains(searchTerm) ||
                                             p.ProblemText.Contains(searchTerm)
                                       orderby p.ProblemShortDesc
                                       select p;

                        if (problems != null)
                        {
                            foundProblems.AddRange(problems.ToList<Problem>());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in SearchProblems.  Exception: {0}", ex.Message));

                return null;
            }

            return foundProblems.OrderBy(fp => fp.ProblemShortDesc).ToList<Problem>();
        }

        /// <summary>
        /// List<Problem> ProblemKnowledgeSearch(KnowledgeSearch searchParms)
        /// </summary>
        /// <param name="searchParms"></param>
        /// <returns></returns>
        public List<Problem> ProblemKnowledgeSearch(KnowledgeSearch searchParms)
        {
            List<Problem> foundProblems = new List<Problem>();

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    // By Categories and No Help
                    if ((searchParms.CategoryIds != null &&
                         searchParms.CategoryIds.Count() > 0) &&
                        (searchParms.LocationIds == null ||
                         searchParms.LocationIds.Count() < 1) &&
                        searchParms.HelpDocOption == HelpDocOptions.NoHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid entityId in searchParms.CategoryIds)
                            {
                                var problems = from p in svcPlsContext.Problems
                                               where (p.ProblemShortDesc.Contains(searchTerm) ||
                                                     p.ProblemText.Contains(searchTerm)) &&
                                                     p.Ticket.CategoryId == entityId &&
                                                     p.Ticket.IsHelpDoc == false
                                               orderby p.ProblemShortDesc
                                               select p;

                                if (problems != null)
                                {
                                    foundProblems.AddRange(problems.ToList<Problem>());
                                }
                            }
                        }
                    }
                    // By Locations and No Help
                    else if ((searchParms.CategoryIds == null ||
                              searchParms.CategoryIds.Count() < 1) &&
                             (searchParms.LocationIds != null &&
                              searchParms.LocationIds.Count() > 0) &&
                             searchParms.HelpDocOption == HelpDocOptions.NoHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid entityId in searchParms.CategoryIds)
                            {
                                var problems = from p in svcPlsContext.Problems
                                               where (p.ProblemShortDesc.Contains(searchTerm) ||
                                                     p.ProblemText.Contains(searchTerm)) &&
                                                     p.Ticket.LocationId == entityId &&
                                                     p.Ticket.IsHelpDoc == false
                                               orderby p.ProblemShortDesc
                                               select p;

                                if (problems != null)
                                {
                                    foundProblems.AddRange(problems.ToList<Problem>());
                                }
                            }
                        }
                    }
                    // By Categories and Help
                    else if ((searchParms.CategoryIds != null &&
                         searchParms.CategoryIds.Count() > 0) &&
                        (searchParms.LocationIds == null ||
                         searchParms.LocationIds.Count() < 1) &&
                        searchParms.HelpDocOption == HelpDocOptions.JustHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid entityId in searchParms.CategoryIds)
                            {
                                var problems = from p in svcPlsContext.Problems
                                               where (p.ProblemShortDesc.Contains(searchTerm) ||
                                                     p.ProblemText.Contains(searchTerm)) &&
                                                     p.Ticket.CategoryId == entityId &&
                                                     p.Ticket.IsHelpDoc == true
                                               orderby p.ProblemShortDesc
                                               select p;

                                if (problems != null)
                                {
                                    foundProblems.AddRange(problems.ToList<Problem>());
                                }
                            }
                        }
                    }
                    // By Locations and Help
                    else if ((searchParms.CategoryIds == null ||
                              searchParms.CategoryIds.Count() < 1) &&
                             (searchParms.LocationIds != null &&
                              searchParms.LocationIds.Count() > 0) &&
                             searchParms.HelpDocOption == HelpDocOptions.JustHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid entityId in searchParms.CategoryIds)
                            {
                                var problems = from p in svcPlsContext.Problems
                                               where (p.ProblemShortDesc.Contains(searchTerm) ||
                                                     p.ProblemText.Contains(searchTerm)) &&
                                                     p.Ticket.LocationId == entityId &&
                                                     p.Ticket.IsHelpDoc == true
                                               orderby p.ProblemShortDesc
                                               select p;

                                if (problems != null)
                                {
                                    foundProblems.AddRange(problems.ToList<Problem>());
                                }
                            }
                        }
                    }
                    // By Categories and Combined Help
                    else if ((searchParms.CategoryIds != null &&
                         searchParms.CategoryIds.Count() > 0) &&
                        (searchParms.LocationIds == null ||
                         searchParms.LocationIds.Count() < 1) &&
                        searchParms.HelpDocOption == HelpDocOptions.Combined)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid entityId in searchParms.CategoryIds)
                            {
                                var problems = from p in svcPlsContext.Problems
                                               where (p.ProblemShortDesc.Contains(searchTerm) ||
                                                     p.ProblemText.Contains(searchTerm)) &&
                                                     p.Ticket.CategoryId == entityId
                                               orderby p.ProblemShortDesc
                                               select p;

                                if (problems != null)
                                {
                                    foundProblems.AddRange(problems.ToList<Problem>());
                                }
                            }
                        }
                    }
                    // By Locations and Combined Help
                    else if ((searchParms.CategoryIds == null ||
                              searchParms.CategoryIds.Count() < 1) &&
                             (searchParms.LocationIds != null &&
                              searchParms.LocationIds.Count() > 0) &&
                             searchParms.HelpDocOption == HelpDocOptions.Combined)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid entityId in searchParms.CategoryIds)
                            {
                                var problems = from p in svcPlsContext.Problems
                                               where (p.ProblemShortDesc.Contains(searchTerm) ||
                                                     p.ProblemText.Contains(searchTerm)) &&
                                                     p.Ticket.LocationId == entityId
                                               orderby p.ProblemShortDesc
                                               select p;

                                if (problems != null)
                                {
                                    foundProblems.AddRange(problems.ToList<Problem>());
                                }
                            }
                        }
                    }
                    // By Categories and Locations and Help
                    else if ((searchParms.CategoryIds != null &&
                              searchParms.CategoryIds.Count() > 0) &&
                             (searchParms.LocationIds != null &&
                              searchParms.LocationIds.Count() > 0) &&
                             searchParms.HelpDocOption == HelpDocOptions.JustHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid categoryId in searchParms.CategoryIds)
                            {
                                foreach (Guid locationId in searchParms.LocationIds)
                                {
                                    var problems = from p in svcPlsContext.Problems
                                                   where (p.ProblemShortDesc.Contains(searchTerm) ||
                                                         p.ProblemText.Contains(searchTerm)) &&
                                                         p.Ticket.CategoryId == categoryId &&
                                                         p.Ticket.LocationId == locationId &&
                                                         p.Ticket.IsHelpDoc == true
                                                   orderby p.ProblemShortDesc
                                                   select p;

                                    if (problems != null)
                                    {
                                        foundProblems.AddRange(problems.ToList<Problem>());
                                    }
                                }
                            }
                        }
                    }
                    // By Categories and Locations and No Help
                    else if ((searchParms.CategoryIds != null &&
                              searchParms.CategoryIds.Count() > 0) &&
                             (searchParms.LocationIds != null &&
                              searchParms.LocationIds.Count() > 0) &&
                             searchParms.HelpDocOption == HelpDocOptions.NoHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid categoryId in searchParms.CategoryIds)
                            {
                                foreach (Guid locationId in searchParms.LocationIds)
                                {
                                    var problems = from p in svcPlsContext.Problems
                                                   where (p.ProblemShortDesc.Contains(searchTerm) ||
                                                          p.ProblemText.Contains(searchTerm)) &&
                                                         p.Ticket.CategoryId == categoryId &&
                                                         p.Ticket.LocationId == locationId &&
                                                         p.Ticket.IsHelpDoc == false
                                                   orderby p.ProblemShortDesc
                                                   select p;

                                    if (problems != null)
                                    {
                                        foundProblems.AddRange(problems.ToList<Problem>());
                                    }
                                }
                            }
                        }
                    }
                    // By Categories and Locations and Combined Help
                    else if ((searchParms.CategoryIds != null &&
                              searchParms.CategoryIds.Count() > 0) &&
                             (searchParms.LocationIds != null &&
                              searchParms.LocationIds.Count() > 0) &&
                             searchParms.HelpDocOption == HelpDocOptions.Combined)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid categoryId in searchParms.CategoryIds)
                            {
                                foreach (Guid locationId in searchParms.LocationIds)
                                {
                                    var problems = from p in svcPlsContext.Problems
                                                   where (p.ProblemShortDesc.Contains(searchTerm) ||
                                                         p.ProblemText.Contains(searchTerm)) &&
                                                         p.Ticket.CategoryId == categoryId &&
                                                         p.Ticket.LocationId == locationId
                                                   orderby p.ProblemShortDesc
                                                   select p;

                                    if (problems != null)
                                    {
                                        foundProblems.AddRange(problems.ToList<Problem>());
                                    }
                                }
                            }
                        }
                    }
                    // By No Categories or Locations and No Help
                    else if ((searchParms.CategoryIds == null ||
                              searchParms.CategoryIds.Count() < 1) &&
                             (searchParms.LocationIds == null ||
                              searchParms.LocationIds.Count() < 1) &&
                             searchParms.HelpDocOption == HelpDocOptions.NoHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            var Problems = from p in svcPlsContext.Problems
                                           where (p.ProblemShortDesc.Contains(searchTerm) ||
                                                  p.ProblemText.Contains(searchTerm)) &&
                                                 p.Ticket.IsHelpDoc == false
                                           orderby p.ProblemShortDesc
                                           select p;

                            if (Problems != null)
                            {
                                foundProblems.AddRange(Problems.ToList<Problem>());
                            }
                        }
                    }
                    // By No Categories or Locations and Help
                    else if ((searchParms.CategoryIds == null ||
                              searchParms.CategoryIds.Count() < 1) &&
                             (searchParms.LocationIds == null ||
                              searchParms.LocationIds.Count() < 1) &&
                             searchParms.HelpDocOption == HelpDocOptions.JustHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            var Problems = from p in svcPlsContext.Problems
                                           where (p.ProblemShortDesc.Contains(searchTerm) ||
                                                  p.ProblemText.Contains(searchTerm)) &&
                                                 p.Ticket.IsHelpDoc == true
                                           orderby p.ProblemShortDesc
                                           select p;

                            if (Problems != null)
                            {
                                foundProblems.AddRange(Problems.ToList<Problem>());
                            }
                        }
                    }
                    // By No Categories or Locations and Combined Help
                    else if ((searchParms.CategoryIds == null ||
                              searchParms.CategoryIds.Count() < 1) &&
                             (searchParms.LocationIds == null ||
                              searchParms.LocationIds.Count() < 1) &&
                             searchParms.HelpDocOption == HelpDocOptions.Combined)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            var Problems = from p in svcPlsContext.Problems
                                           where (p.ProblemShortDesc.Contains(searchTerm) ||
                                                   p.ProblemText.Contains(searchTerm))
                                           orderby p.ProblemShortDesc
                                           select p;

                            if (Problems != null)
                            {
                                foundProblems.AddRange(Problems.ToList<Problem>());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in ProblemKnowledgeSearch(KnowledgeSearch searchParms).  Exception: {0}", ex.Message));

                return null;
            }

            return foundProblems.OrderBy(fs => fs.ProblemShortDesc).ToList<Problem>();
        }

        /// <summary>
        /// List<Solution> SearchSolutions(List<string> searchTerms)
        /// </summary>
        /// <param name="searchTerms"></param>
        /// <returns></returns>
        public List<Solution> SearchSolutions(List<string> searchTerms)
        {
            List<Solution> foundSolutions = new List<Solution>();

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    foreach (string searchTerm in searchTerms)
                    {
                        var solutions = from s in svcPlsContext.Solutions
                                        where s.SolutionShortDesc.Contains(searchTerm) ||
                                              s.SolutionText.Contains(searchTerm)
                                        orderby s.SolutionShortDesc
                                        select s;

                        if (solutions != null)
                        {
                            foundSolutions.AddRange(solutions.ToList<Solution>());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in SearchSolutions.  Exception: {0}", ex.Message));

                return null;
            }

            return foundSolutions.OrderBy(fs => fs.SolutionShortDesc).ToList<Solution>();
        }



        #region oldSolutionKnowledgeSearch
        //public List<SolutionWithHelpDoc> SolutionKnowledgeSearch(KnowledgeSearch searchParms)
        //{
        //    List<SolutionWithHelpDoc> foundSolutions = new List<SolutionWithHelpDoc>();

        //    try
        //    {
        //        using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
        //        {
        //            // By Categories and No Help
        //            if ((searchParms.CategoryIds != null &&
        //                 searchParms.CategoryIds.Count() > 0) &&
        //                (searchParms.LocationIds == null ||
        //                 searchParms.LocationIds.Count() < 1) &&
        //                searchParms.HelpDocOption == HelpDocOptions.NoHelpDocs)
        //            {
        //                foreach (string searchTerm in searchParms.SearchTerms)
        //                {
        //                    foreach (Guid entityId in searchParms.CategoryIds)
        //                    {
        //                        //Solution solnList = new Solution();
        //                        var Solutions = from p in svcPlsContext.Solutions
        //                                        where (p.SolutionShortDesc.Contains(searchTerm) ||
        //                                               p.SolutionText.Contains(searchTerm)) &&
        //                                              p.Ticket.CategoryId == entityId &&
        //                                              p.Ticket.IsHelpDoc == false
        //                                        orderby p.LikeCount descending
        //                                        select new
        //                                        {
        //                                            SolutionId = p.SolutionId,
        //                                            TicketId = p.TicketId,
        //                                            SolutionShortDesc = p.SolutionShortDesc,
        //                                            SolutionText = p.SolutionText,
        //                                            CreateDate = p.CreateDate,
        //                                            EditDate = p.EditDate,
        //                                            UserId = p.UserId,
        //                                            LikeCount = p.LikeCount,
        //                                            UnlikeCount = p.UnlikeCount,
        //                                            IsHelpDoc = p.Ticket.IsHelpDoc
        //                                        };

        //                        if (Solutions != null)
        //                        {
        //                            //foundSolutions.AddRange(Solutions.ToList<Solution>());
        //                            foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
        //                        }
        //                    }
        //                }
        //            }
        //            // By Locations and No Help
        //            else if ((searchParms.CategoryIds == null ||
        //                      searchParms.CategoryIds.Count() < 1) &&
        //                     (searchParms.LocationIds != null &&
        //                      searchParms.LocationIds.Count() > 0) &&
        //                     searchParms.HelpDocOption == HelpDocOptions.NoHelpDocs)
        //            {
        //                foreach (string searchTerm in searchParms.SearchTerms)
        //                {
        //                    foreach (Guid entityId in searchParms.CategoryIds)
        //                    {
        //                        var Solutions = from p in svcPlsContext.Solutions
        //                                        where (p.SolutionShortDesc.Contains(searchTerm) ||
        //                                              p.SolutionText.Contains(searchTerm)) &&
        //                                              p.Ticket.LocationId == entityId &&
        //                                              p.Ticket.IsHelpDoc == false
        //                                        orderby p.LikeCount descending
        //                                        select new
        //                                        {
        //                                            SolutionId = p.SolutionId,
        //                                            TicketId = p.TicketId,
        //                                            SolutionShortDesc = p.SolutionShortDesc,
        //                                            SolutionText = p.SolutionText,
        //                                            CreateDate = p.CreateDate,
        //                                            EditDate = p.EditDate,
        //                                            UserId = p.UserId,
        //                                            LikeCount = p.LikeCount,
        //                                            UnlikeCount = p.UnlikeCount,
        //                                            IsHelpDoc = p.Ticket.IsHelpDoc
        //                                        };

        //                        if (Solutions != null)
        //                        {
        //                            //foundSolutions.AddRange(Solutions.ToList<Solution>());
        //                            foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
        //                        }
        //                    }
        //                }
        //            }
        //            // By Categories and Help
        //            else if ((searchParms.CategoryIds != null &&
        //                 searchParms.CategoryIds.Count() > 0) &&
        //                (searchParms.LocationIds == null ||
        //                 searchParms.LocationIds.Count() < 1) &&
        //                searchParms.HelpDocOption == HelpDocOptions.JustHelpDocs)
        //            {
        //                foreach (string searchTerm in searchParms.SearchTerms)
        //                {
        //                    foreach (Guid entityId in searchParms.CategoryIds)
        //                    {
        //                        var Solutions = from p in svcPlsContext.Solutions
        //                                        where (p.SolutionShortDesc.Contains(searchTerm) ||
        //                                              p.SolutionText.Contains(searchTerm)) &&
        //                                              p.Ticket.CategoryId == entityId &&
        //                                              p.Ticket.IsHelpDoc == true
        //                                        orderby p.LikeCount descending
        //                                        select new
        //                                        {
        //                                            SolutionId = p.SolutionId,
        //                                            TicketId = p.TicketId,
        //                                            SolutionShortDesc = p.SolutionShortDesc,
        //                                            SolutionText = p.SolutionText,
        //                                            CreateDate = p.CreateDate,
        //                                            EditDate = p.EditDate,
        //                                            UserId = p.UserId,
        //                                            LikeCount = p.LikeCount,
        //                                            UnlikeCount = p.UnlikeCount,
        //                                            IsHelpDoc = p.Ticket.IsHelpDoc
        //                                        };

        //                        if (Solutions != null)
        //                        {
        //                            //foundSolutions.AddRange(Solutions.ToList<Solution>());
        //                            foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
        //                        }
        //                    }
        //                }
        //            }
        //            // By Locations and Help
        //            else if ((searchParms.CategoryIds == null ||
        //                      searchParms.CategoryIds.Count() < 1) &&
        //                     (searchParms.LocationIds != null &&
        //                      searchParms.LocationIds.Count() > 0) &&
        //                     searchParms.HelpDocOption == HelpDocOptions.JustHelpDocs)
        //            {
        //                foreach (string searchTerm in searchParms.SearchTerms)
        //                {
        //                    foreach (Guid entityId in searchParms.CategoryIds)
        //                    {
        //                        var Solutions = from p in svcPlsContext.Solutions
        //                                        where (p.SolutionShortDesc.Contains(searchTerm) ||
        //                                              p.SolutionText.Contains(searchTerm)) &&
        //                                              p.Ticket.LocationId == entityId &&
        //                                              p.Ticket.IsHelpDoc == true
        //                                        orderby p.LikeCount descending
        //                                        select new
        //                                        {
        //                                            SolutionId = p.SolutionId,
        //                                            TicketId = p.TicketId,
        //                                            SolutionShortDesc = p.SolutionShortDesc,
        //                                            SolutionText = p.SolutionText,
        //                                            CreateDate = p.CreateDate,
        //                                            EditDate = p.EditDate,
        //                                            UserId = p.UserId,
        //                                            LikeCount = p.LikeCount,
        //                                            UnlikeCount = p.UnlikeCount,
        //                                            IsHelpDoc = p.Ticket.IsHelpDoc
        //                                        };

        //                        if (Solutions != null)
        //                        {
        //                            //foundSolutions.AddRange(Solutions.ToList<Solution>());
        //                            foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
        //                        }
        //                    }
        //                }
        //            }
        //            // By Categories and Combined Help
        //            else if ((searchParms.CategoryIds != null &&
        //                 searchParms.CategoryIds.Count() > 0) &&
        //                (searchParms.LocationIds == null ||
        //                 searchParms.LocationIds.Count() < 1) &&
        //                searchParms.HelpDocOption == HelpDocOptions.Combined)
        //            {
        //                foreach (string searchTerm in searchParms.SearchTerms)
        //                {
        //                    foreach (Guid entityId in searchParms.CategoryIds)
        //                    {
        //                        var Solutions = from p in svcPlsContext.Solutions
        //                                        where (p.SolutionShortDesc.Contains(searchTerm) ||
        //                                              p.SolutionText.Contains(searchTerm)) &&
        //                                              p.Ticket.CategoryId == entityId
        //                                        orderby p.LikeCount descending
        //                                        select new
        //                                        {
        //                                            SolutionId = p.SolutionId,
        //                                            TicketId = p.TicketId,
        //                                            SolutionShortDesc = p.SolutionShortDesc,
        //                                            SolutionText = p.SolutionText,
        //                                            CreateDate = p.CreateDate,
        //                                            EditDate = p.EditDate,
        //                                            UserId = p.UserId,
        //                                            LikeCount = p.LikeCount,
        //                                            UnlikeCount = p.UnlikeCount,
        //                                            IsHelpDoc = p.Ticket.IsHelpDoc
        //                                        };

        //                        if (Solutions != null)
        //                        {
        //                            //foundSolutions.AddRange(Solutions.ToList<Solution>());
        //                            foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
        //                        }
        //                    }
        //                }
        //            }
        //            // By Locations and Combined Help
        //            else if ((searchParms.CategoryIds == null ||
        //                      searchParms.CategoryIds.Count() < 1) &&
        //                     (searchParms.LocationIds != null &&
        //                      searchParms.LocationIds.Count() > 0) &&
        //                     searchParms.HelpDocOption == HelpDocOptions.Combined)
        //            {
        //                foreach (string searchTerm in searchParms.SearchTerms)
        //                {
        //                    foreach (Guid entityId in searchParms.CategoryIds)
        //                    {
        //                        var Solutions = from p in svcPlsContext.Solutions
        //                                        where (p.SolutionShortDesc.Contains(searchTerm) ||
        //                                              p.SolutionText.Contains(searchTerm)) &&
        //                                              p.Ticket.LocationId == entityId
        //                                        orderby p.LikeCount descending
        //                                        select new
        //                                        {
        //                                            SolutionId = p.SolutionId,
        //                                            TicketId = p.TicketId,
        //                                            SolutionShortDesc = p.SolutionShortDesc,
        //                                            SolutionText = p.SolutionText,
        //                                            CreateDate = p.CreateDate,
        //                                            EditDate = p.EditDate,
        //                                            UserId = p.UserId,
        //                                            LikeCount = p.LikeCount,
        //                                            UnlikeCount = p.UnlikeCount,
        //                                            IsHelpDoc = p.Ticket.IsHelpDoc
        //                                        };

        //                        if (Solutions != null)
        //                        {
        //                            //foundSolutions.AddRange(Solutions.ToList<Solution>());
        //                            foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
        //                        }
        //                    }
        //                }
        //            }
        //            // By Categories and Locations and Help
        //            else if ((searchParms.CategoryIds != null &&
        //                      searchParms.CategoryIds.Count() > 0) &&
        //                     (searchParms.LocationIds != null &&
        //                      searchParms.LocationIds.Count() > 0) &&
        //                     searchParms.HelpDocOption == HelpDocOptions.JustHelpDocs)
        //            {
        //                foreach (string searchTerm in searchParms.SearchTerms)
        //                {
        //                    foreach (Guid categoryId in searchParms.CategoryIds)
        //                    {
        //                        foreach (Guid locationId in searchParms.LocationIds)
        //                        {
        //                            var Solutions = from p in svcPlsContext.Solutions
        //                                            where (p.SolutionShortDesc.Contains(searchTerm) ||
        //                                                  p.SolutionText.Contains(searchTerm)) &&
        //                                                  p.Ticket.CategoryId == categoryId &&
        //                                                  p.Ticket.LocationId == locationId &&
        //                                                  p.Ticket.IsHelpDoc == true
        //                                            orderby p.LikeCount descending
        //                                            select new
        //                                            {
        //                                                SolutionId = p.SolutionId,
        //                                                TicketId = p.TicketId,
        //                                                SolutionShortDesc = p.SolutionShortDesc,
        //                                                SolutionText = p.SolutionText,
        //                                                CreateDate = p.CreateDate,
        //                                                EditDate = p.EditDate,
        //                                                UserId = p.UserId,
        //                                                LikeCount = p.LikeCount,
        //                                                UnlikeCount = p.UnlikeCount,
        //                                                IsHelpDoc = p.Ticket.IsHelpDoc
        //                                            };

        //                            if (Solutions != null)
        //                            {
        //                                //foundSolutions.AddRange(Solutions.ToList<Solution>());
        //                                foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            // By Categories and Locations and No Help
        //            else if ((searchParms.CategoryIds != null &&
        //                      searchParms.CategoryIds.Count() > 0) &&
        //                     (searchParms.LocationIds != null &&
        //                      searchParms.LocationIds.Count() > 0) &&
        //                     searchParms.HelpDocOption == HelpDocOptions.NoHelpDocs)
        //            {
        //                foreach (string searchTerm in searchParms.SearchTerms)
        //                {
        //                    foreach (Guid categoryId in searchParms.CategoryIds)
        //                    {
        //                        foreach (Guid locationId in searchParms.LocationIds)
        //                        {

        //                            var Solutions = from p in svcPlsContext.Solutions
        //                                            where (p.SolutionShortDesc.Contains(searchTerm) ||
        //                                                   p.SolutionText.Contains(searchTerm)) &&
        //                                                  p.Ticket.CategoryId == categoryId &&
        //                                                  p.Ticket.LocationId == locationId &&
        //                                                  p.Ticket.IsHelpDoc == false
        //                                            orderby p.LikeCount descending
        //                                            select new
        //                                            {
        //                                                SolutionId = p.SolutionId,
        //                                                TicketId = p.TicketId,
        //                                                SolutionShortDesc = p.SolutionShortDesc,
        //                                                SolutionText = p.SolutionText,
        //                                                CreateDate = p.CreateDate,
        //                                                EditDate = p.EditDate,
        //                                                UserId = p.UserId,
        //                                                LikeCount = p.LikeCount,
        //                                                UnlikeCount = p.UnlikeCount,
        //                                                IsHelpDoc = p.Ticket.IsHelpDoc
        //                                            };

        //                            if (Solutions != null)
        //                            {
        //                                //foundSolutions.AddRange(Solutions.ToList<Solution>());
        //                                foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            // By Categories and Locations and Combined Help
        //            else if ((searchParms.CategoryIds != null &&
        //                      searchParms.CategoryIds.Count() > 0) &&
        //                     (searchParms.LocationIds != null &&
        //                      searchParms.LocationIds.Count() > 0) &&
        //                     searchParms.HelpDocOption == HelpDocOptions.Combined)
        //            {
        //                foreach (string searchTerm in searchParms.SearchTerms)
        //                {
        //                    foreach (Guid categoryId in searchParms.CategoryIds)
        //                    {
        //                        foreach (Guid locationId in searchParms.LocationIds)
        //                        {
        //                            var Solutions = from p in svcPlsContext.Solutions
        //                                            where (p.SolutionShortDesc.Contains(searchTerm) ||
        //                                                  p.SolutionText.Contains(searchTerm)) &&
        //                                                  p.Ticket.CategoryId == categoryId &&
        //                                                  p.Ticket.LocationId == locationId
        //                                            orderby p.LikeCount descending
        //                                            select new
        //                                            {
        //                                                SolutionId = p.SolutionId,
        //                                                TicketId = p.TicketId,
        //                                                SolutionShortDesc = p.SolutionShortDesc,
        //                                                SolutionText = p.SolutionText,
        //                                                CreateDate = p.CreateDate,
        //                                                EditDate = p.EditDate,
        //                                                UserId = p.UserId,
        //                                                LikeCount = p.LikeCount,
        //                                                UnlikeCount = p.UnlikeCount,
        //                                                IsHelpDoc = p.Ticket.IsHelpDoc
        //                                            };

        //                            if (Solutions != null)
        //                            {
        //                                //foundSolutions.AddRange(Solutions.ToList<Solution>());
        //                                foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            // By No Categories or Locations and No Help
        //            else if ((searchParms.CategoryIds == null ||
        //                      searchParms.CategoryIds.Count() < 1) &&
        //                     (searchParms.LocationIds == null ||
        //                      searchParms.LocationIds.Count() < 1) &&
        //                     searchParms.HelpDocOption == HelpDocOptions.NoHelpDocs)
        //            {
        //                foreach (string searchTerm in searchParms.SearchTerms)
        //                {
        //                    var Solutions = from p in svcPlsContext.Solutions
        //                                    where (p.SolutionShortDesc.Contains(searchTerm) ||
        //                                           p.SolutionText.Contains(searchTerm)) &&
        //                                          p.Ticket.IsHelpDoc == false
        //                                    orderby p.LikeCount descending
        //                                    select new
        //                                    {
        //                                        SolutionId = p.SolutionId,
        //                                        TicketId = p.TicketId,
        //                                        SolutionShortDesc = p.SolutionShortDesc,
        //                                        SolutionText = p.SolutionText,
        //                                        CreateDate = p.CreateDate,
        //                                        EditDate = p.EditDate,
        //                                        UserId = p.UserId,
        //                                        LikeCount = p.LikeCount,
        //                                        UnlikeCount = p.UnlikeCount,
        //                                        IsHelpDoc = p.Ticket.IsHelpDoc
        //                                    };

        //                    if (Solutions != null)
        //                    {
        //                        //foundSolutions.AddRange(Solutions.ToList<Solution>());
        //                        foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
        //                    }
        //                }
        //            }
        //            // By No Categories or Locations and Help
        //            else if ((searchParms.CategoryIds == null ||
        //                      searchParms.CategoryIds.Count() < 1) &&
        //                     (searchParms.LocationIds == null ||
        //                      searchParms.LocationIds.Count() < 1) &&
        //                     searchParms.HelpDocOption == HelpDocOptions.JustHelpDocs)
        //            {
        //                foreach (string searchTerm in searchParms.SearchTerms)
        //                {
        //                    var Solutions = from p in svcPlsContext.Solutions
        //                                    where (p.SolutionShortDesc.Contains(searchTerm) ||
        //                                           p.SolutionText.Contains(searchTerm)) &&
        //                                          p.Ticket.IsHelpDoc == true
        //                                    orderby p.LikeCount descending
        //                                    select new
        //                                    {
        //                                        SolutionId = p.SolutionId,
        //                                        TicketId = p.TicketId,
        //                                        SolutionShortDesc = p.SolutionShortDesc,
        //                                        SolutionText = p.SolutionText,
        //                                        CreateDate = p.CreateDate,
        //                                        EditDate = p.EditDate,
        //                                        UserId = p.UserId,
        //                                        LikeCount = p.LikeCount,
        //                                        UnlikeCount = p.UnlikeCount,
        //                                        IsHelpDoc = p.Ticket.IsHelpDoc
        //                                    };

        //                    if (Solutions != null)
        //                    {
        //                        //foundSolutions.AddRange(Solutions.ToList<Solution>());
        //                        foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
        //                    }
        //                }
        //            }
        //            // By No Categories or Locations and Combined Help
        //            else if ((searchParms.CategoryIds == null ||
        //                      searchParms.CategoryIds.Count() < 1) &&
        //                     (searchParms.LocationIds == null ||
        //                      searchParms.LocationIds.Count() < 1) &&
        //                     searchParms.HelpDocOption == HelpDocOptions.Combined)
        //            {
        //                foreach (string searchTerm in searchParms.SearchTerms)
        //                {
        //                    var Solutions = from p in svcPlsContext.Solutions
        //                                    where (p.SolutionShortDesc.Contains(searchTerm) ||
        //                                            p.SolutionText.Contains(searchTerm))
        //                                    orderby p.LikeCount descending
        //                                    select new
        //                                    {
        //                                        SolutionId = p.SolutionId,
        //                                        TicketId = p.TicketId,
        //                                        SolutionShortDesc = p.SolutionShortDesc,
        //                                        SolutionText = p.SolutionText,
        //                                        CreateDate = p.CreateDate,
        //                                        EditDate = p.EditDate,
        //                                        UserId = p.UserId,
        //                                        LikeCount = p.LikeCount,
        //                                        UnlikeCount = p.UnlikeCount,
        //                                        IsHelpDoc = p.Ticket.IsHelpDoc
        //                                    };

        //                    if (Solutions != null)
        //                    {
        //                        //foundSolutions.AddRange(Solutions.ToList<Solution>());
        //                        foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Write(string.Format("Exception in SolutionKnowledgeSearch(KnowledgeSearch searchParms).  Exception: {0}", ex.Message));

        //        return null;
        //    }

        //    return foundSolutions.OrderBy(fs => fs.UnlikeCount - fs.LikeCount).ThenBy(fsc => fsc.LikeCount).ToList<SolutionWithHelpDoc>(); ;
        //}
        #endregion

        public List<SolutionWithHelpDoc> SolutionKnowledgeSearch(KnowledgeSearch searchParms)
        {
            List<SolutionWithHelpDoc> foundSolutions = new List<SolutionWithHelpDoc>();
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //By All Categories & All Locations No Help
                    if (searchParms.IsCategoryAll.ToUpper() == "ALL" &&
                        searchParms.IsLocationAll.ToUpper() == "ALL" &&
                        searchParms.HelpDocOption == HelpDocOptions.NoHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            //Solution solnList = new Solution();
                            var Solutions = from p in svcPlsContext.Solutions
                                            where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                   p.SolutionText.Contains(searchTerm)) &&
                                                  p.Ticket.IsHelpDoc == false
                                            orderby p.LikeCount descending
                                            select new
                                            {
                                                SolutionId = p.SolutionId,
                                                TicketId = p.TicketId,
                                                SolutionShortDesc = p.SolutionShortDesc,
                                                SolutionText = p.SolutionText,
                                                CreateDate = p.CreateDate,
                                                EditDate = p.EditDate,
                                                UserId = p.UserId,
                                                LikeCount = p.LikeCount,
                                                UnlikeCount = p.UnlikeCount,
                                                IsHelpDoc = p.Ticket.IsHelpDoc
                                            };

                            if (Solutions != null)
                            {
                                //foundSolutions.AddRange(Solutions.ToList<Solution>());
                                foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                            }
                        }

                    }
                    //By All Categories & All Locations Help
                    else if (searchParms.IsCategoryAll.ToUpper() == "ALL" &&
                        searchParms.IsLocationAll.ToUpper() == "ALL" &&
                        searchParms.HelpDocOption == HelpDocOptions.JustHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            var Solutions = from p in svcPlsContext.Solutions
                                            where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                  p.SolutionText.Contains(searchTerm)) &&
                                                  p.Ticket.IsHelpDoc == true
                                            orderby p.LikeCount descending
                                            select new
                                            {
                                                SolutionId = p.SolutionId,
                                                TicketId = p.TicketId,
                                                SolutionShortDesc = p.SolutionShortDesc,
                                                SolutionText = p.SolutionText,
                                                CreateDate = p.CreateDate,
                                                EditDate = p.EditDate,
                                                UserId = p.UserId,
                                                LikeCount = p.LikeCount,
                                                UnlikeCount = p.UnlikeCount,
                                                IsHelpDoc = p.Ticket.IsHelpDoc
                                            };

                            if (Solutions != null)
                            {
                                //foundSolutions.AddRange(Solutions.ToList<Solution>());
                                foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                            }
                        }

                    }
                    //By All Categories & All Locations Combined Help
                    else if (searchParms.IsCategoryAll.ToUpper() == "ALL" &&
                        searchParms.IsLocationAll.ToUpper() == "ALL" &&
                        searchParms.HelpDocOption == HelpDocOptions.Combined)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            var Solutions = from p in svcPlsContext.Solutions
                                            where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                  p.SolutionText.Contains(searchTerm))
                                            orderby p.LikeCount descending
                                            select new
                                            {
                                                SolutionId = p.SolutionId,
                                                TicketId = p.TicketId,
                                                SolutionShortDesc = p.SolutionShortDesc,
                                                SolutionText = p.SolutionText,
                                                CreateDate = p.CreateDate,
                                                EditDate = p.EditDate,
                                                UserId = p.UserId,
                                                LikeCount = p.LikeCount,
                                                UnlikeCount = p.UnlikeCount,
                                                IsHelpDoc = p.Ticket.IsHelpDoc
                                            };

                            if (Solutions != null)
                            {
                                //foundSolutions.AddRange(Solutions.ToList<Solution>());
                                foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                            }
                        }

                    }
                    //By All Categories & Locations & No Help
                    else if (searchParms.IsCategoryAll.ToUpper() == "ALL" &&
                        (searchParms.LocationIds != null &&
                         searchParms.LocationIds.Count() > 0) &&
                    searchParms.HelpDocOption == HelpDocOptions.NoHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid locationId in searchParms.LocationIds)
                            {
                                var Solutions = from p in svcPlsContext.Solutions
                                                where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                      p.SolutionText.Contains(searchTerm)) &&
                                                      p.Ticket.LocationId == locationId &&
                                                      p.Ticket.IsHelpDoc == false
                                                orderby p.LikeCount descending
                                                select new
                                                {
                                                    SolutionId = p.SolutionId,
                                                    TicketId = p.TicketId,
                                                    SolutionShortDesc = p.SolutionShortDesc,
                                                    SolutionText = p.SolutionText,
                                                    CreateDate = p.CreateDate,
                                                    EditDate = p.EditDate,
                                                    UserId = p.UserId,
                                                    LikeCount = p.LikeCount,
                                                    UnlikeCount = p.UnlikeCount,
                                                    IsHelpDoc = p.Ticket.IsHelpDoc
                                                };

                                if (Solutions != null)
                                {
                                    foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                                }
                            }

                        }
                    }
                    //By All Categories & Locations & Help
                    else if (searchParms.IsCategoryAll.ToUpper() == "ALL" &&
                        (searchParms.LocationIds != null &&
                         searchParms.LocationIds.Count() > 0) &&
                    searchParms.HelpDocOption == HelpDocOptions.JustHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid locationId in searchParms.LocationIds)
                            {
                                var Solutions = from p in svcPlsContext.Solutions
                                                where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                      p.SolutionText.Contains(searchTerm)) &&
                                                      p.Ticket.LocationId == locationId &&
                                                      p.Ticket.IsHelpDoc == true
                                                orderby p.LikeCount descending
                                                select new
                                                {
                                                    SolutionId = p.SolutionId,
                                                    TicketId = p.TicketId,
                                                    SolutionShortDesc = p.SolutionShortDesc,
                                                    SolutionText = p.SolutionText,
                                                    CreateDate = p.CreateDate,
                                                    EditDate = p.EditDate,
                                                    UserId = p.UserId,
                                                    LikeCount = p.LikeCount,
                                                    UnlikeCount = p.UnlikeCount,
                                                    IsHelpDoc = p.Ticket.IsHelpDoc
                                                };

                                if (Solutions != null)
                                {
                                    foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                                }
                            }
                        }
                    }
                    //By All Categories & Locations & Combined Help
                    else if (searchParms.IsCategoryAll.ToUpper() == "ALL" &&
                    (searchParms.LocationIds != null &&
                     searchParms.LocationIds.Count() > 0) &&
                    searchParms.HelpDocOption == HelpDocOptions.Combined)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid locationId in searchParms.LocationIds)
                            {
                                var Solutions = from p in svcPlsContext.Solutions
                                                where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                      p.SolutionText.Contains(searchTerm)) &&
                                                      p.Ticket.LocationId == locationId
                                                orderby p.LikeCount descending
                                                select new
                                                {
                                                    SolutionId = p.SolutionId,
                                                    TicketId = p.TicketId,
                                                    SolutionShortDesc = p.SolutionShortDesc,
                                                    SolutionText = p.SolutionText,
                                                    CreateDate = p.CreateDate,
                                                    EditDate = p.EditDate,
                                                    UserId = p.UserId,
                                                    LikeCount = p.LikeCount,
                                                    UnlikeCount = p.UnlikeCount,
                                                    IsHelpDoc = p.Ticket.IsHelpDoc
                                                };

                                if (Solutions != null)
                                {
                                    foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                                }
                            }
                        }
                    }

                    //By All Locations & Categories & No Help
                    else if (searchParms.IsLocationAll.ToUpper() == "ALL" &&
                        (searchParms.CategoryIds != null &&
                         searchParms.CategoryIds.Count() > 0) &&
                    searchParms.HelpDocOption == HelpDocOptions.NoHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid categoryId in searchParms.CategoryIds)
                            {
                                var Solutions = from p in svcPlsContext.Solutions
                                                where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                      p.SolutionText.Contains(searchTerm)) &&
                                                      p.Ticket.CategoryId == categoryId &&
                                                      p.Ticket.IsHelpDoc == false
                                                orderby p.LikeCount descending
                                                select new
                                                {
                                                    SolutionId = p.SolutionId,
                                                    TicketId = p.TicketId,
                                                    SolutionShortDesc = p.SolutionShortDesc,
                                                    SolutionText = p.SolutionText,
                                                    CreateDate = p.CreateDate,
                                                    EditDate = p.EditDate,
                                                    UserId = p.UserId,
                                                    LikeCount = p.LikeCount,
                                                    UnlikeCount = p.UnlikeCount,
                                                    IsHelpDoc = p.Ticket.IsHelpDoc
                                                };

                                if (Solutions != null)
                                {
                                    foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                                }
                            }

                        }
                    }
                    //By All Locations & Categories & Help
                    else if (searchParms.IsLocationAll.ToUpper() == "ALL" &&
                        (searchParms.CategoryIds != null &&
                         searchParms.CategoryIds.Count() > 0) &&
                    searchParms.HelpDocOption == HelpDocOptions.JustHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid categoryId in searchParms.CategoryIds)
                            {
                                var Solutions = from p in svcPlsContext.Solutions
                                                where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                      p.SolutionText.Contains(searchTerm)) &&
                                                      p.Ticket.CategoryId == categoryId &&
                                                      p.Ticket.IsHelpDoc == true
                                                orderby p.LikeCount descending
                                                select new
                                                {
                                                    SolutionId = p.SolutionId,
                                                    TicketId = p.TicketId,
                                                    SolutionShortDesc = p.SolutionShortDesc,
                                                    SolutionText = p.SolutionText,
                                                    CreateDate = p.CreateDate,
                                                    EditDate = p.EditDate,
                                                    UserId = p.UserId,
                                                    LikeCount = p.LikeCount,
                                                    UnlikeCount = p.UnlikeCount,
                                                    IsHelpDoc = p.Ticket.IsHelpDoc
                                                };

                                if (Solutions != null)
                                {
                                    foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                                }
                            }
                        }
                    }
                    //By All Locations & Categories & Combined Help
                    else if (searchParms.IsLocationAll.ToUpper() == "ALL" &&
                        (searchParms.CategoryIds != null &&
                         searchParms.CategoryIds.Count() > 0) &&
                    searchParms.HelpDocOption == HelpDocOptions.Combined)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid categoryId in searchParms.CategoryIds)
                            {
                                var Solutions = from p in svcPlsContext.Solutions
                                                where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                      p.SolutionText.Contains(searchTerm)) &&
                                                      p.Ticket.CategoryId == categoryId
                                                orderby p.LikeCount descending
                                                select new
                                                {
                                                    SolutionId = p.SolutionId,
                                                    TicketId = p.TicketId,
                                                    SolutionShortDesc = p.SolutionShortDesc,
                                                    SolutionText = p.SolutionText,
                                                    CreateDate = p.CreateDate,
                                                    EditDate = p.EditDate,
                                                    UserId = p.UserId,
                                                    LikeCount = p.LikeCount,
                                                    UnlikeCount = p.UnlikeCount,
                                                    IsHelpDoc = p.Ticket.IsHelpDoc
                                                };

                                if (Solutions != null)
                                {
                                    foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                                }
                            }
                        }
                    }

                    // By Categories and No Help
                    else if ((searchParms.CategoryIds != null &&
                         searchParms.CategoryIds.Count() > 0) &&
                        (searchParms.LocationIds == null ||
                         searchParms.LocationIds.Count() < 1) &&
                        searchParms.HelpDocOption == HelpDocOptions.NoHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid entityId in searchParms.CategoryIds)
                            {
                                //Solution solnList = new Solution();
                                var Solutions = from p in svcPlsContext.Solutions
                                                where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                       p.SolutionText.Contains(searchTerm)) &&
                                                      p.Ticket.CategoryId == entityId &&
                                                      p.Ticket.IsHelpDoc == false
                                                orderby p.LikeCount descending
                                                select new
                                                {
                                                    SolutionId = p.SolutionId,
                                                    TicketId = p.TicketId,
                                                    SolutionShortDesc = p.SolutionShortDesc,
                                                    SolutionText = p.SolutionText,
                                                    CreateDate = p.CreateDate,
                                                    EditDate = p.EditDate,
                                                    UserId = p.UserId,
                                                    LikeCount = p.LikeCount,
                                                    UnlikeCount = p.UnlikeCount,
                                                    IsHelpDoc = p.Ticket.IsHelpDoc
                                                };

                                if (Solutions != null)
                                {
                                    //foundSolutions.AddRange(Solutions.ToList<Solution>());
                                    foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                                }
                            }
                        }
                    }
                    // By Locations and No Help
                    else if ((searchParms.CategoryIds == null ||
                              searchParms.CategoryIds.Count() < 1) &&
                             (searchParms.LocationIds != null &&
                              searchParms.LocationIds.Count() > 0) &&
                             searchParms.HelpDocOption == HelpDocOptions.NoHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid entityId in searchParms.CategoryIds)
                            {
                                var Solutions = from p in svcPlsContext.Solutions
                                                where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                      p.SolutionText.Contains(searchTerm)) &&
                                                      p.Ticket.LocationId == entityId &&
                                                      p.Ticket.IsHelpDoc == false
                                                orderby p.LikeCount descending
                                                select new
                                                {
                                                    SolutionId = p.SolutionId,
                                                    TicketId = p.TicketId,
                                                    SolutionShortDesc = p.SolutionShortDesc,
                                                    SolutionText = p.SolutionText,
                                                    CreateDate = p.CreateDate,
                                                    EditDate = p.EditDate,
                                                    UserId = p.UserId,
                                                    LikeCount = p.LikeCount,
                                                    UnlikeCount = p.UnlikeCount,
                                                    IsHelpDoc = p.Ticket.IsHelpDoc
                                                };

                                if (Solutions != null)
                                {
                                    //foundSolutions.AddRange(Solutions.ToList<Solution>());
                                    foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                                }
                            }
                        }
                    }
                    // By Categories and Help
                    else if ((searchParms.CategoryIds != null &&
                         searchParms.CategoryIds.Count() > 0) &&
                        (searchParms.LocationIds == null ||
                         searchParms.LocationIds.Count() < 1) &&
                        searchParms.HelpDocOption == HelpDocOptions.JustHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid entityId in searchParms.CategoryIds)
                            {
                                var Solutions = from p in svcPlsContext.Solutions
                                                where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                      p.SolutionText.Contains(searchTerm)) &&
                                                      p.Ticket.CategoryId == entityId &&
                                                      p.Ticket.IsHelpDoc == true
                                                orderby p.LikeCount descending
                                                select new
                                                {
                                                    SolutionId = p.SolutionId,
                                                    TicketId = p.TicketId,
                                                    SolutionShortDesc = p.SolutionShortDesc,
                                                    SolutionText = p.SolutionText,
                                                    CreateDate = p.CreateDate,
                                                    EditDate = p.EditDate,
                                                    UserId = p.UserId,
                                                    LikeCount = p.LikeCount,
                                                    UnlikeCount = p.UnlikeCount,
                                                    IsHelpDoc = p.Ticket.IsHelpDoc
                                                };

                                if (Solutions != null)
                                {
                                    //foundSolutions.AddRange(Solutions.ToList<Solution>());
                                    foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                                }
                            }
                        }
                    }
                    // By Locations and Help
                    else if ((searchParms.CategoryIds == null ||
                              searchParms.CategoryIds.Count() < 1) &&
                             (searchParms.LocationIds != null &&
                              searchParms.LocationIds.Count() > 0) &&
                             searchParms.HelpDocOption == HelpDocOptions.JustHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid entityId in searchParms.CategoryIds)
                            {
                                var Solutions = from p in svcPlsContext.Solutions
                                                where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                      p.SolutionText.Contains(searchTerm)) &&
                                                      p.Ticket.LocationId == entityId &&
                                                      p.Ticket.IsHelpDoc == true
                                                orderby p.LikeCount descending
                                                select new
                                                {
                                                    SolutionId = p.SolutionId,
                                                    TicketId = p.TicketId,
                                                    SolutionShortDesc = p.SolutionShortDesc,
                                                    SolutionText = p.SolutionText,
                                                    CreateDate = p.CreateDate,
                                                    EditDate = p.EditDate,
                                                    UserId = p.UserId,
                                                    LikeCount = p.LikeCount,
                                                    UnlikeCount = p.UnlikeCount,
                                                    IsHelpDoc = p.Ticket.IsHelpDoc
                                                };

                                if (Solutions != null)
                                {
                                    //foundSolutions.AddRange(Solutions.ToList<Solution>());
                                    foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                                }
                            }
                        }
                    }
                    // By Categories and Combined Help
                    else if ((searchParms.CategoryIds != null &&
                         searchParms.CategoryIds.Count() > 0) &&
                        (searchParms.LocationIds == null ||
                         searchParms.LocationIds.Count() < 1) &&
                        searchParms.HelpDocOption == HelpDocOptions.Combined)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid entityId in searchParms.CategoryIds)
                            {
                                var Solutions = from p in svcPlsContext.Solutions
                                                where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                      p.SolutionText.Contains(searchTerm)) &&
                                                      p.Ticket.CategoryId == entityId
                                                orderby p.LikeCount descending
                                                select new
                                                {
                                                    SolutionId = p.SolutionId,
                                                    TicketId = p.TicketId,
                                                    SolutionShortDesc = p.SolutionShortDesc,
                                                    SolutionText = p.SolutionText,
                                                    CreateDate = p.CreateDate,
                                                    EditDate = p.EditDate,
                                                    UserId = p.UserId,
                                                    LikeCount = p.LikeCount,
                                                    UnlikeCount = p.UnlikeCount,
                                                    IsHelpDoc = p.Ticket.IsHelpDoc
                                                };

                                if (Solutions != null)
                                {
                                    //foundSolutions.AddRange(Solutions.ToList<Solution>());
                                    foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                                }
                            }
                        }
                    }
                    // By Locations and Combined Help
                    else if ((searchParms.CategoryIds == null ||
                              searchParms.CategoryIds.Count() < 1) &&
                             (searchParms.LocationIds != null &&
                              searchParms.LocationIds.Count() > 0) &&
                             searchParms.HelpDocOption == HelpDocOptions.Combined)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid entityId in searchParms.CategoryIds)
                            {
                                var Solutions = from p in svcPlsContext.Solutions
                                                where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                      p.SolutionText.Contains(searchTerm)) &&
                                                      p.Ticket.LocationId == entityId
                                                orderby p.LikeCount descending
                                                select new
                                                {
                                                    SolutionId = p.SolutionId,
                                                    TicketId = p.TicketId,
                                                    SolutionShortDesc = p.SolutionShortDesc,
                                                    SolutionText = p.SolutionText,
                                                    CreateDate = p.CreateDate,
                                                    EditDate = p.EditDate,
                                                    UserId = p.UserId,
                                                    LikeCount = p.LikeCount,
                                                    UnlikeCount = p.UnlikeCount,
                                                    IsHelpDoc = p.Ticket.IsHelpDoc
                                                };

                                if (Solutions != null)
                                {
                                    //foundSolutions.AddRange(Solutions.ToList<Solution>());
                                    foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                                }
                            }
                        }
                    }
                    // By Categories and Locations and Help
                    else if ((searchParms.CategoryIds != null &&
                              searchParms.CategoryIds.Count() > 0) &&
                             (searchParms.LocationIds != null &&
                              searchParms.LocationIds.Count() > 0) &&
                             searchParms.HelpDocOption == HelpDocOptions.JustHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid categoryId in searchParms.CategoryIds)
                            {
                                foreach (Guid locationId in searchParms.LocationIds)
                                {
                                    var Solutions = from p in svcPlsContext.Solutions
                                                    where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                          p.SolutionText.Contains(searchTerm)) &&
                                                          p.Ticket.CategoryId == categoryId &&
                                                          p.Ticket.LocationId == locationId &&
                                                          p.Ticket.IsHelpDoc == true
                                                    orderby p.LikeCount descending
                                                    select new
                                                    {
                                                        SolutionId = p.SolutionId,
                                                        TicketId = p.TicketId,
                                                        SolutionShortDesc = p.SolutionShortDesc,
                                                        SolutionText = p.SolutionText,
                                                        CreateDate = p.CreateDate,
                                                        EditDate = p.EditDate,
                                                        UserId = p.UserId,
                                                        LikeCount = p.LikeCount,
                                                        UnlikeCount = p.UnlikeCount,
                                                        IsHelpDoc = p.Ticket.IsHelpDoc
                                                    };

                                    if (Solutions != null)
                                    {
                                        //foundSolutions.AddRange(Solutions.ToList<Solution>());
                                        foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                                    }
                                }
                            }
                        }
                    }
                    // By Categories and Locations and No Help
                    else if ((searchParms.CategoryIds != null &&
                              searchParms.CategoryIds.Count() > 0) &&
                             (searchParms.LocationIds != null &&
                              searchParms.LocationIds.Count() > 0) &&
                             searchParms.HelpDocOption == HelpDocOptions.NoHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid categoryId in searchParms.CategoryIds)
                            {
                                foreach (Guid locationId in searchParms.LocationIds)
                                {

                                    var Solutions = from p in svcPlsContext.Solutions
                                                    where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                           p.SolutionText.Contains(searchTerm)) &&
                                                          p.Ticket.CategoryId == categoryId &&
                                                          p.Ticket.LocationId == locationId &&
                                                          p.Ticket.IsHelpDoc == false
                                                    orderby p.LikeCount descending
                                                    select new
                                                    {
                                                        SolutionId = p.SolutionId,
                                                        TicketId = p.TicketId,
                                                        SolutionShortDesc = p.SolutionShortDesc,
                                                        SolutionText = p.SolutionText,
                                                        CreateDate = p.CreateDate,
                                                        EditDate = p.EditDate,
                                                        UserId = p.UserId,
                                                        LikeCount = p.LikeCount,
                                                        UnlikeCount = p.UnlikeCount,
                                                        IsHelpDoc = p.Ticket.IsHelpDoc
                                                    };

                                    if (Solutions != null)
                                    {
                                        //foundSolutions.AddRange(Solutions.ToList<Solution>());
                                        foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                                    }
                                }
                            }
                        }
                    }
                    // By Categories and Locations and Combined Help
                    else if ((searchParms.CategoryIds != null &&
                              searchParms.CategoryIds.Count() > 0) &&
                             (searchParms.LocationIds != null &&
                              searchParms.LocationIds.Count() > 0) &&
                             searchParms.HelpDocOption == HelpDocOptions.Combined)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            foreach (Guid categoryId in searchParms.CategoryIds)
                            {
                                foreach (Guid locationId in searchParms.LocationIds)
                                {
                                    var Solutions = from p in svcPlsContext.Solutions
                                                    where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                          p.SolutionText.Contains(searchTerm)) &&
                                                          p.Ticket.CategoryId == categoryId &&
                                                          p.Ticket.LocationId == locationId
                                                    orderby p.LikeCount descending
                                                    select new
                                                    {
                                                        SolutionId = p.SolutionId,
                                                        TicketId = p.TicketId,
                                                        SolutionShortDesc = p.SolutionShortDesc,
                                                        SolutionText = p.SolutionText,
                                                        CreateDate = p.CreateDate,
                                                        EditDate = p.EditDate,
                                                        UserId = p.UserId,
                                                        LikeCount = p.LikeCount,
                                                        UnlikeCount = p.UnlikeCount,
                                                        IsHelpDoc = p.Ticket.IsHelpDoc
                                                    };

                                    if (Solutions != null)
                                    {
                                        //foundSolutions.AddRange(Solutions.ToList<Solution>());
                                        foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                                    }
                                }
                            }
                        }
                    }
                    // By No Categories or Locations and No Help
                    else if ((searchParms.CategoryIds == null ||
                              searchParms.CategoryIds.Count() < 1) &&
                             (searchParms.LocationIds == null ||
                              searchParms.LocationIds.Count() < 1) &&
                             searchParms.HelpDocOption == HelpDocOptions.NoHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            var Solutions = from p in svcPlsContext.Solutions
                                            where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                   p.SolutionText.Contains(searchTerm)) &&
                                                  p.Ticket.IsHelpDoc == false
                                            orderby p.LikeCount descending
                                            select new
                                            {
                                                SolutionId = p.SolutionId,
                                                TicketId = p.TicketId,
                                                SolutionShortDesc = p.SolutionShortDesc,
                                                SolutionText = p.SolutionText,
                                                CreateDate = p.CreateDate,
                                                EditDate = p.EditDate,
                                                UserId = p.UserId,
                                                LikeCount = p.LikeCount,
                                                UnlikeCount = p.UnlikeCount,
                                                IsHelpDoc = p.Ticket.IsHelpDoc
                                            };

                            if (Solutions != null)
                            {
                                //foundSolutions.AddRange(Solutions.ToList<Solution>());
                                foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                            }
                        }
                    }
                    // By No Categories or Locations and Help
                    else if ((searchParms.CategoryIds == null ||
                              searchParms.CategoryIds.Count() < 1) &&
                             (searchParms.LocationIds == null ||
                              searchParms.LocationIds.Count() < 1) &&
                             searchParms.HelpDocOption == HelpDocOptions.JustHelpDocs)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            var Solutions = from p in svcPlsContext.Solutions
                                            where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                   p.SolutionText.Contains(searchTerm)) &&
                                                  p.Ticket.IsHelpDoc == true
                                            orderby p.LikeCount descending
                                            select new
                                            {
                                                SolutionId = p.SolutionId,
                                                TicketId = p.TicketId,
                                                SolutionShortDesc = p.SolutionShortDesc,
                                                SolutionText = p.SolutionText,
                                                CreateDate = p.CreateDate,
                                                EditDate = p.EditDate,
                                                UserId = p.UserId,
                                                LikeCount = p.LikeCount,
                                                UnlikeCount = p.UnlikeCount,
                                                IsHelpDoc = p.Ticket.IsHelpDoc
                                            };

                            if (Solutions != null)
                            {
                                //foundSolutions.AddRange(Solutions.ToList<Solution>());
                                foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                            }
                        }
                    }
                    // By No Categories or Locations and Combined Help
                    else if ((searchParms.CategoryIds == null ||
                              searchParms.CategoryIds.Count() < 1) &&
                             (searchParms.LocationIds == null ||
                              searchParms.LocationIds.Count() < 1) &&
                             searchParms.HelpDocOption == HelpDocOptions.Combined)
                    {
                        foreach (string searchTerm in searchParms.SearchTerms)
                        {
                            var Solutions = from p in svcPlsContext.Solutions
                                            where (p.SolutionShortDesc.Contains(searchTerm) ||
                                                    p.SolutionText.Contains(searchTerm))
                                            orderby p.LikeCount descending
                                            select new
                                            {
                                                SolutionId = p.SolutionId,
                                                TicketId = p.TicketId,
                                                SolutionShortDesc = p.SolutionShortDesc,
                                                SolutionText = p.SolutionText,
                                                CreateDate = p.CreateDate,
                                                EditDate = p.EditDate,
                                                UserId = p.UserId,
                                                LikeCount = p.LikeCount,
                                                UnlikeCount = p.UnlikeCount,
                                                IsHelpDoc = p.Ticket.IsHelpDoc
                                            };

                            if (Solutions != null)
                            {
                                //foundSolutions.AddRange(Solutions.ToList<Solution>());
                                foundSolutions.AddRange(Solutions.ToNonAnonymousList(typeof(SolutionWithHelpDoc)) as List<SolutionWithHelpDoc>);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in SolutionKnowledgeSearch(KnowledgeSearch searchParms).  Exception: {0}", ex.Message));

                return null;
            }

            return foundSolutions.OrderBy(fs => fs.UnlikeCount - fs.LikeCount).ThenBy(fsc => fsc.LikeCount).ToList<SolutionWithHelpDoc>(); ;
        }

        #endregion Search Methods

        #region Snooze Methods
        public List<Snooze> GetSnoozes()
        {
            List<Snooze> snoozeList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var snoozes = svcPlsContext.Snoozes.OrderBy(c => c.CreateDate);

                    if (snoozes != null)
                    {
                        snoozeList = snoozes.OrderByDescending(s => s.CreateDate).ToList<Snooze>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetSnoozes().  Error: {0}", ex.Message));

                snoozeList = null;
            }

            return snoozeList;
        }

        /// <summary>
        /// Snooze GetSnooze(string snoozeId)
        /// </summary>
        /// <param name="snoozeId"></param>
        /// <returns></returns>
        public Snooze GetSnooze(Guid snoozeId)
        {
            Snooze snooze = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var snoozes = svcPlsContext.Snoozes.Where(c => c.SnoozeId == snoozeId);

                    if (snoozes != null)
                    {
                        snooze = snoozes.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetSnooze(string snoozeId).  Error: {0}", ex.Message));

                snooze = null;
            }

            return snooze;
        }

        /// <summary>
        /// Snooze AddSnooze(Snooze snooze)
        /// </summary>
        /// <param name="snooze"></param>
        /// <returns></returns>
        public Snooze AddSnooze(Snooze snooze)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    if (snooze.SnoozeId == Guid.Empty)
                    {
                        snooze.SnoozeId = Guid.NewGuid();
                    }

                    svcPlsContext.Snoozes.InsertOnSubmit(snooze);

                    svcPlsContext.SubmitChanges();

                    //variables
                    DateTime startDate = DateTime.Parse(snooze.StartDate);
                    string startTime = startDate.ToString("hh:mm tt");
                    DateTime endDate = DateTime.Parse(snooze.EndDate);
                    string endTime = endDate.ToString("hh:mm tt");
                    int intervalTime = Convert.ToInt32(snooze.SnoozeInterval);
                    Guid intervalType = (Guid)snooze.IntervalTypeId;
                    int i = 1;
                    DateTime dtCnt = Convert.ToDateTime(startTime);
                    int interval = 0;
                    var intype = svcPlsContext.IntervalTypes.Where(t => t.IntervalTypeId == intervalType).FirstOrDefault();
                    if (intype != null)
                    {
                        string intType = intype.Name;
                        if (intType == "Hour")
                        {
                            interval = intervalTime * 60;
                        }
                        else if (intType == "Minute")
                        {
                            interval = intervalTime;
                        }

                    }
                    //For email sending purpose
                    for (DateTime dtCount = startDate; dtCount <= endDate; dtCount = dtCount.AddMinutes(interval))
                    {
                        //insert records in audit table with count records
                        SnoozeAudit snoozAdt = new SnoozeAudit();
                        snoozAdt.SnoozeAuditId = Guid.NewGuid();
                        snoozAdt.SnoozeId = snooze.SnoozeId;
                        snoozAdt.IntervalTime = interval;
                        snoozAdt.StartDate = snooze.StartDate;
                        snoozAdt.EndDate = snooze.EndDate;
                        snoozAdt.Count = i;
                        snoozAdt.EmailDate = Convert.ToString(dtCount);
                        if (dtCount == startDate)
                        {
                            snoozAdt.IsSent = "start";
                        }
                        else if (dtCount.AddMinutes(interval) > endDate || dtCount == endDate)
                        {
                            snoozAdt.IsSent = "end";
                        }
                        else
                        {
                            snoozAdt.IsSent = "no";
                        }

                        svcPlsContext.SnoozeAudits.InsertOnSubmit(snoozAdt);
                        svcPlsContext.SubmitChanges();
                        i = i + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in AddSnooze().  Error: {0}", ex.Message));

                snooze = null;
            }

            return snooze;
        }

        /// <summary>
        /// Snooze UpdateSnooze(Snooze snooze, Guid snoozeId)
        /// </summary>
        /// <param name="snooze"></param>
        /// <param name="snoozeId"></param>
        /// <returns></returns>
        public Snooze UpdateSnooze(Snooze snooze, Guid snoozeId)
        {
            Snooze snz = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific organization from Database
                    snz = (from c in svcPlsContext.Snoozes
                           where c.SnoozeId == snoozeId
                           select c).FirstOrDefault();

                    if (snz != null)
                    {
                        snz.CompletedDate = snooze.CompletedDate;
                        //snz.CreateDate = snooze.CreateDate;
                        snz.EndDate = snooze.EndDate;
                        snz.IntervalTypeId = snooze.IntervalTypeId;
                        snz.IsCompleted = snooze.IsCompleted;
                        snz.IsDateInterval = snooze.IsDateInterval;
                        snz.IsQuickShare = snooze.IsQuickShare;
                        snz.QuickShareHours = snooze.QuickShareHours;
                        snz.QuickShareMinutes = snooze.QuickShareMinutes;
                        snz.SnoozeInterval = snooze.SnoozeInterval;
                        snz.ReasonId = snooze.ReasonId;
                        snz.StartDate = snooze.StartDate;
                        snz.EditDate = snooze.EditDate;

                        //Save to database
                        svcPlsContext.SubmitChanges();

                        //Add records in snoozeaudit
                        if (Convert.ToDateTime(snz.StartDate) >= DateTime.Now && Convert.ToDateTime(snz.EndDate) > DateTime.Now)
                        {
                            var snzAdt = (from s in svcPlsContext.SnoozeAudits
                                          where s.SnoozeId == snoozeId
                                          select s).FirstOrDefault();

                            if (snzAdt != null)
                            {
                                svcPlsContext.SnoozeAudits.DeleteOnSubmit(snzAdt);
                                svcPlsContext.SubmitChanges();
                            }

                            //variables
                            DateTime startDate = DateTime.Parse(snooze.StartDate);
                            string startTime = startDate.ToString("hh:mm tt");
                            DateTime endDate = DateTime.Parse(snooze.EndDate);
                            string endTime = endDate.ToString("hh:mm tt");
                            int intervalTime = Convert.ToInt32(snooze.SnoozeInterval);
                            Guid intervalType = (Guid)snooze.IntervalTypeId;
                            int i = 1;
                            DateTime dtCnt = Convert.ToDateTime(startTime);
                            int interval = 0;
                            var intype = svcPlsContext.IntervalTypes.Where(t => t.IntervalTypeId == intervalType).FirstOrDefault();
                            if (intype != null)
                            {
                                string intType = intype.Name;
                                if (intType == "Hour")
                                {
                                    interval = intervalTime * 60;
                                }
                                else if (intType == "Minute")
                                {
                                    interval = intervalTime;
                                }

                            }
                            //For email sending purpose
                            for (DateTime dtCount = startDate; dtCount <= endDate; dtCount = dtCount.AddMinutes(interval))
                            {
                                //insert records in audit table with count records
                                SnoozeAudit snoozAdt = new SnoozeAudit();
                                snoozAdt.SnoozeAuditId = Guid.NewGuid();
                                snoozAdt.SnoozeId = snooze.SnoozeId;
                                snoozAdt.IntervalTime = interval;
                                snoozAdt.StartDate = snooze.StartDate;
                                snoozAdt.EndDate = snooze.EndDate;
                                snoozAdt.Count = i;
                                snoozAdt.EmailDate = Convert.ToString(dtCount);
                                if (dtCount == startDate)
                                {
                                    snoozAdt.IsSent = "start";
                                }
                                else if (dtCount.AddMinutes(interval) > endDate || dtCount == endDate)
                                {
                                    snoozAdt.IsSent = "end";
                                }
                                else
                                {
                                    snoozAdt.IsSent = "no";
                                }

                                svcPlsContext.SnoozeAudits.InsertOnSubmit(snoozAdt);
                                svcPlsContext.SubmitChanges();
                                i = i + 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in UpdateSnooze().  Error: {0}", ex.Message));

                snz = null;
            }

            return snz;
        }

        /// <summary>
        /// bool DeleteSnooze(Guid snoozeId)
        /// </summary>
        /// <param name="snoozeId"></param>
        /// <returns></returns>
        public bool DeleteSnooze(Guid snoozeId)
        {
            bool result = false;

            Snooze snz = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //check the ticketId from snoozedticket table
                    var delsnoozeticket = (from st in svcPlsContext.SnoozedTickets
                                           where st.SnoozeId == snoozeId
                                           select st).FirstOrDefault();

                    if (delsnoozeticket != null)
                    {
                        svcPlsContext.SnoozedTickets.DeleteOnSubmit(delsnoozeticket);
                        svcPlsContext.SubmitChanges();
                    }


                    //Get the specific snooze from Database
                    snz = (from c in svcPlsContext.Snoozes
                           where c.SnoozeId == snoozeId
                           select c).FirstOrDefault();


                    if (snz != null)
                    {
                        // Delete the snooze
                        svcPlsContext.Snoozes.DeleteOnSubmit(snz);

                        // Save to database
                        svcPlsContext.SubmitChanges();

                        //Delete the snooze from snoozeaudit table
                        var delsnzadt = svcPlsContext.SnoozeAudits.Where(t => t.SnoozeId == snoozeId);
                        if (delsnzadt != null)
                        {
                            svcPlsContext.SnoozeAudits.DeleteAllOnSubmit(delsnzadt);
                            svcPlsContext.SubmitChanges();
                        }

                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in DeleteSnooze().  Error: {0}", ex.Message));

                result = false;
            }

            return result;
        }
        #endregion Snooze Methods

        #region SnoozeReason Methods
        public List<SnoozeReason> GetSnoozeReasons()
        {
            List<SnoozeReason> snoozeReasonList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var snoozeReasons = svcPlsContext.SnoozeReasons.OrderBy(c => c.Name);

                    if (snoozeReasons != null)
                    {
                        snoozeReasonList = snoozeReasons.ToList<SnoozeReason>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetSnoozeReasons().  Error: {0}", ex.Message));

                snoozeReasonList = null;
            }

            return snoozeReasonList;
        }

        /// <summary>
        /// SnoozeReason GetSnoozeReason(string snoozeReasonId)
        /// </summary>
        /// <param name="snoozeReasonId"></param>
        /// <returns></returns>
        public SnoozeReason GetSnoozeReason(Guid snoozeReasonId)
        {
            SnoozeReason snoozeReason = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var snoozeReasons = svcPlsContext.SnoozeReasons.Where(c => c.ReasonId == snoozeReasonId);

                    if (snoozeReasons != null)
                    {
                        snoozeReason = snoozeReasons.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetSnoozeReason(string snoozeReasonId).  Error: {0}", ex.Message));

                snoozeReason = null;
            }

            return snoozeReason;
        }

        /// <summary>
        /// string AddSnoozeReason(SnoozeReason snoozeReason)
        /// </summary>
        /// <param name="snoozeReason"></param>
        /// <returns></returns>
        public string AddSnoozeReason(SnoozeReason snoozeReason)
        {
            string message = string.Empty;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    if (snoozeReason.ReasonId == Guid.Empty)
                    {
                        snoozeReason.ReasonId = Guid.NewGuid();
                    }

                    svcPlsContext.SnoozeReasons.InsertOnSubmit(snoozeReason);

                    svcPlsContext.SubmitChanges();
                    message = "New SnoozeReason Added Successfully";
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in AddSnoozeReason().  Error: {0}", ex.Message));

                snoozeReason = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "AddSnoozeReason";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {

                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();


                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }

                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }

        /// <summary>
        /// string UpdateSnoozeReason(SnoozeReason snoozeReason, Guid snoozeReasonId)
        /// </summary>
        /// <param name="snoozeReason"></param>
        /// <param name="snoozeReasonId"></param>
        /// <returns></returns>
        public string UpdateSnoozeReason(SnoozeReason snoozeReason, Guid snoozeReasonId)
        {
            SnoozeReason snz = null;
            string message = string.Empty;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific organization from Database
                    snz = (from c in svcPlsContext.SnoozeReasons
                           where c.ReasonId == snoozeReasonId
                           select c).FirstOrDefault();

                    if (snz != null)
                    {
                        snz.Name = snoozeReason.Name;
                        //snz.CreateDate = snoozeReason.CreateDate;
                        snz.EditDate = snoozeReason.EditDate;

                        //Save to database
                        svcPlsContext.SubmitChanges();
                        message = "Selected Snooze Reason Updated Successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in UpdateSnoozeReason().  Error: {0}", ex.Message));

                snz = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "UpdateSnoozeReason";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {

                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();


                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }

                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }

        /// <summary>
        /// string DeleteSnoozeReason(Guid snoozeReasonId)
        /// </summary>
        /// <param name="snoozeReasonId"></param>
        /// <returns></returns>
        public string DeleteSnoozeReason(Guid snoozeReasonId)
        {
            string message = string.Empty;
            SnoozeReason cat = null;
            Snooze sno = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Check the SnoozeReasonId is available in Snooze table
                    sno = (from s in svcPlsContext.Snoozes
                           where s.ReasonId == snoozeReasonId
                           select s).FirstOrDefault();

                    if (sno != null)
                    {
                        message = "This SnoozeReason is used by one of the Snoozed Tech in the Snooze list";
                    }
                    else
                    {
                        //Get the specific organization from Database
                        cat = (from c in svcPlsContext.SnoozeReasons
                               where c.ReasonId == snoozeReasonId
                               select c).FirstOrDefault();

                        if (cat != null)
                        {
                            // Delete the organization
                            svcPlsContext.SnoozeReasons.DeleteOnSubmit(cat);

                            // Save to database
                            svcPlsContext.SubmitChanges();
                            message = "Selected SnoozeReason Deleted Successfully";

                            //result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in DeleteSnoozeReason().  Error: {0}", ex.Message));

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "DeleteSnoozeReason";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {

                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();


                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }

                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }
        #endregion SnoozeReason Methods

        #region IntervalType Methods
        /// <summary>
        /// List<IntervalType> GetIntervalTypes()
        /// </summary>
        /// <returns></returns>
        public List<IntervalType> GetIntervalTypes()
        {
            List<IntervalType> intervalTypeList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var intervalTypes = svcPlsContext.IntervalTypes.OrderBy(c => c.Name);

                    if (intervalTypes != null)
                    {
                        intervalTypeList = intervalTypes.ToList<IntervalType>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetIntervalTypes().  Error: {0}", ex.Message));

                intervalTypeList = null;
            }

            return intervalTypeList;
        }

        /// <summary>
        /// IntervalType GetIntervalType(Guid intervalTypeId)
        /// </summary>
        /// <param name="intervalTypeId"></param>
        /// <returns></returns>
        public IntervalType GetIntervalType(Guid intervalTypeId)
        {
            IntervalType intervalType = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var intervalTypes = svcPlsContext.IntervalTypes.Where(c => c.IntervalTypeId == intervalTypeId);

                    if (intervalTypes != null)
                    {
                        intervalType = intervalTypes.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetIntervalType(string intervalTypeId).  Error: {0}", ex.Message));

                intervalType = null;
            }

            return intervalType;
        }

        /// <summary>
        /// string AddIntervalType(IntervalType intervalType)
        /// </summary>
        /// <param name="intervalType"></param>
        /// <returns></returns>
        public string AddIntervalType(IntervalType intervalType)
        {
            string message = string.Empty;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    if (intervalType.IntervalTypeId == Guid.Empty)
                    {
                        intervalType.IntervalTypeId = Guid.NewGuid();
                    }

                    svcPlsContext.IntervalTypes.InsertOnSubmit(intervalType);

                    svcPlsContext.SubmitChanges();
                    message = "New IntervalType Added Successfully";
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in AddIntervalType().  Error: {0}", ex.Message));

                intervalType = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "AddIntervalType";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {

                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();


                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }

                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }

        /// <summary>
        /// string UpdateIntervalType(IntervalType intervalType, Guid intervalTypeId)
        /// </summary>
        /// <param name="intervalType"></param>
        /// <param name="intervalTypeId"></param>
        /// <returns></returns>
        public string UpdateIntervalType(IntervalType intervalType, Guid intervalTypeId)
        {
            IntervalType snz = null;
            string message = string.Empty;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific organization from Database
                    snz = (from c in svcPlsContext.IntervalTypes
                           where c.IntervalTypeId == intervalTypeId
                           select c).FirstOrDefault();

                    if (snz != null)
                    {
                        snz.Name = intervalType.Name;
                        //snz.CreateDate = intervalType.CreateDate;
                        snz.EditDate = intervalType.EditDate;

                        //Save to database
                        svcPlsContext.SubmitChanges();
                        message = "Selected IntervalType Updated Successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in UpdateIntervalType().  Error: {0}", ex.Message));

                snz = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "UpdateIntervalType";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();
                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }

                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }

        /// <summary>
        /// string DeleteIntervalType(Guid intervalTypeId)
        /// </summary>
        /// <param name="intervalTypeId"></param>
        /// <returns></returns>
        public string DeleteIntervalType(Guid intervalTypeId)
        {
            IntervalType cat = null;
            Snooze sno = null;
            string message = string.Empty;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Check the IntervalType is available in Snooze table
                    sno = (from s in svcPlsContext.Snoozes
                           where s.IntervalTypeId == intervalTypeId
                           select s).FirstOrDefault();

                    if (sno != null)
                    {
                        message = "This IntervalType is used by one of Snoozed Tech in the Snooze list.";
                    }
                    else
                    {
                        //Get the specific organization from Database
                        cat = (from c in svcPlsContext.IntervalTypes
                               where c.IntervalTypeId == intervalTypeId
                               select c).FirstOrDefault();

                        if (cat != null)
                        {
                            // Delete the organization
                            svcPlsContext.IntervalTypes.DeleteOnSubmit(cat);

                            // Save to database
                            svcPlsContext.SubmitChanges();
                            message = "Selected IntervalType Deleted Successfully";
                            //result = true;
                        }
                    }
                    //cat = (from c in svcPlsContext.IntervalTypes
                    //       where c.IntervalTypeId == intervalTypeId
                    //       select c).FirstOrDefault();

                    //if (cat != null)
                    //{
                    //    // Delete the organization
                    //    svcPlsContext.IntervalTypes.DeleteOnSubmit(cat);

                    //    // Save to database
                    //    svcPlsContext.SubmitChanges();

                    //    result = true;
                    //}
                    //else
                    //{
                    //    result = false;
                    //}
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in DeleteIntervalType().  Error: {0}", ex.Message));

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "DeleteIntervalType";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();
                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }

                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }
        #endregion IntervalType Methods

        #region ApplicationType Methods
        /// <summary>
        /// List<ApplicationType> GetApplicationTypes()
        /// </summary>
        /// <returns></returns>
        public List<ApplicationType> GetApplicationTypes()
        {
            List<ApplicationType> applicationTypeList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var applicationTypes = svcPlsContext.ApplicationTypes.OrderBy(c => c.Name);

                    if (applicationTypes != null)
                    {
                        applicationTypeList = applicationTypes.ToList<ApplicationType>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetApplicationTypes().  Error: {0}", ex.Message));

                applicationTypeList = null;
            }

            return applicationTypeList;
        }

        /// <summary>
        /// ApplicationType GetApplicationType(Guid applicationTypeId)
        /// </summary>
        /// <param name="applicationTypeId"></param>
        /// <returns></returns>
        public ApplicationType GetApplicationType(Guid applicationTypeId)
        {
            ApplicationType applicationType = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var applicationTypes = svcPlsContext.ApplicationTypes.Where(c => c.ApplicationTypeId == applicationTypeId);

                    if (applicationTypes != null)
                    {
                        applicationType = applicationTypes.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetApplicationType(string applicationTypeId).  Error: {0}", ex.Message));

                applicationType = null;
            }

            return applicationType;
        }

        /// <summary>
        /// ApplicationType AddApplicationType(ApplicationType applicationType)
        /// </summary>
        /// <param name="applicationType"></param>
        /// <returns></returns>
        public ApplicationType AddApplicationType(ApplicationType applicationType)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    if (applicationType.ApplicationTypeId == Guid.Empty)
                    {
                        applicationType.ApplicationTypeId = Guid.NewGuid();
                    }

                    svcPlsContext.ApplicationTypes.InsertOnSubmit(applicationType);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in AddApplicationType().  Error: {0}", ex.Message));

                applicationType = null;
            }

            return applicationType;
        }

        /// <summary>
        /// ApplicationType UpdateApplicationType(ApplicationType applicationType, Guid applicationTypeId)
        /// </summary>
        /// <param name="applicationType"></param>
        /// <param name="applicationTypeId"></param>
        /// <returns></returns>
        public ApplicationType UpdateApplicationType(ApplicationType applicationType, Guid applicationTypeId)
        {
            ApplicationType snz = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific organization from Database
                    snz = (from c in svcPlsContext.ApplicationTypes
                           where c.ApplicationTypeId == applicationTypeId
                           select c).FirstOrDefault();

                    if (snz != null)
                    {
                        snz.Name = applicationType.Name;
                        //snz.CreateDate = applicationType.CreateDate;
                        snz.EditDate = applicationType.EditDate;

                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in UpdateApplicationType().  Error: {0}", ex.Message));

                snz = null;
            }

            return snz;
        }

        /// <summary>
        /// bool DeleteApplicationType(Guid applicationTypeId)
        /// </summary>
        /// <param name="applicationTypeId"></param>
        /// <returns></returns>
        public bool DeleteApplicationType(Guid applicationTypeId)
        {
            bool result = false;

            ApplicationType cat = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific organization from Database
                    cat = (from c in svcPlsContext.ApplicationTypes
                           where c.ApplicationTypeId == applicationTypeId
                           select c).FirstOrDefault();

                    if (cat != null)
                    {
                        // Delete the organization
                        svcPlsContext.ApplicationTypes.DeleteOnSubmit(cat);

                        // Save to database
                        svcPlsContext.SubmitChanges();

                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in DeleteApplicationType().  Error: {0}", ex.Message));

                result = false;
            }

            return result;
        }
        #endregion ApplicationType Methods

        #region SystemConfig Methods
        /// <summary>
        /// List<SystemConfig> GetSystemConfigs()
        /// </summary>
        /// <returns></returns>
        public List<SystemConfig> GetSystemConfigs()
        {
            List<SystemConfig> systemConfigList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var systemConfigs = svcPlsContext.SystemConfigs.OrderBy(c => c.ConfigKey);

                    if (systemConfigs != null)
                    {
                        systemConfigList = systemConfigs.ToList<SystemConfig>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetSystemConfigs().  Error: {0}", ex.Message));

                systemConfigList = null;
            }

            return systemConfigList;
        }

        /// <summary>
        /// SystemConfig GetSystemConfig(Guid systemConfigId)
        /// </summary>
        /// <param name="systemConfigId"></param>
        /// <returns></returns>
        public SystemConfig GetSystemConfig(Guid systemConfigId)
        {
            SystemConfig systemConfig = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var systemConfigs = svcPlsContext.SystemConfigs.Where(c => c.SystemConfigId == systemConfigId);

                    if (systemConfigs != null)
                    {
                        systemConfig = systemConfigs.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetSystemConfig(Guid systemConfigId).  Error: {0}", ex.Message));

                systemConfig = null;
            }

            return systemConfig;
        }

        /// <summary>
        /// string GetSystemConfigValue(string configKey)
        /// 
        /// Gets SystemConfig Value by its Key
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        public string GetSystemConfigValue(string configKey)
        {
            Logger.Write(string.Format("ConfigKey from GetSystemConfigValue: '{0}'", configKey));

            string systemConfigValue = string.Empty;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var systemConfigs = svcPlsContext.SystemConfigs.Where(c => c.ConfigKey == configKey);

                    if (systemConfigs != null)
                    {
                        systemConfigValue = systemConfigs.FirstOrDefault().ConfigValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetSystemConfigValue(string configKey).  Error: {0}", ex.Message));

                systemConfigValue = string.Empty;
            }

            return systemConfigValue;
        }

        /// <summary>
        /// SystemConfig AddSystemConfig(SystemConfig systemConfig)
        /// </summary>
        /// <param name="systemConfig"></param>
        /// <returns></returns>
        public SystemConfig AddSystemConfig(SystemConfig systemConfig)
        {
            Logger.Write(string.Format("SystemConfigId: '{0}'", systemConfig.SystemConfigId));

            try
            {
                SystemConfig foundConfig = GetSystemConfig(systemConfig.SystemConfigId);

                if (foundConfig != null && string.IsNullOrWhiteSpace(foundConfig.ConfigKey) != true)
                {
                    return foundConfig;
                }

                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    if (systemConfig.SystemConfigId == Guid.Empty)
                    {
                        systemConfig.SystemConfigId = Guid.NewGuid();
                    }

                    Logger.Write(string.Format("ConfigKey: '{0}'", systemConfig.ConfigKey));

                    svcPlsContext.SystemConfigs.InsertOnSubmit(systemConfig);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in AddSystemConfig().  Error: {0}", ex.Message));

                systemConfig = null;
            }

            return systemConfig;
        }

        /// <summary>
        /// SystemConfig UpdateSystemConfig(SystemConfig systemConfig, Guid systemConfigId)
        /// </summary>
        /// <param name="systemConfig"></param>
        /// <param name="systemConfigId"></param>
        /// <returns></returns>
        public SystemConfig UpdateSystemConfig(SystemConfig systemConfig, Guid systemConfigId)
        {
            SystemConfig config = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific organization from Database
                    config = (from c in svcPlsContext.SystemConfigs
                              where c.SystemConfigId == systemConfigId
                              select c).FirstOrDefault();

                    if (config != null)
                    {
                        config.ConfigKey = systemConfig.ConfigKey;
                        config.ConfigValue = systemConfig.ConfigValue;
                        //config.CreateDate = systemConfig.CreateDate;
                        config.EditDate = systemConfig.EditDate;

                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in UpdateSystemConfig().  Error: {0}", ex.Message));

                config = null;
            }

            return config;
        }

        /// <summary>
        /// bool DeleteSystemConfig(Guid systemConfigId)
        /// </summary>
        /// <param name="systemConfigId"></param>
        /// <returns></returns>
        public bool DeleteSystemConfig(Guid systemConfigId)
        {
            bool result = false;

            SystemConfig config = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific organization from Database
                    config = (from c in svcPlsContext.SystemConfigs
                              where c.SystemConfigId == systemConfigId
                              select c).FirstOrDefault();

                    if (config != null)
                    {
                        // Delete the organization
                        svcPlsContext.SystemConfigs.DeleteOnSubmit(config);

                        // Save to database
                        svcPlsContext.SubmitChanges();

                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in DeleteSystemConfig().  Error: {0}", ex.Message));

                result = false;
            }

            return result;
        }
        #endregion SystemConfig Methods

        #region Tech Performance Methods
        /// <summary>
        /// long GetOpenTickets(Guid techId, string startDate, string endDate)
        /// 
        /// Gets a count of open tickets for the given tech during the time range specified.
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public long GetOpenTickets(Guid techId, string startDate, string endDate)
        {
            long openTicketCount = 0;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    openTicketCount = svcPlsContext.Tickets.Where(t => t.CloseDate == null &&
                                                                        t.UserId == techId &&
                                                                        Convert.ToDateTime(t.CreateDate) >= Convert.ToDateTime(startDate) &&
                                                                        Convert.ToDateTime(t.CreateDate) <= Convert.ToDateTime(endDate)).Count();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in GetOpenTickets.  Exception: {0}", ex.Message));

                return 0;
            }

            return openTicketCount;
        }

        /// <summary>
        /// long GetClosedTickets(Guid techId, string startDate, string endDate)
        /// 
        /// Gets a count of closed tickets for the given tech during the time range specified.
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public long GetClosedTickets(Guid techId, string startDate, string endDate)
        {
            long closedTicketCount = 0;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    closedTicketCount = svcPlsContext.Tickets.Where(t => t.CloseDate != null &&
                                                                         t.UserId == techId &&
                                                                         Convert.ToDateTime(t.CreateDate) >= Convert.ToDateTime(startDate) &&
                                                                         Convert.ToDateTime(t.CreateDate) <= Convert.ToDateTime(endDate)).Count();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in GetClosedTickets.  Exception: {0}", ex.Message));

                return 0;
            }

            return closedTicketCount;
        }

        /// <summary>
        /// double GetAverageTimeToCloseByTech(Guid techId, string startDate, string endDate)
        /// 
        /// Gets average time between ticket creation and closing for a given tech.  
        /// Expressed in minutes.
        /// 
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public double GetAverageTimeToCloseByTech(Guid techId, string startDate, string endDate)
        {
            TimeSpan averageTicketTime;

            List<TimeSpan> timeSpans = new List<TimeSpan>();

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var tickets = svcPlsContext.Tickets.Where(t => t.CloseDate != null &&
                                                                   t.UserId == techId &&
                                                                  Convert.ToDateTime(t.CreateDate) >= Convert.ToDateTime(startDate) &&
                                                                  Convert.ToDateTime(t.CreateDate) <= Convert.ToDateTime(endDate));

                    if (tickets != null &&
                        tickets.Count() > 0)
                    {
                        foreach (Ticket ticket in tickets)
                        {
                            TimeSpan timeSpan = (TimeSpan)(Convert.ToDateTime(ticket.CloseDate) - Convert.ToDateTime(ticket.CreateDate));

                            timeSpans.Add(timeSpan);
                        }
                    }
                    else
                    {
                        return 0.00D;
                    }

                    TimeSpan cummulative = new TimeSpan();

                    foreach (TimeSpan ts in timeSpans)
                    {
                        cummulative += ts;
                    }

                    double averageTicks = cummulative.Ticks / timeSpans.Count;

                    averageTicketTime = TimeSpan.FromTicks((long)averageTicks);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in GetAverageTimeToCloseByTech.  Exception: {0}", ex.Message));

                return TimeSpan.MinValue.TotalMinutes;
            }

            return averageTicketTime.TotalMinutes;
        }

        /// <summary>
        /// double GetAverageResponseTimeByTech(Guid techId, string startDate, string endDate)
        /// 
        /// Gets average time for the given Tech to respond to tickets.
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public double GetAverageResponseTimeByTech(Guid techId, string startDate, string endDate)
        {
            TimeSpan averageResponseTime;

            List<TimeSpan> timeSpans = new List<TimeSpan>();

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var tickets = svcPlsContext.Tickets.Where(t => t.ResponseTimeStamp != null &&
                                                                   t.UserId == techId &&
                                                                    Convert.ToDateTime(t.CreateDate) >= Convert.ToDateTime(startDate) &&
                                                                    Convert.ToDateTime(t.CreateDate) <= Convert.ToDateTime(endDate));

                    if (tickets != null &&
                        tickets.Count() > 0)
                    {
                        foreach (Ticket ticket in tickets)
                        {
                            TimeSpan timeSpan = (TimeSpan)(Convert.ToDateTime(ticket.ResponseTimeStamp) - Convert.ToDateTime(ticket.CreateDate));

                            timeSpans.Add(timeSpan);
                        }
                    }
                    else
                    {
                        return 0.00D;
                    }

                    TimeSpan cummulative = new TimeSpan();

                    foreach (TimeSpan ts in timeSpans)
                    {
                        cummulative += ts;
                    }

                    double averageTicks = cummulative.Ticks / timeSpans.Count;

                    averageResponseTime = TimeSpan.FromTicks((long)averageTicks);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in GetAverageResponseTimeByTech.  Exception: {0}", ex.Message));

                return TimeSpan.MinValue.TotalMinutes;
            }

            return averageResponseTime.TotalMinutes;
        }

        /// <summary>
        /// List<TechMetrics> GetTechMetrics()
        /// </summary>
        /// <returns></returns>
        public List<TechMetrics> GetTechMetrics()
        {
            DateTime endDate = DateTime.UtcNow.Date.AddDays(1);
            DateTime startDate = DateTime.UtcNow.Date.AddDays(-90).Date;

            List<TechMetrics> techMetricsList = new List<TechMetrics>();
            List<User> userList = null;
            TimeSpan averageTicketTime;
            List<TimeSpan> atcTimeSpan = new List<TimeSpan>();
            TimeSpan averageResponseTime;
            List<TimeSpan> artTimeSpans = new List<TimeSpan>();
            TechMetrics techMetric = new TechMetrics();
            Guid userId;
            long openTicketCnt = 0;
            long closedTicketCnt = 0;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //check users
                    var users = svcPlsContext.Users.OrderBy(u => u.UserName);

                    if (users != null)
                    {
                        userList = users.ToList<User>();

                        foreach (var userDetail in userList)
                        {
                            techMetric = new TechMetrics();
                            userId = userDetail.UserId;

                            //Get Openticket Count
                            openTicketCnt = svcPlsContext.Tickets.Where(t => t.CloseDate == null &&
                                                                                t.Status.ToUpper() != "C" &&
                                                                                t.UserId == userId &&
                                                                                Convert.ToDateTime(t.CreateDate) >= startDate &&
                                                                                Convert.ToDateTime(t.CreateDate) <= endDate).Count();

                            techMetric.TTO = openTicketCnt;


                            //Get Closeticket Count
                            closedTicketCnt = svcPlsContext.Tickets.Where(t => t.CloseDate != null &&
                                                                                 t.Status.ToUpper() == "C" &&
                                                                                 t.UserId == userId &&
                                                                                 Convert.ToDateTime(t.CreateDate) >= startDate &&
                                                                                 Convert.ToDateTime(t.CreateDate) <= endDate).Count();
                            techMetric.TTC = closedTicketCnt;


                            //Get Average time to close tickets
                            var atoTickets = svcPlsContext.Tickets.Where(t => t.CloseDate != null &&
                                                                           t.UserId == userId &&
                                                                           Convert.ToDateTime(t.CreateDate) >= startDate &&
                                                                           Convert.ToDateTime(t.CreateDate) <= endDate);

                            if (atoTickets != null && atoTickets.Count() > 0)
                            {
                                foreach (Ticket ticket in atoTickets)
                                {
                                    TimeSpan timeSpan = (TimeSpan)(Convert.ToDateTime(ticket.CloseDate) - Convert.ToDateTime(ticket.CreateDate));

                                    atcTimeSpan.Add(timeSpan);
                                }
                                TimeSpan cummulative = new TimeSpan();

                                foreach (TimeSpan ts in atcTimeSpan)
                                {
                                    cummulative += ts;
                                }

                                double averageTick = cummulative.Ticks / atcTimeSpan.Count;

                                averageTicketTime = TimeSpan.FromTicks((long)averageTick);
                                techMetric.ATO = averageTicketTime.TotalMinutes;

                            }
                            else
                            {
                                techMetric.ATO = 0.00D;
                            }

                            //Get Average Response Time Tickets
                            var artTickets = svcPlsContext.Tickets.Where(t => t.ResponseTimeStamp != null &&
                                                                           t.UserId == userId &&
                                                                           Convert.ToDateTime(t.CreateDate) >= startDate &&
                                                                           Convert.ToDateTime(t.CreateDate) <= endDate);

                            if (artTickets != null && artTickets.Count() > 0)
                            {
                                foreach (Ticket ticket in artTickets)
                                {
                                    TimeSpan timeSpan = (TimeSpan)(Convert.ToDateTime(ticket.ResponseTimeStamp) - Convert.ToDateTime(ticket.CreateDate));

                                    artTimeSpans.Add(timeSpan);
                                }
                                TimeSpan cummulatives = new TimeSpan();

                                foreach (TimeSpan ts in artTimeSpans)
                                {
                                    cummulatives += ts;
                                }

                                double averageTicks = cummulatives.Ticks / artTimeSpans.Count;

                                averageResponseTime = TimeSpan.FromTicks((long)averageTicks);
                                techMetric.ART = averageResponseTime.TotalMinutes;
                            }
                            else
                            {
                                techMetric.ART = 0.00D;
                            }

                            techMetric.UserId = userDetail.UserId;
                            techMetric.UserName = userDetail.UserName;
                            techMetric.HashedPassword = userDetail.HashedPassword;
                            techMetric.FirstName = userDetail.FirstName;
                            techMetric.MiddleName = userDetail.MiddleName;
                            techMetric.LastName = userDetail.LastName;
                            techMetric.Email = userDetail.Email;
                            techMetric.Phone = userDetail.Phone;
                            techMetric.CreateDate = userDetail.CreateDate;
                            techMetric.EditDate = userDetail.EditDate;

                            //Adding individual techMetrics to List object
                            techMetricsList.Add(techMetric);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in GetTechMetrics.  Exception: {0}", ex.Message));
                return techMetricsList = null;
            }
            return techMetricsList;
        }
        #endregion Tech Performance Methods

        #region Like/Unlike Methods
        /// <summary>
        /// List<LikeUnlike> GetLikesAndUnlikesForTech(Guid techId, string startDate, string endDate)
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<LikeUnlike> GetLikesAndUnlikesForTech(Guid techId, string startDate, string endDate)
        {
            // Get list of LikesAndUnlikes entries based on solutionId, StartDate, EndDate
            List<LikeUnlike> likeUnlikeList = new List<LikeUnlike>();

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var likeUnlikes = svcPlsContext.LikeUnlikes.Where(t => t.UserId == techId &&
                                                                         Convert.ToDateTime(t.CreateDate) >= Convert.ToDateTime(startDate) &&
                                                                         Convert.ToDateTime(t.CreateDate) <= Convert.ToDateTime(endDate)).OrderBy(t => Convert.ToDateTime(t.CreateDate));
                    likeUnlikeList = likeUnlikes.ToList<LikeUnlike>();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in GetLikesAndUnlikesForTech  Exception: {0}", ex.Message));

                likeUnlikeList = null;
            }

            return likeUnlikeList;
        }

        /// <summary>
        /// List<LikeUnlike> GetLikesAndUnlikesByTechAndSolution(Guid techId, Guid solutionId)
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public List<LikeUnlike> GetLikesAndUnlikesByTechAndSolution(Guid techId, Guid solutionId)
        {
            // Get list of LikesAndUnlikes entries based on solutionId & TechId
            List<LikeUnlike> likeUnlikeList = new List<LikeUnlike>();

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var likeUnlikes = svcPlsContext.LikeUnlikes.Where(t => t.UserId == techId &&
                                                                         t.SolutionId == solutionId).OrderBy(t => t.CreateDate);
                    likeUnlikeList = likeUnlikes.ToList<LikeUnlike>();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in GetLikesAndUnlikesByTechAndUser  Exception: {0}", ex.Message));

                likeUnlikeList = null;
            }

            return likeUnlikeList;
        }

        /// <summary>
        /// List<LikeUnlike> GetLikesAndUnlikesForSolution(Guid solutionId, string startDate, string endDate)
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<LikeUnlike> GetLikesAndUnlikesForSolution(Guid solutionId, string startDate, string endDate)
        {
            // Get list of LikesAndUnlikes entries based on solutionId, StartDate, EndDate
            List<LikeUnlike> likeUnlikeList = new List<LikeUnlike>();

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var likeUnlikes = svcPlsContext.LikeUnlikes.Where(t => t.SolutionId == solutionId &&
                                                                         Convert.ToDateTime(t.CreateDate) >= Convert.ToDateTime(startDate) &&
                                                                         Convert.ToDateTime(t.CreateDate) <= Convert.ToDateTime(endDate)).OrderBy(t => Convert.ToDateTime(t.CreateDate));
                    likeUnlikeList = likeUnlikes.ToList<LikeUnlike>();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in GetLikesAndUnlikesForSolution  Exception: {0}", ex.Message));

                likeUnlikeList = null;
            }

            return likeUnlikeList;
        }

        /// <summary>
        /// LikeUnlike AddLikeUnlike(LikeUnlike likeUnlike)
        /// </summary>
        /// <param name="likeUnlike"></param>
        /// <returns></returns>
        public LikeUnlike AddLikeUnlike(LikeUnlike likeUnlike)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Checking condition whether Solution and User already have like
                    var likeUnlikelist = (from t in svcPlsContext.LikeUnlikes
                                          where t.SolutionId == likeUnlike.SolutionId && t.UserId == likeUnlike.UserId
                                          select t).FirstOrDefault();

                    //If not exists in likeUnlike table then we are inserting
                    if (likeUnlikelist == null)
                    {
                        svcPlsContext.LikeUnlikes.InsertOnSubmit(likeUnlike);
                        svcPlsContext.SubmitChanges();
                    }
                    else
                    {
                        likeUnlikelist.SolutionId = likeUnlike.SolutionId;
                        if (likeUnlike.Like == 1)
                        {
                            likeUnlikelist.Like = likeUnlikelist.Like + 1;
                        }
                        else
                        {
                            if (likeUnlikelist.Unlike != true)
                            {
                                likeUnlikelist.Unlike = likeUnlike.Unlike;
                            }
                        }


                        likeUnlikelist.UserId = likeUnlike.UserId;
                        //likeUnlikelist.CreateDate = likeUnlike.CreateDate;
                        likeUnlikelist.EditDate = likeUnlike.EditDate;

                        //If exists in likeUnlike table then we are updating the record
                        svcPlsContext.SubmitChanges();
                    }

                    if (likeUnlike.Like == 0) //&& likeUnlikelist.Unlike != true
                    {
                        //if (likeUnlike.Unlike == true)
                        //{
                        //get the like count value count from Solutions and updating the count
                        var solution = (from sol in svcPlsContext.Solutions
                                        where sol.SolutionId == likeUnlike.SolutionId
                                        select sol).FirstOrDefault();

                        solution.UnlikeCount = solution.UnlikeCount + 1;
                        solution.EditDate = likeUnlike.EditDate;
                        svcPlsContext.SubmitChanges();
                    }
                    else
                    {
                        //get the unLike count value count from Solutions and updating the count
                        var solution = (from sol in svcPlsContext.Solutions
                                        where sol.SolutionId == likeUnlike.SolutionId
                                        select sol).FirstOrDefault();

                        solution.LikeCount = solution.LikeCount + 1;
                        solution.EditDate = likeUnlike.EditDate;
                        svcPlsContext.SubmitChanges();
                    }



                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddLikeUnlike().  Error: {0}", ex.Message);

                likeUnlike = null;
            }

            return likeUnlike;
        }

        /// <summary>
        /// bool DeleteLikeUnlike(Guid likeUnlikeId)
        /// </summary>
        /// <param name="likeUnlikeId"></param>
        /// <returns></returns>
        public bool DeleteLikeUnlike(Guid likeUnlikeId)
        {
            bool result = false;
            LikeUnlike likeUnlike = null;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific LikeUnlike object from Database
                    likeUnlike = (from t in svcPlsContext.LikeUnlikes
                                  where t.LikeUnlikeId == likeUnlikeId
                                  select t).FirstOrDefault();

                    if (likeUnlike != null)
                    {
                        // Delete the LikeUnlike object
                        svcPlsContext.LikeUnlikes.DeleteOnSubmit(likeUnlike);

                        // Save to database
                        svcPlsContext.SubmitChanges();

                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteLikeUnlike().  Error: {0}", ex.Message);

                result = false;
            }

            return result;
        }

        /// <summary>
        /// LikeUnlike UpdateLikeUnlike(LikeUnlike likeUnlike, Guid likeUnlikeId)
        /// </summary>
        /// <param name="likeUnlike"></param>
        /// <param name="likeUnlikeId"></param>
        /// <returns></returns>
        public LikeUnlike UpdateLikeUnlike(LikeUnlike likeUnlike, Guid likeUnlikeId)
        {
            LikeUnlike likeUnlikeDetail = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific LikeUnlike from Database
                    likeUnlikeDetail = (from t in svcPlsContext.LikeUnlikes
                                        where t.LikeUnlikeId == likeUnlikeId
                                        select t).FirstOrDefault();

                    if (likeUnlikeDetail != null)
                    {
                        likeUnlikeDetail.SolutionId = likeUnlike.SolutionId;
                        likeUnlikeDetail.Like = likeUnlike.Like;
                        likeUnlikeDetail.Unlike = likeUnlike.Unlike;
                        likeUnlikeDetail.UserId = likeUnlike.UserId;
                        //likeUnlikeDetail.CreateDate = likeUnlike.CreateDate;
                        likeUnlikeDetail.EditDate = likeUnlike.EditDate;

                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateLikeUnlike().  Error: {0}", ex.Message);

                likeUnlikeDetail = null;
            }

            return likeUnlikeDetail;
        }

        #endregion Like/Unlike Methods

        #region Daily Recap Methods
        /// <summary>
        /// GetDailyRecapByTech(Guid techId, string startDate, string endDate)
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<DailyRecap> GetDailyRecapByTech(Guid techId, string startDate, string endDate)
        {
            // Get list of DailyRecap entries based on TechId, StartDate, EndDate
            List<DailyRecap> dailyRecapList = new List<DailyRecap>();

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var dailyRecaps = svcPlsContext.DailyRecaps.Where(t => t.TechId == techId &&
                                                                         Convert.ToDateTime(t.TimeStamp) >= Convert.ToDateTime(startDate) &&
                                                                         Convert.ToDateTime(t.TimeStamp) <= Convert.ToDateTime(endDate)).OrderBy(t => Convert.ToDateTime(t.TimeStamp));
                    dailyRecapList = dailyRecaps.ToList<DailyRecap>();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in GetDailyRecapByTech  Exception: {0}", ex.Message));

                dailyRecapList = null;
            }

            return dailyRecapList;
        }

        /// <summary>
        /// List<DailyRecap> AddDailyRecap(List<DailyRecap> dailyRecapList)
        /// </summary>
        /// <param name="dailyRecapList"></param>
        /// <returns></returns>
        public List<DailyRecap> AddDailyRecap(List<DailyRecap> dailyRecapList)
        {
            // Add list of DailyRecap entries to DB

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    foreach (DailyRecap dailyRecap in dailyRecapList)
                    {
                        svcPlsContext.DailyRecaps.InsertOnSubmit(dailyRecap);

                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddDailyRecap().  Error: {0}", ex.Message);

                dailyRecapList = null;
            }

            return dailyRecapList;

            // Generate report data

            // Send email to support@servicetrackingsystems.net

        }
        #endregion Daily Recap Methods

        #region RecapSetting Methods

        /// <summary>
        /// string AddRecapSetting(RecapSetting recapSetting)
        /// </summary>
        /// <param name="recapSetting"></param>
        /// <returns></returns>
        public string AddRecapSetting(RecapSetting recapSetting)
        {
            string message = string.Empty;

            // Add RecapSetting Informations into DB
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.RecapSettings.InsertOnSubmit(recapSetting);
                    svcPlsContext.SubmitChanges();
                    message = "New RecapSetting Added Successfully";
                }
            }

            catch (Exception ex)
            {
                Logger.Write("Error in AddRecapSetting().  Error: {0}", ex.Message);

                recapSetting = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "AddRecapSetting";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();
                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }

                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }


        /// <summary>
        /// string UpdateRecapSetting(RecapSetting recapSetting, Guid recapSettingId)
        /// </summary>
        /// <param name="recapSetting"></param>
        /// <param name="recapSettingId"></param>
        /// <returns></returns>
        public string UpdateRecapSetting(RecapSetting recapSetting, Guid recapSettingId)
        {
            string message = string.Empty;
            RecapSetting recapSettingList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific RecapSetting from Database
                    recapSettingList = (from c in svcPlsContext.RecapSettings
                                        where c.RecapSettingId == recapSettingId
                                        select c).FirstOrDefault();

                    if (recapSettingList != null)
                    {
                        //Delete RecapSetting details in RecapSettingDay table
                        var delRecapSettingDay = (from c in svcPlsContext.RecapSettingDays
                                                  where c.RecapSettingId == recapSettingId
                                                  select c);

                        if (delRecapSettingDay != null)
                        {

                            // Delete the RecapSettingDay details based on recapSettingId
                            svcPlsContext.RecapSettingDays.DeleteAllOnSubmit(delRecapSettingDay.ToList<RecapSettingDay>());
                            svcPlsContext.SubmitChanges();
                        }


                        //Delete RecapSetting details in RecapSettingLocation table
                        var delRecapSettingLocation = (from c in svcPlsContext.RecapSettingLocations
                                                       where c.RecapSettingId == recapSettingId
                                                       select c);

                        if (delRecapSettingLocation != null)
                        {
                            // Delete the RecapSettingLocation details based on recapSettingId
                            svcPlsContext.RecapSettingLocations.DeleteAllOnSubmit(delRecapSettingLocation.ToList<RecapSettingLocation>());
                            svcPlsContext.SubmitChanges();
                        }

                        //Delete RecapSetting details in RecapSettingCategory table
                        var delRecapSettingCategory = (from c in svcPlsContext.RecapSettingCategories
                                                       where c.RecapSettingId == recapSettingId
                                                       select c);

                        if (delRecapSettingCategory != null)
                        {
                            // Delete the RecapSettingCategory details based on recapSettingId
                            svcPlsContext.RecapSettingCategories.DeleteAllOnSubmit(delRecapSettingCategory.ToList<RecapSettingCategory>());
                            svcPlsContext.SubmitChanges();
                        }

                        //Delete RecapSetting details in RecapSettingUser table
                        var delRecapSettingUser = (from c in svcPlsContext.RecapSettingUsers
                                                   where c.RecapSettingId == recapSettingId
                                                   select c);

                        if (delRecapSettingUser != null)
                        {
                            // Delete the RecapSettingUser details based on recapSettingId
                            svcPlsContext.RecapSettingUsers.DeleteAllOnSubmit(delRecapSettingUser.ToList<RecapSettingUser>());
                            svcPlsContext.SubmitChanges();
                        }

                        recapSettingList.Name = recapSetting.Name;
                        recapSettingList.BroadcastTime = recapSetting.BroadcastTime;
                        recapSettingList.StartTime = recapSetting.StartTime;
                        recapSettingList.EndTime = recapSetting.EndTime;
                        recapSettingList.RecapMail = recapSetting.RecapMail;
                        recapSettingList.TimeZone = recapSetting.TimeZone;
                        recapSettingList.Active = recapSetting.Active;
                        recapSettingList.RecapSettingLocations = recapSetting.RecapSettingLocations;
                        recapSettingList.RecapSettingDays = recapSetting.RecapSettingDays;
                        recapSettingList.RecapSettingCategories = recapSetting.RecapSettingCategories;
                        recapSettingList.RecapSettingUsers = recapSetting.RecapSettingUsers;
                        recapSettingList.EditDate = recapSetting.EditDate;

                        //Saving changes to DB
                        svcPlsContext.SubmitChanges();
                        message = "Selected RecapSetting Updated Successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateRecapSetting().  Error: {0}", ex.Message);

                recapSettingList = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "UpdateRecapSetting";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();
                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }

                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }


        /// <summary>
        /// string DeleteRecapSetting(Guid recapSettingId)
        /// </summary>
        /// <param name="recapSettingId"></param>
        /// <returns></returns>
        public string DeleteRecapSetting(Guid recapSettingId)
        {
            RecapSetting recapSetting = null;
            string message = string.Empty;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific RecapSettings object from Database
                    recapSetting = (from c in svcPlsContext.RecapSettings
                                    where c.RecapSettingId == recapSettingId
                                    select c).FirstOrDefault();

                    if (recapSetting != null)
                    {
                        //Delete RecapSetting details in RecapSettingDay table
                        var delRecapSettingDay = (from c in svcPlsContext.RecapSettingDays
                                                  where c.RecapSettingId == recapSettingId
                                                  select c);

                        if (delRecapSettingDay != null)
                        {
                            // Delete the RecapSettingDay details based on recapSettingId
                            svcPlsContext.RecapSettingDays.DeleteAllOnSubmit(delRecapSettingDay.ToList<RecapSettingDay>());
                        }

                        //Delete RecapSetting details in RecapSettingLocation table
                        var delRecapSettingLocation = (from c in svcPlsContext.RecapSettingLocations
                                                       where c.RecapSettingId == recapSettingId
                                                       select c);

                        if (delRecapSettingLocation != null)
                        {
                            // Delete the RecapSettingLocation details based on recapSettingId
                            svcPlsContext.RecapSettingLocations.DeleteAllOnSubmit(delRecapSettingLocation.ToList<RecapSettingLocation>());
                        }

                        //Delete RecapSetting details in RecapSettingCategory table
                        var delRecapSettingCategory = (from c in svcPlsContext.RecapSettingCategories
                                                       where c.RecapSettingId == recapSettingId
                                                       select c);

                        if (delRecapSettingCategory != null)
                        {
                            // Delete the RecapSettingCategory details based on recapSettingId
                            svcPlsContext.RecapSettingCategories.DeleteAllOnSubmit(delRecapSettingCategory.ToList<RecapSettingCategory>());
                            svcPlsContext.SubmitChanges();
                        }

                        //Delete RecapSetting details in RecapSettingUser table
                        var delRecapSettingUser = (from c in svcPlsContext.RecapSettingUsers
                                                   where c.RecapSettingId == recapSettingId
                                                   select c);

                        if (delRecapSettingUser != null)
                        {
                            // Delete the RecapSettingUser details based on recapSettingId
                            svcPlsContext.RecapSettingUsers.DeleteAllOnSubmit(delRecapSettingUser.ToList<RecapSettingUser>());
                            svcPlsContext.SubmitChanges();
                        }

                        // Delete the RecapSettings object
                        svcPlsContext.RecapSettings.DeleteOnSubmit(recapSetting);

                        // Save to database
                        svcPlsContext.SubmitChanges();
                        message = "Selected RecapSetting Deleted Successfully";
                    }
                    //else
                    //{
                    //    result = false;
                    //}
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteRecapSetting().  Error: {0}", ex.Message);

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "DeleteRecapSetting";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();
                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }

                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, ashok@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return message;
        }


        /// <summary>
        /// RecapSetting GetRecapSetting(Guid recapSettingId)
        /// </summary>
        /// <param name="recapSettingId"></param>
        /// <returns></returns>
        public RecapSetting GetRecapSetting(Guid recapSettingId)
        {
            RecapSetting recapSetting = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var recapsettings = svcPlsContext.RecapSettings.Where(c => c.RecapSettingId == recapSettingId);

                    var getRecapSettingDay = (from c in svcPlsContext.RecapSettingDays
                                              where c.RecapSettingId == recapSettingId
                                              select c);

                    var getRecapSettingLocation = (from c in svcPlsContext.RecapSettingLocations
                                                   where c.RecapSettingId == recapSettingId
                                                   select c);

                    var getRecapSettingUser = (from c in svcPlsContext.RecapSettingUsers
                                               where c.RecapSettingId == recapSettingId
                                               select c);

                    var getRecapSettingCategory = (from c in svcPlsContext.RecapSettingCategories
                                                   where c.RecapSettingId == recapSettingId
                                                   select c);

                    if (recapsettings != null)
                    {
                        recapSetting = recapsettings.FirstOrDefault();
                        recapSetting.RecapSettingDays.Assign(getRecapSettingDay);
                        recapSetting.RecapSettingLocations.Assign(getRecapSettingLocation);
                        recapSetting.RecapSettingUsers.Assign(getRecapSettingUser);
                        recapSetting.RecapSettingCategories.Assign(getRecapSettingCategory);

                        recapSetting.RecapSettingDays.OrderBy(r => r.RecapSettingDayId);
                        recapSetting.RecapSettingUsers.OrderBy(r => r.RecapSettingUserId);
                        recapSetting.RecapSettingCategories.OrderBy(r => r.RecapSettingCategoryId);
                        recapSetting.RecapSettingLocations.OrderBy(r => r.RecapSettingLocationId);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetRecapSetting(Guid recapSettingId).  Error: {0}", ex.Message);

                recapSetting = null;
            }

            return recapSetting;
        }


        /// <summary>
        /// List<RecapSetting> GetRecapSettings()
        /// </summary>
        /// <returns></returns>
        public List<RecapSetting> GetRecapSettings()
        {
            List<RecapSetting> recapSettingList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var recapSettings = svcPlsContext.RecapSettings.OrderByDescending(s => s.CreateDate);

                    if (recapSettings != null)
                    {
                        recapSettingList = recapSettings.ToList<RecapSetting>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetRecapSettings().  Error: {0}", ex.Message);

                recapSettingList = null;
            }

            return recapSettingList;
        }

        #endregion RecapSetting Methods

        #region SnoozedTicket Methods
        /// <summary>
        /// SnoozedTicket AddSnoozedTicket(SnoozedTicket snoozedTicket)
        /// </summary>
        /// <param name="snoozedTicket"></param>
        /// <returns></returns>
        public SnoozedTicket AddSnoozedTicket(SnoozedTicket snoozedTicket)
        {
            // Add SnoozedTicket Informations into DB
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.SnoozedTickets.InsertOnSubmit(snoozedTicket);
                    svcPlsContext.SubmitChanges();
                }
            }

            catch (Exception ex)
            {
                Logger.Write("Error in AddSnoozedTicket().  Error: {0}", ex.Message);

                snoozedTicket = null;
            }

            return snoozedTicket;
        }


        /// <summary>
        /// List<SnoozedTicket> GetSnoozedTickets()
        /// </summary>
        /// <returns></returns>
        public List<SnoozedTicket> GetSnoozedTickets()
        {
            List<SnoozedTicket> snoozedTicketList = null;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var snoozedTickets = svcPlsContext.SnoozedTickets.OrderByDescending(s => s.CreateDate);

                    if (snoozedTickets != null)
                    {
                        snoozedTicketList = snoozedTickets.ToList<SnoozedTicket>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetSnoozedTickets().  Error: {0}", ex.Message);

                snoozedTicketList = null;
            }

            return snoozedTicketList;
        }


        /// <summary>
        /// SnoozedTicket UpdateSnoozedTicket(SnoozedTicket snoozedTicket, Guid snoozedTicketId)
        /// </summary>
        /// <param name="snoozedTicket"></param>
        /// <param name="snoozedTicketId"></param>
        /// <returns></returns>
        public SnoozedTicket UpdateSnoozedTicket(SnoozedTicket snoozedTicket, Guid snoozedTicketId)
        {
            SnoozedTicket snoozedTicketDetail = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific SnoozedTicket from Database
                    snoozedTicketDetail = (from t in svcPlsContext.SnoozedTickets
                                           where t.SnoozedTicketId == snoozedTicketId
                                           select t).FirstOrDefault();

                    if (snoozedTicketDetail != null)
                    {
                        snoozedTicketDetail.SnoozeId = snoozedTicket.SnoozeId;
                        snoozedTicketDetail.TicketId = snoozedTicket.TicketId;
                        //snoozedTicketDetail.CreateDate = snoozedTicket.CreateDate;
                        snoozedTicketDetail.EditDate = snoozedTicket.EditDate;

                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateSnoozedTicket().  Error: {0}", ex.Message);

                snoozedTicketDetail = null;
            }

            return snoozedTicketDetail;
        }

        /// <summary>
        /// bool DeleteSnoozedTicket(Guid ticketId)
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        public bool DeleteSnoozedTicket(Guid ticketId)
        {
            bool result = false;
            SnoozedTicket snoozedTicket = null;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific SnoozedTicket object from Database
                    snoozedTicket = (from s in svcPlsContext.SnoozedTickets
                                     where s.TicketId == ticketId
                                     select s).FirstOrDefault();

                    if (snoozedTicket != null)
                    {
                        // Delete the SnoozedTicket object
                        svcPlsContext.SnoozedTickets.DeleteOnSubmit(snoozedTicket);

                        // Save to database
                        svcPlsContext.SubmitChanges();

                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteSnoozedTicket().  Error: {0}", ex.Message);

                result = false;
            }

            return result;
        }


        /// <summary>
        /// SnoozedTicket GetSnoozedTicket(Guid snoozedTicketId)
        /// </summary>
        /// <param name="snoozedTicketId"></param>
        /// <returns></returns>
        public SnoozedTicket GetSnoozedTicket(Guid snoozedTicketId)
        {
            SnoozedTicket snoozedTicket = null;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var snoozedTickets = svcPlsContext.SnoozedTickets.Where(s => s.SnoozedTicketId == snoozedTicketId);

                    if (snoozedTickets != null)
                    {
                        snoozedTicket = snoozedTickets.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetSnoozedTicket().  Error: {0}", ex.Message);

                snoozedTicket = null;
            }

            return snoozedTicket;
        }

        #endregion SnoozedTicket Methods

        #region Email Body
        public void EmailBody(Guid ticketId, Guid snozId, bool isCreation)
        {
            EmailElements emailElements = new EmailElements();
            StringBuilder sbEmailBody = new StringBuilder();
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var emailContents = (from t in svcPlsContext.Tickets
                                         join u in svcPlsContext.Users on t.UserId equals u.UserId
                                         join c in svcPlsContext.Contacts on t.ContactId equals c.ContactId
                                         join l in svcPlsContext.Locations on t.LocationId equals l.LocationId
                                         where t.TicketId == ticketId
                                         select new
                                         {
                                             u.Email,
                                             u.UserName,
                                             t.TicketNum,
                                             t.TicketName,
                                             t.Status,
                                             t.CreateDate,
                                             t.ResponseTimeStamp,
                                             l.LocationName,
                                             c.ContactName,
                                             c.CallbackNumber
                                         }).FirstOrDefault();

                    if (emailContents != null)
                    {
                        StringCollection usrEmail = new StringCollection();
                        StringCollection usrEmailCC = new StringCollection();
                        StringCollection usrEmailBCC = new StringCollection();
                        usrEmail.Add(emailContents.Email);
                        emailElements.ToAddresses = usrEmail;

                        char[] separator = new char[] { ',' };
                        string emailCC = ConfigurationManager.AppSettings["CC"];
                        string[] strSplitArr = emailCC.Split(separator);
                        foreach (string arrStr in strSplitArr)
                        {
                            usrEmailCC.Add(arrStr);
                        }

                        string emailBCC = ConfigurationManager.AppSettings["BCC"];
                        string[] strSplitArrBcc = emailBCC.Split(separator);
                        foreach (string arrStr in strSplitArrBcc)
                        {
                            usrEmailBCC.Add(arrStr);
                        }
                        emailElements.BCCAddresses = usrEmailBCC;
                        emailElements.CCAddresses = usrEmailCC;

                        emailElements.FromAddress = "clienthappiness@servicetrackingsystems.net";
                        emailElements.SubjectText = "STS - ServiceTech - Snooze #" + snozId;
                        sbEmailBody.Append("Hey" + " " + emailContents.UserName);
                        sbEmailBody.Append("<br /><br />");
                        sbEmailBody.Append("Please accept this ticket once you're back.");
                        sbEmailBody.Append("<br /><br />");
                        sbEmailBody.Append("<table><tr>");
                        sbEmailBody.Append("<td>Ticket Number</td><td>:</td><td>");
                        sbEmailBody.Append(emailContents.TicketNum);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Ticket Description</td><td>:</td><td>");
                        sbEmailBody.Append(emailContents.TicketName);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Location</td><td>:</td><td>");
                        sbEmailBody.Append(emailContents.LocationName);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Contact</td><td>:</td><td>");
                        sbEmailBody.Append(emailContents.ContactName);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Call back Number</td><td>:</td><td>");
                        sbEmailBody.Append(emailContents.CallbackNumber);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Status</td><td>:</td><td>");
                        sbEmailBody.Append(emailContents.Status);
                        sbEmailBody.Append("</td></tr><tr>");
                        if (isCreation == false) //Add Ticket
                        {
                            sbEmailBody.Append("<td>CreateDate</td><td>:</td><td>");
                            sbEmailBody.Append(emailContents.CreateDate);
                        }
                        else //Assign Ticket
                        {
                            sbEmailBody.Append("<td>AssignDate</td><td>:</td><td>");
                            sbEmailBody.Append(emailContents.ResponseTimeStamp);
                        }
                        sbEmailBody.Append("</td>");
                        sbEmailBody.Append("</tr></table>");
                        sbEmailBody.Append("<br /><br />Thanks");
                        sbEmailBody.Append("<br /><b>Service Tracking Systems</b>");
                        sbEmailBody.Append("<br />clienthappiness@servicetrackingsystems.net");
                        //sbEmailBody.Append("<br />M: +173537383833");
                        emailElements.BodyText = sbEmailBody.ToString();

                        SendEmail(emailElements);
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in EmailBody().  Error: {0}", ex.Message);
            }
        }

        public void AssignEmailBody(Guid ticketId)
        {
            EmailElements emailElements = new EmailElements();
            StringBuilder sbEmailBody = new StringBuilder();
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var emailtick = (from t in svcPlsContext.Tickets
                                     join u in svcPlsContext.Users on t.UserId equals u.UserId
                                     join c in svcPlsContext.Contacts on t.ContactId equals c.ContactId
                                     join l in svcPlsContext.Locations on t.LocationId equals l.LocationId
                                     where t.TicketId == ticketId
                                     select new
                                     {
                                         u.Email,
                                         u.UserName,
                                         t.TicketNum,
                                         t.TicketName,
                                         t.Status,
                                         t.CreateDate,
                                         t.ResponseTimeStamp,
                                         t.Comments,
                                         l.LocationName,
                                         c.ContactName,
                                         c.CallbackNumber
                                     }).FirstOrDefault();


                    if (emailtick != null)
                    {
                        StringCollection usrEmail = new StringCollection();
                        StringCollection usrEmailCC = new StringCollection();
                        StringCollection usrEmailBCC = new StringCollection();
                        usrEmail.Add(emailtick.Email);
                        emailElements.ToAddresses = usrEmail;

                        char[] separator = new char[] { ',' };
                        string emailCC = ConfigurationManager.AppSettings["CC"];
                        string[] strSplitArr = emailCC.Split(separator);
                        foreach (string arrStr in strSplitArr)
                        {
                            usrEmailCC.Add(arrStr);
                        }

                        string emailBCC = ConfigurationManager.AppSettings["BCC"];
                        string[] strSplitArrBcc = emailBCC.Split(separator);
                        foreach (string arrStr in strSplitArrBcc)
                        {
                            usrEmailBCC.Add(arrStr);
                        }
                        emailElements.BCCAddresses = usrEmailBCC;
                        emailElements.CCAddresses = usrEmailCC;
                        emailElements.FromAddress = "clienthappiness@servicetrackingsystems.net";
                        emailElements.SubjectText = "STS - ServiceTech - Assigned";
                        sbEmailBody.Append("Hey" + " " + emailtick.UserName);
                        sbEmailBody.Append("<br /><br />");
                        sbEmailBody.Append("The following ticket is assigned to you, please check it out.");
                        sbEmailBody.Append("<br /><br />");
                        sbEmailBody.Append("<table><tr>");
                        sbEmailBody.Append("<td>Ticket Number</td><td>:</td><td>");
                        sbEmailBody.Append(emailtick.TicketNum);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Ticket Description</td><td>:</td><td>");
                        sbEmailBody.Append(emailtick.TicketName);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Location</td><td>:</td><td>");
                        sbEmailBody.Append(emailtick.LocationName);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Contact</td><td>:</td><td>");
                        sbEmailBody.Append(emailtick.ContactName);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Call back Number</td><td>:</td><td>");
                        sbEmailBody.Append(emailtick.CallbackNumber);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Status</td><td>:</td><td>");
                        sbEmailBody.Append(emailtick.Status);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Comments</td><td>:</td><td>");
                        sbEmailBody.Append(emailtick.Comments);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>AssignDate</td><td>:</td><td>");
                        sbEmailBody.Append(emailtick.ResponseTimeStamp);
                        sbEmailBody.Append("</td>");
                        sbEmailBody.Append("</tr></table>");

                        sbEmailBody.Append("<br /><br />Thanks");
                        sbEmailBody.Append("<br /><b>Service Tracking Systems</b>");
                        sbEmailBody.Append("<br />clienthappiness@servicetrackingsystems.net");
                        //sbEmailBody.Append("<br />M: +173537383833");
                        emailElements.BodyText = sbEmailBody.ToString();

                        SendEmail(emailElements);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AssignEmailBody().  Error: {0}", ex.Message);
            }
        }

        public void NewLocationEmail(string locationName, string address, string city, string state)
        {
            EmailElements emailElements = new EmailElements();
            StringBuilder sbEmailBody = new StringBuilder();
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    StringCollection usrEmail = new StringCollection();
                    StringCollection usrEmailCC = new StringCollection();
                    StringCollection usrEmailBCC = new StringCollection();
                    usrEmail.Add("support@servicetrackingsystems.net");
                    emailElements.ToAddresses = usrEmail;

                    char[] separator = new char[] { ',' };
                    string emailCC = ConfigurationManager.AppSettings["CC"];
                    string[] strSplitArr = emailCC.Split(separator);
                    foreach (string arrStr in strSplitArr)
                    {
                        usrEmailCC.Add(arrStr);
                    }

                    string emailBCC = ConfigurationManager.AppSettings["BCC"];
                    string[] strSplitArrBcc = emailBCC.Split(separator);
                    foreach (string arrStr in strSplitArrBcc)
                    {
                        usrEmailBCC.Add(arrStr);
                    }
                    emailElements.BCCAddresses = usrEmailBCC;
                    emailElements.CCAddresses = usrEmailCC;
                    emailElements.FromAddress = "clienthappiness@servicetrackingsystems.net";
                    emailElements.SubjectText = "STS - ServiceTech - New Location added";
                    sbEmailBody.Append("New Location has been added into the System");
                    sbEmailBody.Append("<br /><br />");
                    sbEmailBody.Append("<table><tr>");
                    sbEmailBody.Append("<td>Location</td><td>:</td><td>");
                    sbEmailBody.Append(locationName);
                    sbEmailBody.Append("</td></tr><tr>");
                    sbEmailBody.Append("<td>Address</td><td>:</td><td>");
                    sbEmailBody.Append(address);
                    sbEmailBody.Append("</td></tr><tr>");
                    sbEmailBody.Append("<td>City</td><td>:</td><td>");
                    sbEmailBody.Append(city);
                    sbEmailBody.Append("</td></tr><tr>");
                    sbEmailBody.Append("<td>State</td><td>:</td><td>");
                    sbEmailBody.Append(state);
                    sbEmailBody.Append("</td>");
                    sbEmailBody.Append("</tr></table>");

                    sbEmailBody.Append("<br /><br />Thanks");
                    sbEmailBody.Append("<br /><b>Service Tracking Systems</b>");
                    sbEmailBody.Append("<br />clienthappiness@servicetrackingsystems.net");

                    //sbEmailBody.Append("<br />M: +173537383833");
                    emailElements.BodyText = sbEmailBody.ToString();

                    SendEmail(emailElements);
                }

            }
            catch (Exception ex)
            {
                Logger.Write("Error in EmailBody().  Error: {0}", ex.Message);
            }
        }

        public void NewTicketEmail(string email, int ticNumber, string name)
        {
            EmailElements emailElements = new EmailElements();
            StringBuilder sbEmailBody = new StringBuilder();
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    StringCollection usrEmail = new StringCollection();
                    StringCollection usrEmailCC = new StringCollection();
                    StringCollection usrEmailBCC = new StringCollection();
                    usrEmail.Add(email);
                    emailElements.ToAddresses = usrEmail;

                    char[] separator = new char[] { ',' };
                    string emailCC = ConfigurationManager.AppSettings["CC"];
                    string[] strSplitArr = emailCC.Split(separator);
                    foreach (string arrStr in strSplitArr)
                    {
                        usrEmailCC.Add(arrStr);
                    }
                    string emailBCC = ConfigurationManager.AppSettings["BCC"];
                    string[] strSplitArrBcc = emailBCC.Split(separator);
                    foreach (string arrStr in strSplitArrBcc)
                    {
                        usrEmailBCC.Add(arrStr);
                    }
                    emailElements.BCCAddresses = usrEmailBCC;
                    emailElements.CCAddresses = usrEmailCC;
                    emailElements.FromAddress = "clienthappiness@servicetrackingsystems.net";
                    emailElements.SubjectText = "STS - ServiceTech - Support Ticket #" + ticNumber;
                    sbEmailBody.Append("Dear" + " " + name);
                    sbEmailBody.Append("<br /><br />");
                    sbEmailBody.Append(" Thank you for contacting our support department. We have opened a ticket with the reference #" + ticNumber + " and will be in touch shortly to resolve it for you");
                    sbEmailBody.Append("<br /><br /><br />Thanks");
                    sbEmailBody.Append("<br /><b>Service Tracking Systems</b>");
                    sbEmailBody.Append("<br />clienthappiness@servicetrackingsystems.net");
                    //sbEmailBody.Append("<br />M: +173537383833");
                    emailElements.BodyText = sbEmailBody.ToString();
                    SendEmail(emailElements);
                }

            }
            catch (Exception ex)
            {
                Logger.Write("Error in EmailBody().  Error: {0}", ex.Message);
            }
        }

        public void ClosedTicketEmail(Guid ticketId)
        {
            EmailElements emailElements = new EmailElements();
            StringBuilder sbEmailBody = new StringBuilder();
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var emailContents = (from t in svcPlsContext.Tickets
                                         join u in svcPlsContext.Users on t.UserId equals u.UserId
                                         join c in svcPlsContext.Contacts on t.ContactId equals c.ContactId
                                         join l in svcPlsContext.Locations on t.LocationId equals l.LocationId
                                         where t.TicketId == ticketId
                                         select new
                                         {
                                             u.Email,
                                             u.UserName,
                                             t.TicketNum,
                                             t.TicketName,
                                             t.Status,
                                             t.CreateDate,
                                             t.ResponseTimeStamp,
                                             l.LocationName,
                                             c.ContactName,
                                             c.CallbackNumber
                                         }).FirstOrDefault();

                    if (emailContents!=null)
                    {
                        StringCollection usrEmail = new StringCollection();
                        StringCollection usrEmailCC = new StringCollection();
                        StringCollection usrEmailBCC = new StringCollection();
                        usrEmail.Add(emailContents.Email);
                        emailElements.ToAddresses = usrEmail;

                        char[] separator = new char[] { ',' };
                        string emailCC = ConfigurationManager.AppSettings["CC"];
                        string[] strSplitArr = emailCC.Split(separator);
                        foreach (string arrStr in strSplitArr)
                        {
                            usrEmailCC.Add(arrStr);
                        }

                        string emailBCC = ConfigurationManager.AppSettings["BCC"];
                        string[] strSplitArrBcc = emailBCC.Split(separator);
                        foreach (string arrStr in strSplitArrBcc)
                        {
                            usrEmailBCC.Add(arrStr);
                        }
                        emailElements.BCCAddresses = usrEmailBCC;
                        emailElements.CCAddresses = usrEmailCC;
                        emailElements.FromAddress = "clienthappiness@servicetrackingsystems.net";
                        emailElements.SubjectText = "STS - ServiceTech - Ticket #" + emailContents.TicketNum + " Closed.";

                        sbEmailBody.Append("Hey" + " " + emailContents.UserName);
                        sbEmailBody.Append("<br /><br />");
                        sbEmailBody.Append("The following ticket has been closed");
                        sbEmailBody.Append("<br /><br />");
                        sbEmailBody.Append("<table><tr>");
                        sbEmailBody.Append("<td>Ticket Number</td><td>:</td><td>");
                        sbEmailBody.Append(emailContents.TicketNum);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Ticket Description</td><td>:</td><td>");
                        sbEmailBody.Append(emailContents.TicketName);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Location</td><td>:</td><td>");
                        sbEmailBody.Append(emailContents.LocationName);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Contact</td><td>:</td><td>");
                        sbEmailBody.Append(emailContents.ContactName);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Call back Number</td><td>:</td><td>");
                        sbEmailBody.Append(emailContents.CallbackNumber);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("<td>Status</td><td>:</td><td>");
                        sbEmailBody.Append(emailContents.Status);
                        sbEmailBody.Append("</td></tr><tr>");
                        sbEmailBody.Append("</tr></table>");

                        sbEmailBody.Append("<br /><br />Thanks");
                        sbEmailBody.Append("<br /><b>Service Tracking Systems</b>");
                        sbEmailBody.Append("<br />clienthappiness@servicetrackingsystems.net");
                        //sbEmailBody.Append("<br />M: +173537383833");
                        emailElements.BodyText = sbEmailBody.ToString();
                        SendEmail(emailElements);
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Write("Error in ClosedTicketEmail().  Error: {0}", ex.Message);
            }
        }

        #endregion

        #region Error Methods
        /// <summary>
        /// ErrorMethod(string methodName, string expDetails)
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="expDetails"></param>
        /// <returns></returns>
        public void ErrorMethod(string methodName, string expDetails, string innerException)
        {
            ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext();
            ErrorDetail errDetail = new ErrorDetail();
            errDetail.ExceptionId = Guid.NewGuid();
            errDetail.ExceptionMessage = expDetails;
            errDetail.InnerException = innerException;
            errDetail.MethodName = methodName;
            errDetail.CreateDate = DateTime.Now;

            //Insert Error Message in Database
            svcPlsContext.ErrorDetails.InsertOnSubmit(errDetail);
            svcPlsContext.SubmitChanges();
        }

        /// <summary>
        /// List<ErrorDetail> ErrorDetails()
        /// </summary>
        /// <returns></returns>
        public List<ErrorDetail> ErrorDetails()
        {
            List<ErrorDetail> errors = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var errordetails = svcPlsContext.ErrorDetails.OrderByDescending(e => e.CreateDate);

                    if (errordetails != null)
                    {
                        errors = errordetails.ToList<ErrorDetail>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in ErrorDetails().  Error: {0}", ex.Message);

                errors = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "ErrorMethod";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

            }

            return errors;
        }


        /// <summary>
        ///  GetErrorDetail(string methodName, DateTime startDate, DateTime endDate)
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<ErrorDetail> GetErrorDetail(string methodName, DateTime startDate, DateTime endDate)
        {
            // Get list of ErrorDetail entries based on MethodName, StartDate, EndDate
            List<ErrorDetail> errorList = new List<ErrorDetail>();

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var errorDetails = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName &&
                                                                         e.CreateDate >= startDate &&
                                                                         e.CreateDate <= endDate).OrderByDescending(e => e.CreateDate);
                    errorList = errorDetails.ToList<ErrorDetail>();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in GetErrorDetail  Exception: {0}", ex.Message));

                errorList = null;

                //Pass error messages to ErrorMethod for save in Database
                //string methodName = "GetErrorDetail";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod("GetErrorDetail", expDetails, innerException);

            }

            return errorList;

        }

        /// <summary>
        /// bool DeleteErrorDetail(string methodName)
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public bool DeleteErrorDetail(string methodName)
        {
            bool result = false;
            ErrorDetail error = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific error from Database
                    error = (from e in svcPlsContext.ErrorDetails
                             where e.MethodName == methodName
                             select e).FirstOrDefault();

                    if (error != null)
                    {
                        // Delete the error
                        svcPlsContext.ErrorDetails.DeleteOnSubmit(error);

                        // Save to database
                        svcPlsContext.SubmitChanges();

                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in DeleteErrorDetails().  Error: {0}", ex.Message));

                result = false;

                //Pass error messages to ErrorMethod for save in Database
                //string methodName = "DeleteErrorDetail";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod("DeleteErrorDetail", expDetails, innerException);
            }

            return result;
        }

        #endregion Error Methods

        #region DeviceDetail Methods
        /// <summary>
        /// List<DeviceDetail> GetDeviceDetails()
        /// </summary>
        /// <returns></returns>
        public List<DeviceDetail> GetDeviceDetails()
        {
            List<DeviceDetail> deviceDetailList = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var devicedetails = svcPlsContext.DeviceDetails.OrderBy(d => d.CreateDate);

                    if (devicedetails != null)
                    {
                        deviceDetailList = devicedetails.ToList<DeviceDetail>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetDeviceDetails().  Error: {0}", ex.Message);

                deviceDetailList = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "GetDeviceDetails";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

            }

            return deviceDetailList;
        }
        /// <summary>
        /// DeviceDetail GetDeviceDetail(string deviceDetailId)
        /// </summary>
        /// <param name="deviceDetailId"></param>
        /// <returns></returns>
        public DeviceDetail GetDeviceDetail(string deviceDetailId)
        {
            DeviceDetail devicedetail = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var devicedetails = svcPlsContext.DeviceDetails.Where(d => d.DeviceDetailId == Guid.Parse(deviceDetailId));

                    if (devicedetails != null)
                    {
                        devicedetail = devicedetails.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetDeviceDetail(string deviceDetailId).  Error: {0}", ex.Message);

                devicedetail = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "GetDeviceDetail";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return devicedetail;
        }

        /// <summary>
        /// DeviceDetail AddDeviceDetail(DeviceDetail deviceDetail)
        /// </summary>
        /// <param name="deviceDetail"></param>
        /// <returns></returns>

        public DeviceDetail AddDeviceDetail(DeviceDetail deviceDetail)
        {
            //string message = string.Empty;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var devicedetails = svcPlsContext.DeviceDetails.Where(d => d.DeviceToken == deviceDetail.DeviceToken && d.UserId == deviceDetail.UserId);

                    if (devicedetails != null && devicedetails.Any())
                    {

                    }
                    else
                    {
                        svcPlsContext.DeviceDetails.InsertOnSubmit(deviceDetail);
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddDeviceDetail().  Error: {0}", ex.Message);


                deviceDetail = null;

                //Pass error messages to ErrorMethod for save in Database

                string methodName = "AddDeviceDetail";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

            }

            return deviceDetail;
        }
        /// <summary>
        /// DeviceDetail UpdateDeviceDetail(DeviceDetail deviceDetail, string deviceDetailId)
        /// </summary>
        /// <param name="deviceDetail"></param>
        /// <param name="deviceDetailId"></param>
        /// <returns></returns>
        public DeviceDetail UpdateDeviceDetail(DeviceDetail deviceDetail, Guid deviceDetailId)
        {
            DeviceDetail dev = null;
            //string message = string.Empty;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific DeviceDetails from Database
                    dev = (from d in svcPlsContext.DeviceDetails
                           where d.DeviceDetailId == deviceDetailId
                           select d).FirstOrDefault();

                    if (dev != null)
                    {
                        dev.UDID = deviceDetail.UDID;
                        dev.DeviceToken = deviceDetail.DeviceToken;
                        dev.EditDate = deviceDetail.EditDate;
                        //Save to database
                        svcPlsContext.SubmitChanges();
                        //message = "Selected DeviceDetails Updated Successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateDeviceDetail().  Error: {0}", ex.Message);

                dev = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "UpdateDeviceDetail";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

            }

            return deviceDetail;
        }
        /// <summary>
        /// bool DeleteDeviceDetail(Guid categoryId)
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>        

        public bool DeleteDeviceDetail(Guid deviceDetailId)
        {
            bool result = false;
            DeviceDetail dev = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific DeviceDetails from Database
                    dev = (from d in svcPlsContext.DeviceDetails
                           where d.DeviceDetailId == deviceDetailId
                           select d).FirstOrDefault();

                    if (dev != null)
                    {
                        // Delete the DeviceDetails
                        svcPlsContext.DeviceDetails.DeleteOnSubmit(dev);

                        // Save to database
                        svcPlsContext.SubmitChanges();

                        result = true;
                    }
                    else
                    {
                        result = false;
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteDeviceDetail().  Error: {0}", ex.Message);
                //Pass error messages to ErrorMethod for save in Database
                string methodName = "DeleteDeviceDetail";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);

            }

            return result;
        }

        #endregion DeviceDetail Methods

        #region UserRole Methods
        /// <summary>
        /// UserRole GetUserRole(Guid userRoleId)
        /// </summary>
        /// <param name="userRoleId"></param>
        /// <returns></returns>
        public UserRole GetUserRole(Guid userRoleId)
        {
            UserRole uRole = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var uRoles = svcPlsContext.UserRoles.Where(u => u.UserRoleId == userRoleId);

                    if (uRoles != null)
                    {
                        uRole = uRoles.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetUserRole(Guid userRoleId).  Error: {0}", ex.Message);

                uRole = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "GetUserRole";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return uRole;
        }
        /// <summary>
        /// UserRole AddUserRole(UserRole userRole)
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>
        public UserRole AddUserRole(UserRole userRole)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.UserRoles.InsertOnSubmit(userRole);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddUserRole().  Error: {0}", ex.Message);

                userRole = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "AddUserRole";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return userRole;
        }
        /// <summary>
        /// UserRole UpdateUserRole(UserRole userRole, Guid userRoleId)
        /// </summary>
        /// <param name="userRole"></param>
        /// <param name="userRoleId"></param>
        /// <returns></returns>
        public UserRole UpdateUserRole(UserRole userRole, Guid userRoleId)
        {
            UserRole userRoleDetail = null;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific UserRole from Database
                    userRoleDetail = (from u in svcPlsContext.UserRoles
                                      where u.UserRoleId == userRoleId
                                      select u).FirstOrDefault();

                    if (userRoleDetail != null)
                    {
                        userRoleDetail.UserId = userRole.UserId;
                        userRoleDetail.RoleId = userRole.RoleId;
                        userRoleDetail.EditDate = userRole.EditDate;
                        userRoleDetail.Role = userRole.Role;
                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateUserRole().  Error: {0}", ex.Message);

                userRoleDetail = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "UpdateUserRole";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return userRoleDetail;
        }

        /// <summary>
        /// bool DeleteUserRole(Guid userRoleId)
        /// </summary>
        /// <param name="userRoleId"></param>
        /// <returns></returns>
        public bool DeleteUserRole(Guid userRoleId)
        {
            bool result = false;
            UserRole userRole = null;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific UserRole object from Database
                    userRole = (from u in svcPlsContext.UserRoles
                                where u.UserRoleId == userRoleId
                                select u).FirstOrDefault();

                    if (userRole != null)
                    {
                        // Delete the UserRole object
                        svcPlsContext.UserRoles.DeleteOnSubmit(userRole);

                        // Save to database
                        svcPlsContext.SubmitChanges();

                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteUserRole().  Error: {0}", ex.Message);

                result = false;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "DeleteUserRole";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return result;
        }
        #endregion UserRole Methods

        #region SMS Methods
        /// <summary>
        /// void SendSMS(string ph, string message)
        /// </summary>
        /// <param name="ph"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public void SendSMS(string ph, string message)
        {
            string msg = string.Empty;
            try
            {

                string strurl = "https://www.primemessage.net/TxTNotify/TxTNotify?PhoneDestination=" + ph + "&Message=" + message + "&CustomerNickname=VALET&Username=valetplease&Password=vp0209";
                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(strurl, false));
                HttpWebResponse httpResponse = (HttpWebResponse)(httpReq.GetResponse());
                msg = httpResponse.StatusCode.ToString();

                //Response.Write("sms send successfully");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion SMS Methods

        #region Push Notification
        /// <summary>
        /// void PushNotificationMethod(string deviceToken, string message)
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public void PushNotificationMethod(string deviceToken, string message)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    PushService push = new PushService();
                    var appleCert = File.ReadAllBytes(HostingEnvironment.MapPath("~/Apple Development IOS Push Services- net.servicetrackingsystems.svctech.p12"));
                    string p12FilePassword = "sts2013";
                    push.StartApplePushService(new ApplePushChannelSettings(false, appleCert, p12FilePassword));

                    push.QueueNotification(NotificationFactory.Apple()
                        .ForDeviceToken(deviceToken)
                        .WithAlert(message)
                        .WithSound("default")
                        .WithBadge(0));

                    push.StopAllServices(true);
	
                }
            }
            catch (Exception ex)
            {
                string methodName = "PushNotificationMethod";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var error = svcPlsContext.ErrorDetails.Where(e => e.MethodName == methodName).OrderByDescending(e => e.CreateDate).FirstOrDefault();
                    if (error != null)
                    {
                        message = "Error Occured: " + error.ExceptionId;
                    }
                }

                SmtpClient mailClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add("kannappan@colanonline.com");
                string ccAddress = "karthik.krishna@colanonline.com, poomalai@colanonline.com, magesh.ponnusamy@colanonline.com";
                mail.CC.Add(ccAddress);
                mail.From = new MailAddress("clienthappiness@servicetrackingsystems.net");
                mail.Subject = "APNS Error Email";
                mail.Body = "Exception: " + ex.Message + "<br /><br />InnerException: " + ex.InnerException;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.exg6.exghost.com";
                smtp.Port = 2525;
                smtp.Credentials = new System.Net.NetworkCredential("clienthappiness@servicetrackingsystems.net", "ServIce!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
        }
        #endregion Push Notification

        #region Permission Methods
        /// <summary>
        /// List<Permission> GetPermissions()
        /// </summary>
        /// <returns></returns>
        public List<Permission> GetPermissions()
        {
            List<Permission> permissionList = null;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var permissions = svcPlsContext.Permissions.OrderBy(p => p.CreateDate);

                    if (permissions != null)
                    {
                        permissionList = permissions.ToList<Permission>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetPermissions().  Error: {0}\n{1}", ex.Message,
                             ex.InnerException != null ? ex.InnerException.Message : string.Empty));

                permissionList = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "GetPermissions";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return permissionList;
        }
        /// <summary>
        /// Permission GetPermission(Guid permissionId)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public Permission GetPermission(Guid permissionId)
        {
            Permission permission = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var permissions = svcPlsContext.Permissions.Where(p => p.PermissionId == permissionId);

                    if (permissions != null)
                    {
                        permission = permissions.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetPermission(Guid permissionId).  Error: {0}", ex.Message);

                permission = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "GetPermission";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return permission;
        }
        /// <summary>
        /// Permission AddPermission(Permission permission)
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public Permission AddPermission(Permission permission)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.Permissions.InsertOnSubmit(permission);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddPermission().  Error: {0}", ex.Message);

                permission = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "AddPermission";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return permission;
        }
        /// <summary>
        /// Permission UpdatePermission(Permission permission, Guid permissionId)
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public Permission UpdatePermission(Permission permission, Guid permissionId)
        {
            Permission permissionDetail = null;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific Permission from Database
                    permissionDetail = (from p in svcPlsContext.Permissions
                                        where p.PermissionId == permissionId
                                        select p).FirstOrDefault();

                    if (permissionDetail != null)
                    {

                        permissionDetail.Description = permission.Description;
                        permissionDetail.EditDate = permission.EditDate;
                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdatePermission().  Error: {0}", ex.Message);

                permissionDetail = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "UpdatePermission";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return permissionDetail;
        }

        /// <summary>
        /// bool DeletePermission(Guid permissionId)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public bool DeletePermission(Guid permissionId)
        {
            bool result = false;
            Permission permission = null;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific Permission object from Database
                    permission = (from p in svcPlsContext.Permissions
                                  where p.PermissionId == permissionId
                                  select p).FirstOrDefault();

                    if (permission != null)
                    {
                        // Delete the Permission object
                        svcPlsContext.Permissions.DeleteOnSubmit(permission);

                        // Save to database
                        svcPlsContext.SubmitChanges();

                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeletePermission().  Error: {0}", ex.Message);

                result = false;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "DeletePermission";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return result;
        }
        #endregion permission Methods

        #region RolePermission Methods
        /// <summary>
        /// List<RolePermission> GetRolePermissions()
        /// </summary>
        /// <returns></returns>
        public List<RolePermission> GetRolePermissions()
        {
            List<RolePermission> rolepermissionList = null;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var rolepermissions = svcPlsContext.RolePermissions.OrderBy(r => r.CreateDate);

                    if (rolepermissions != null)
                    {
                        rolepermissionList = rolepermissions.ToList<RolePermission>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetRolePermissions().  Error: {0}\n{1}", ex.Message,
                             ex.InnerException != null ? ex.InnerException.Message : string.Empty));

                rolepermissionList = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "GetRolePermissions";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return rolepermissionList;
        }

        /// <summary>
        /// RolePermission GetRolePermission(Guid rolepermissionId)
        /// </summary>
        /// <param name="rolepermissionId"></param>
        /// <returns></returns>
        public RolePermission GetRolePermission(Guid rolepermissionId)
        {
            RolePermission rolepermission = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var rolepermissions = svcPlsContext.RolePermissions.Where(r => r.RolePermissionId == rolepermissionId);

                    if (rolepermissions != null)
                    {
                        rolepermission = rolepermissions.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetRolePermission(Guid rolepermissionId).  Error: {0}", ex.Message);

                rolepermission = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "GetRolePermission";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return rolepermission;
        }
        /// <summary>
        /// RolePermission AddRolePermission(RolePermission rolepermission)
        /// </summary>
        /// <param name="rolepermission"></param>
        /// <returns></returns>
        public RolePermission AddRolePermission(RolePermission rolepermission)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.RolePermissions.InsertOnSubmit(rolepermission);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddRolePermission().  Error: {0}", ex.Message);

                rolepermission = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "AddRolePermission";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return rolepermission;
        }
        /// <summary>
        /// RolePermission UpdateRolePermission(RolePermission rolepermission, Guid rolepermissionId)
        /// </summary>
        /// <param name="rolepermission"></param>
        /// <param name="rolepermissionId"></param>
        /// <returns></returns>
        public RolePermission UpdateRolePermission(RolePermission rolepermission, Guid rolepermissionId)
        {
            RolePermission rolepermissionDetail = null;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific RolePermission from Database
                    rolepermissionDetail = (from r in svcPlsContext.RolePermissions
                                            where r.RolePermissionId == rolepermissionId
                                            select r).FirstOrDefault();

                    if (rolepermissionDetail != null)
                    {
                        rolepermissionDetail.RoleId = rolepermission.RoleId;
                        rolepermissionDetail.PermissionId = rolepermission.PermissionId;
                        rolepermissionDetail.EditDate = rolepermission.EditDate;
                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateRolePermission().  Error: {0}", ex.Message);

                rolepermissionDetail = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "UpdateRolePermission";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return rolepermissionDetail;
        }

        /// <summary>
        /// bool DeleteRolePermission(Guid rolepermissionId)
        /// </summary>
        /// <param name="rolepermissionId"></param>
        /// <returns></returns>
        public bool DeleteRolePermission(Guid rolepermissionId)
        {
            bool result = false;
            RolePermission rolepermission = null;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific RolePermission object from Database
                    rolepermission = (from r in svcPlsContext.RolePermissions
                                      where r.RolePermissionId == rolepermissionId
                                      select r).FirstOrDefault();

                    if (rolepermission != null)
                    {
                        // Delete the RolePermission object
                        svcPlsContext.RolePermissions.DeleteOnSubmit(rolepermission);

                        // Save to database
                        svcPlsContext.SubmitChanges();

                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteRolePermission().  Error: {0}", ex.Message);

                result = false;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "DeleteRolePermission";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return result;
        }
        #endregion RolePermission Methods

        #region RolePermission Detail

        /// <summary>
        /// List<RolePermissionDetail> GetRolePermissionDetail(Guid roleId)
        /// </summary>
        /// <returns></returns>
        public List<RolePermissionDetail> GetRolePermissionDetail(Guid roleId)
        {
            List<RolePermissionDetail> rplist = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var rpquery = svcPlsContext.RolePermissions.Where(rpt => rpt.RoleId == roleId);

                    if (rpquery != null && rpquery.Any())
                    {
                        var query = from permission in svcPlsContext.Permissions
                                    join rp in svcPlsContext.RolePermissions.Where(rpt => rpt.RoleId == roleId)
                                    on permission.PermissionId equals rp.PermissionId
                                    into p_rp
                                    from rolePermission in p_rp.DefaultIfEmpty()
                                    select new
                                    {
                                        PermissionId = permission.PermissionId,
                                        RoleId = roleId,
                                        Category = permission.Category,
                                        Description = permission.Description,
                                        CreateDate = permission.CreateDate,
                                        EditDate = permission.EditDate,
                                        Status = rolePermission.PermissionId == null ? false : true
                                    };

                        if (query != null)
                        {
                            rplist = query.ToNonAnonymousList(typeof(RolePermissionDetail)) as List<RolePermissionDetail>;
                        }
                    }
                    else
                    {
                        //var rolepermissions = svcPlsContext.RolePermissions.OrderBy(r => r.CreateDate);
                        var permission = from p in svcPlsContext.Permissions
                                         select new
                                         {
                                             PermissionId = p.PermissionId,
                                             RoleId = roleId,
                                             Category = p.Category,
                                             Description = p.Description,
                                             CreateDate = p.CreateDate,
                                             EditDate = p.EditDate,
                                             Status = false
                                         };

                        if (permission != null)
                        {                           
                            rplist = permission.ToNonAnonymousList(typeof(RolePermissionDetail)) as List<RolePermissionDetail>;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in GetRolePermissionDetail.  Exception: {0}", ex.Message));
                return rplist = null;
            }
            return rplist.OrderBy(t => t.Category).ThenBy(t => t.Description).ToList<RolePermissionDetail>();

        }

        /// <summary>
        /// bool UpdateRolePermissionDetail(List<RolePermissionDetail> rolePermissiondetail)
        /// </summary>       
        /// <param name="rolePermissiondetail"></param>
        /// <returns></returns>
        public bool UpdateRolePermissionDetail(List<RolePermissionDetail> rolePermissiondetail)
        {
            bool result = false;
            RolePermission rp = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    foreach (var rps in rolePermissiondetail)
                    {
                        if (rps.Status == true)
                        {
                            var rolepermission = (from rpermission in svcPlsContext.RolePermissions
                                                  where rpermission.PermissionId == rps.PermissionId &&
                                                  rpermission.RoleId == rps.RoleId
                                                  select rpermission).FirstOrDefault();

                            if (rolepermission == null)
                            {
                                rp = new RolePermission();
                                rp.RolePermissionId = Guid.NewGuid();
                                rp.RoleId = rps.RoleId;
                                rp.PermissionId = rps.PermissionId;
                                rp.CreateDate = rps.CreateDate;
                                rp.EditDate = rps.EditDate;

                                svcPlsContext.RolePermissions.InsertOnSubmit(rp);
                                svcPlsContext.SubmitChanges();
                            }
                            result = true;
                        }
                        else if (rps.Status == false)
                        {
                            var rolepermission = (from rpermission in svcPlsContext.RolePermissions
                                                  where rpermission.PermissionId == rps.PermissionId &&
                                                  rpermission.RoleId == rps.RoleId
                                                  select rpermission).FirstOrDefault();
                            if (rolepermission != null)
                            {
                                svcPlsContext.RolePermissions.DeleteOnSubmit(rolepermission);
                                svcPlsContext.SubmitChanges();
                                result = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Exception in UpdateRolePermissionDetail.  Exception: {0}", ex.Message));
                result = false;
            }
            return result;

        }



        #endregion RolePermission Detail

        #region TicketComment Methods
        /// <summary>
        /// List<TicketComment> GetTicketComments()
        /// </summary>
        /// <returns></returns>  
        public List<TicketComment> GetTicketComments()
        {
            List<TicketComment> ticketCommentList = null;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var ticketComments = svcPlsContext.TicketComments.OrderBy(t => t.CreateDate);

                    if (ticketComments != null)
                    {
                        ticketCommentList = ticketComments.ToList<TicketComment>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error in GetTicketComments().  Error: {0}\n{1}", ex.Message,
                             ex.InnerException != null ? ex.InnerException.Message : string.Empty));

                ticketCommentList = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "GetTicketComments";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return ticketCommentList;
        }

        ///// <summary>        
        ///// TicketComment GetTicketComment(Guid ticketCommentId)
        ///// </summary>
        ///// <param name="ticketCommentId"></param>
        ///// <returns></returns>
        //public TicketComment GetTicketComment(Guid ticketCommentId)
        //{
        //    TicketComment ticketComment = null;

        //    try
        //    {
        //        using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
        //        {
        //            var ticketComments = svcPlsContext.TicketComments.Where(t => t.TicketCommentId == ticketCommentId);

        //            if (ticketComments != null)
        //            {
        //                ticketComment = ticketComments.FirstOrDefault();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Write("Error in GetTicketComment(Guid ticketCommentId).  Error: {0}", ex.Message);

        //        ticketComment = null;

        //        //Pass error messages to ErrorMethod for save in Database
        //        string methodName = "GetTicketComment";
        //        string expDetails = ex.Message;
        //        string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
        //        ErrorMethod(methodName, expDetails, innerException);
        //    }

        //    return ticketComment;
        //}

        /// <summary>
        /// TicketComment AddTicketComment(TicketComment ticketComment)
        /// </summary>
        ///<param name="ticketComment"></param>
        /// <returns></returns>
        public TicketComment AddTicketComment(TicketComment ticketComment)
        {
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    svcPlsContext.TicketComments.InsertOnSubmit(ticketComment);

                    svcPlsContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in AddTicketComment().  Error: {0}", ex.Message);

                ticketComment = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "AddTicketComment";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return ticketComment;
        }

        /// <summary>
        /// TicketComment UpdateTicketComment(TicketComment ticketComment, Guid ticketCommentId)
        /// </summary>
        /// <param name="ticketComment"></param>
        /// <param name="ticketCommentId"></param>
        /// <returns></returns>
        public TicketComment UpdateTicketComment(TicketComment ticketComment, Guid ticketCommentId)
        {
            TicketComment ticketCommentDetail = null;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific TicketComment from Database
                    ticketCommentDetail = (from t in svcPlsContext.TicketComments
                                           where t.TicketCommentId == ticketCommentId
                                           select t).FirstOrDefault();

                    if (ticketCommentDetail != null)
                    {

                        ticketCommentDetail.TicketId = ticketComment.TicketId;
                        ticketCommentDetail.UserId = ticketComment.UserId;
                        ticketCommentDetail.Comment = ticketComment.Comment;
                        ticketCommentDetail.EditDate = ticketComment.EditDate;
                        //Save to database
                        svcPlsContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in UpdateTicketComment().  Error: {0}", ex.Message);

                ticketCommentDetail = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "UpdateTicketComment";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return ticketCommentDetail;
        }

        /// <summary>
        /// bool DeleteTicketComment(Guid ticketCommentId)
        /// </summary>
        /// <param name="ticketCommentId"></param>
        /// <returns></returns>
        public bool DeleteTicketComment(Guid ticketCommentId)
        {
            bool result = false;
            TicketComment ticketComment = null;
            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    //Get the specific TicketComment object from Database
                    ticketComment = (from t in svcPlsContext.TicketComments
                                     where t.TicketCommentId == ticketCommentId
                                     select t).FirstOrDefault();

                    if (ticketComment != null)
                    {
                        // Delete the TicketComment object
                        svcPlsContext.TicketComments.DeleteOnSubmit(ticketComment);

                        // Save to database
                        svcPlsContext.SubmitChanges();

                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in DeleteTicketComment().  Error: {0}", ex.Message);

                result = false;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "DeleteTicketComment";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return result;
        }

        /// <summary>        
        /// List<TicketComment> GetTicketComment(Guid ticketId)
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        public List<TicketComment> GetTicketComment(Guid ticketId)
        {
            List<TicketComment> ticketComments = null;

            try
            {
                using (ServicePleaseDataContext svcPlsContext = new ServicePleaseDataContext())
                {
                    var ticComments = svcPlsContext.TicketComments.Where(t => t.TicketId == ticketId).OrderByDescending(t=>t.CreateDate).Take(3);

                    if (ticComments != null)
                    {
                        ticketComments = ticComments.ToList<TicketComment>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error in GetTicketComment(Guid ticketId).  Error: {0}", ex.Message);

                ticketComments = null;

                //Pass error messages to ErrorMethod for save in Database
                string methodName = "GetTicketComment";
                string expDetails = ex.Message;
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                ErrorMethod(methodName, expDetails, innerException);
            }

            return ticketComments;
        }
        #endregion TicketComment Methods

        #endregion
    }
}
