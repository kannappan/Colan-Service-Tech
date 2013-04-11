using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using ServicePleaseServiceLibrary.Model;
using System.Web.Security;
using System.Collections.Specialized;

namespace ServicePleaseServiceLibrary.Interfaces
{
    [ServiceContract]
	[ServiceKnownType(typeof(BlobPacket))]
    public interface IServicePleaseService
    {
        #region Role Methods
        /// <summary>
        /// List<Role> GetRoles()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "roles",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
        List<Role> GetRoles();

        /// <summary>
        /// Role AddRole(Role role)
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "role",
                   Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        Role AddRole(Role role);

        /// <summary>
        /// Role GetRoleForUser(Guid userId)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "roleByUserId?userId={userId}",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
        Role GetRoleForUser(Guid userId);

        ///// <summary>
        ///// UserRole AddUserRole(UserRole userRole)
        ///// </summary>
        ///// <param name="userRole"></param>
        ///// <returns></returns>
        //[WebInvoke(UriTemplate = "userRole",
        //           Method = "POST",
        //           RequestFormat = WebMessageFormat.Json,
        //           ResponseFormat = WebMessageFormat.Json,
        //           BodyStyle = WebMessageBodyStyle.Bare)]
        //UserRole AddUserRole(UserRole userRole);

        /// <summary>
        /// FeatureRole AddFeatureRole(FeatureRole featureRole)
        /// </summary>
        /// <param name="featureRole"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "featureRole",
                   Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        FeatureRole AddFeatureRole(FeatureRole featureRole);

        /// <summary>
        /// IsUserAuthorized(Guid featureId, Guid roleId)
        /// </summary>
        /// <param name="featureId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "isAuthorizedForFeature?featureId={featureId}&roleId={roleId}",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
        bool IsUserAuthorizedForFeature(Guid featureId, Guid roleId);

        #endregion Role Methods

        #region ServicePlan Methods
        /// <summary>
        /// List<ServicePlanType> GetServicePlanTypes()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "servicePlanTypes",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
        List<ServicePlanType> GetServicePlanTypes();

        /// <summary>
        /// List<ServicePlanInfo> GetServicePlanInfo()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "servicePlanInfo",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
        List<ServicePlanInfo> GetServicePlanInfo();

        /// <summary>
        /// List<ServicePlanInfo> GetServicePlanInfoByLocation()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "servicePlanInfoByLocation?locationId={locationId}",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
        List<ServicePlanInfo> GetServicePlanInfoByLocation(Guid locationId);

        #endregion ServicePlan Methods

        #region User Methods
        /// <summary>
        /// bool ValidateUser(string userId, string password)
        /// 
        /// Validates the supplied credentials against the ASP.NET Membership database tables.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns>true if valid user</returns>
        [WebGet(UriTemplate = "validateUser?userId={userId}&password={password}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        bool ValidateUser(string userId, string password);

        /// <summary>
        /// MembershipUser CreateUser(string userId, string password)
        /// 
        /// Creates a new user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="firstName"></param>
        /// <param name="middleName"></param>
        /// <param name="lastName"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "user?userId={userId}&password={password}&firstName={firstName}&middleName={middleName}&lastName={lastName}&locationId={locationId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        User CreateUser(string userId, string password, string firstName, string middleName, string lastName, Guid locationId);

        /// <summary>
        /// User AddUser(User user)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "user",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        User AddUser(User user);

        /// <summary>
        /// User GetUser(string userName)
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "user/{userName}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        User GetUser(string userName);

        /// <summary>
        /// List<User> GetUsers()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "users",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<User> GetUsers();

        /// <summary>
        /// User UpdateUser(Guid userId, User user);
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateUser?userId={userId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        User UpdateUser(Guid userId, User user);

        /// <summary>
        /// String DeleteUser(Guid userId)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteUser?userId={userId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string DeleteUser(Guid userId);

        /// <summary>
        /// GetUserOrganization(Guid userId)
        /// 
        /// Gets the Organization that this user belongs to.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "userOrganization?userId={userId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        Organization GetUserOrganization(Guid userId);
        #endregion User Methods

        #region Organization Methods
        /// <summary>
        /// List<Organization> GetOrganizations()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "organizations",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Organization> GetOrganizations();

        /// <summary>
        /// Organization AddOrganization(Organization organization)
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "organization",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Organization AddOrganization(Organization organization);

        /// <summary>
        /// Organization UpdateOrganization(Organization organization, int organizationId)
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateOrganization?organizationId={organizationId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Organization UpdateOrganization(Organization organization, Guid organizationId);

        /// <summary>
        /// bool DeleteOrganization(int organizationId)
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteOrganization?organizationId={organizationId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeleteOrganization(Guid organizationId);
        #endregion Organization Methods

        #region Category Methods
        /// <summary>
        /// List<Category> GetCategories()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "categories",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Category> GetCategories();

        /// <summary>
        /// List<Category> GetCategory(string categoryId)
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "category/{categoryId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        Category GetCategory(string categoryId);

        /// <summary>
        /// List<Category> GetCategoriesByOrganization(string organizationId)
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "categoriesByOrganization/{organizationId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Category> GetCategoriesByOrganization(string organizationId);

        /// <summary>
        /// string AddCategory(Category category)
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "category",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string AddCategory(Category category);

        /// <summary>
        /// string UpdateCategory(Category category, int categoryId)
        /// </summary>
        /// <param name="category"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateCategory?categoryId={categoryId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string UpdateCategory(Category category, Guid categoryId);

        /// <summary>
        /// string DeleteCategory(int categoryId)
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteCategory?categoryId={categoryId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string DeleteCategory(Guid categoryId);
        #endregion Category Methods

        #region Location Methods
        /// <summary>
        /// List<Location> GetLocations()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "locations",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Location> GetLocations();

        /// <summary>
        /// Location GetLocationsByOrganization(Guid organizationId)
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "locationsByOrganization?organizationId={organizationId}",
               RequestFormat = WebMessageFormat.Json,
               ResponseFormat = WebMessageFormat.Json)]
        List<Location> GetLocationByOrganization(Guid organizationId);

        /// <summary>
        /// Location GetLocation(Guid locationId)
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "location?locationId={locationId}",
               RequestFormat = WebMessageFormat.Json,
               ResponseFormat = WebMessageFormat.Json)]
        Location GetLocation(Guid locationId);

        /// <summary>
        /// Location AddLocation(Location location)
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "location",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Location AddLocation(Location location);

        /// <summary>
        /// Location AddLocationBackGround(Location location)
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "locationBackGround",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Location AddLocationBackGround(Location location);

        /// <summary>
        /// Location UpdateLocation(Location location, int locationId)
        /// </summary>
        /// <param name="location"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateLocation?locationId={locationId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Location UpdateLocation(Location location, Guid locationId);

        /// <summary>
        /// string DeleteLocation(int locationId)
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteLocation?locationId={locationId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string DeleteLocation(Guid locationId);


        /// <summary>
        /// string DeleteLocation(int locationId)
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "delLocation?locationId={locationId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string DelLocation(Guid locationId);

        #endregion Location Methods

        #region LocationInfo Methods
        /// <summary>
        /// List<LocationInfo> GetLocationInfo()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "locationInfo",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<LocationInfo> GetLocationInfoList();

        /// <summary>
        /// LocationInfo GetLocationInfo(Guid locationInfoId)
        /// </summary>
        /// <param name="locationInfoId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "locationInfo?locationInfoId={locationInfoId}",
               RequestFormat = WebMessageFormat.Json,
               ResponseFormat = WebMessageFormat.Json)]
        LocationInfo GetLocationInfo(Guid locationInfoId);

        /// <summary>
        /// LocationInfo AddLocationInfo(LocationInfo locationInfo)
        /// </summary>
        /// <param name="locationInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "locationInfo",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        LocationInfo AddLocationInfo(LocationInfo locationInfo);

        /// <summary>
        /// LocationInfo UpdateLocationInfo(LocationInfo locationInfo, int locationInfoId)
        /// </summary>
        /// <param name="locationInfo"></param>
        /// <param name="locationInfoId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateLocationInfo?locationInfoId={locationInfoId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        LocationInfo UpdateLocationInfo(LocationInfo locationInfo, Guid locationInfoId);

        /// <summary>
        /// string DeleteLocationInfo(int locationInfoId)
        /// </summary>
        /// <param name="locationInfoId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteLocationInfo?locationInfoId={locationInfoId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string DeleteLocationInfo(Guid locationInfoId);
        #endregion LocationInfo Methods

        #region Contact Methods
        /// <summary>
        /// List<Contact> GetContacts()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "contacts",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Contact> GetContacts();

        /// <summary>
        /// List<Contact> GetContactsByLocation(Guid locationId)
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "contactsByLocation?locationId={locationId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Contact> GetContactsByLocation(Guid locationId);

        /// <summary>
        /// Contact GetContact(Guid contactId)
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "contact?contactId={contactId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        Contact GetContact(Guid contactId);

        /// <summary>
        /// string AddContact(Contact contact)
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "contact",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string AddContact(Contact contact);

        /// <summary>
        /// Contact UpdateContact(Contact contact, int contactId)
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="contactId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateContact?contactId={contactId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Contact UpdateContact(Contact contact, Guid contactId);

        /// <summary>
        /// string DeleteContact(int contactId)
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteContact?contactId={contactId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string DeleteContact(Guid contactId);
        #endregion Contact Methods

        #region Ticket Methods
        /// <summary>
        /// List<Ticket> GetTickets()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "tickets",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Ticket> GetTickets();

        /// <summary>
        /// List<Ticket> GetTicketsByHourElapsed()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "ticketsByHourElapsed?sortOrder={sortOrder}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Ticket> GetTicketsByHourElapsed(string sortOrder = "Oldest");

        /// <summary>
        /// List<Ticket> GetTicketsByCategory()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "ticketsByCategory?categoryId={categoryId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Ticket> GetTicketsByCategory(Guid categoryId);

        /// <summary>
        /// List<Ticket> GetTicketsByCategoryAndLocation()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "ticketsByCategoryLocation?categoryId={categoryId}&locationId={locationId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Ticket> GetTicketsByCategoryAndLocation(Guid categoryId, Guid locationId);

        /// <summary>
        /// List<Ticket> GetTicketsByLocation()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "ticketsByLocation?locationId={locationId}&contactId={contactId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Ticket> GetTicketsByLocation(Guid locationId, Guid contactId);

        /// <summary>
        /// List<Ticket> GetTicketsByLocation()
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "ticketsByLocations",
                Method = "POST",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Ticket> GetTicketsByLocations(List<Guid> locationIds);

        /// <summary>
        /// List<Ticket> GetTicketsByStatus(string status)
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "ticketsByStatus?status={status}",
                Method = "GET",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Ticket> GetTicketsByStatus(string status);

        /// <summary>
        /// List<Ticket> GetTicket(string ticketId)
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "ticket?ticketId={ticketId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        Ticket GetTicket(Guid ticketId);

        /// <summary>
        /// Ticket AddTicket(Ticket ticket)
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "ticket",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Ticket AddTicket(Ticket ticket);

        /// <summary>
        /// Ticket AddTicketBackGround(Ticket ticket)
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "ticketBackGround",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Ticket AddTicketBackGround(Ticket ticket);

        /// <summary>
        /// Ticket UpdateTicket(Ticket ticket, int ticketId)
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateTicket?ticketId={ticketId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Ticket UpdateTicket(Guid ticketId, Ticket ticket);

        /// <summary>
        /// Ticket UpdateTicketBackGround(Guid ticketId,Ticket ticket)
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateTicketBackGround?ticketId={ticketId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Ticket UpdateTicketBackGround(Guid ticketId, Ticket ticket);

        /// <summary>
        /// Ticket AssignTicket(Ticket ticket)
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "assignTicket",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Ticket AssignTicket(Ticket ticket);

        /// <summary>
        /// Ticket AssignTicketBackGround(Ticket ticket)
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "assignTicketBackGround",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Ticket AssignTicketBackGround(Ticket ticket);

        /// <summary>
        /// string DeleteTicket(int ticketId)
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteTicket?ticketId={ticketId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string DeleteTicket(Guid ticketId);
        #endregion Ticket Methods

        #region TicketMonitor Methods
        /// <summary>
        /// List<TicketMonitorRows> GetTicketMonitorRows()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "ticketMonitorRows",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<TicketMonitorRow> GetTicketMonitorRows();

        /// <summary>
        /// List<TicketMonitorRows> GetTicketMonitorRowsByHourElapsed()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "ticketMonitorRowsByElapsedTime?sortOrder={sortOrder}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<TicketMonitorRow> GetTicketMonitorRowsByElapsedTime(string sortOrder = "Oldest");

        /// <summary>
        /// List<TicketMonitorRows> GetTicketMonitorRowsByCategory()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "ticketMonitorRowsByCategory?categoryId={categoryId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<TicketMonitorRow> GetTicketMonitorRowsByCategory(Guid categoryId);

        /// <summary>
        /// List<TicketMonitorRows> GetTicketMonitorRowsByContacts()
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "ticketMonitorRowsByContacts",
                Method = "POST",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<TicketMonitorRow> GetTicketMonitorRowsByContacts(List<Guid> contactIds);

        /// <summary>
        /// List<TicketMonitorRows> GetTicketMonitorRowsByCategoryAndLocation()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "ticketMonitorRowsByCategoryLocation?categoryId={categoryId}&locationId={locationId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<TicketMonitorRow> GetTicketMonitorRowsByCategoryAndLocation(Guid categoryId, Guid locationId);

        /// <summary>
        /// List<TicketMonitorRows> GetTicketMonitorRowsByLocation()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "ticketMonitorRowsByLocation?locationId={locationId}&contactId={contactId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<TicketMonitorRow> GetTicketMonitorRowsByLocation(Guid locationId, Guid contactId);

        /// <summary>
        /// List<TicketMonitorRows> GetTicketMonitorRowsByLocation()
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "ticketMonitorRowsByLocations",
                Method = "POST",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<TicketMonitorRow> GetTicketMonitorRowsByLocations(List<Guid> locationIds);

        /// <summary>
        /// List<TicketMonitorRow> GetTicketMonitorRowsByStatus(List<string> status)
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "ticketMonitorRowsByStatus",
                Method = "POST",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<TicketMonitorRow> GetTicketMonitorRowsByStatus(List<string> status);

        /// <summary>
        /// List<TicketMonitorRows> GetTicketMonitorRowsByUser(Guid userId)
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "ticketMonitorRowsByTech?userId={userId}",
                Method = "GET",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<TicketMonitorRow> GetTicketMonitorRowsByUser(Guid userId);

        /// <summary>
        /// List<TicketMonitorRows> GetTicketMonitorRowsByServicePlanType(Guid servicePlanTypeId)
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "ticketMonitorRowsByServicePlanType?servicePlanTypeId={servicePlanTypeId}",
                Method = "GET",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<TicketMonitorRow> GetTicketMonitorRowsByServicePlanType(Guid servicePlanTypeId);

        /// <summary>
        /// List<TicketMonitorRows> GetTicketMonitorRowsByServicePlanName(Guid servicePlanName)
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "ticketMonitorRowsByServicePlanName?servicePlanName={servicePlanName}",
                Method = "GET",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<TicketMonitorRow> GetTicketMonitorRowsByServicePlanName(string servicePlanName);

        /// <summary>
        /// List<TicketMonitorRows> GetTicketMonitorRowsByTechs(List<Guid> techIds)
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "ticketMonitorRowsByTechs",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json)]
        List<TicketMonitorRow> GetTicketMonitorRowsByTechs(List<Guid> techIds);

        /// <summary>
        /// List<TicketMonitorRows> GetTicketMonitorRowsByCategories(List<Guid> categoryIds)
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "ticketMonitorRowsByCategories",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json)]
        List<TicketMonitorRow> GetTicketMonitorRowsByCategories(List<Guid> categoryIds);

        /// <summary>
        /// List<TicketMonitorRows> GetTicketMonitorRowsByServicePlanTypes(List<Guid> servicePlanTypeIds)
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "ticketMonitorRowsByServicePlanTypes",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json)]
        List<TicketMonitorRow> GetTicketMonitorRowsByServicePlanTypes(List<Guid> servicePlanTypeIds);

        /// <summary>
        /// List<Ticket> GetTicketMonitorRow(string ticketId)
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "ticketMonitorRow?ticketId={ticketId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        TicketMonitorRow GetTicketMonitorRow(Guid ticketId);

        /// <summary>
        /// List<TicketMonitorRow> GetTicketMonitorRowsByUserAndIsStatusClosed(Guid userId, bool isStatusClosed);
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "ticketMonitorRowsByTechAndIsStatusClosed?userId={userId}&isStatusClosed={isStatusClosed}",
                Method = "GET",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<TicketMonitorRow> GetTicketMonitorRowsByUserAndIsStatusClosed(Guid userId, bool isStatusClosed);

        #endregion TicketMonitor Methods

        #region Solution Methods
        /// <summary>
        /// List<Solution> GetSolutions()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "solutions",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Solution> GetSolutions();

        /// <summary>
        /// Solution GetSolution(Guid solutionId)
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "solution?solutionId={solutionId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        Solution GetSolution(Guid solutionId);

        /// <summary>
        /// Solution AddSolution(Solution solution)
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "solution",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Solution AddSolution(Solution solution);

        /// <summary>
        /// Solution UpdateSolution(Solution solution, int solutionId)
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateSolution?solutionId={solutionId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Solution UpdateSolution(Solution solution, Guid solutionId);

        /// <summary>
        /// bool DeleteSolution(int solutionId)
        /// </summary>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteSolution?solutionId={solutionId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeleteSolution(Guid solutionId);

        ///<summary>
        ///List<SolutionsWithBlob> GetSolutionsByTicketId(Guid ticketId)
        ///</summary>
        ///<param name="ticketId"></param>
        ///<returns></returns>
        [WebGet(UriTemplate = "solutionsByTicket?ticketId={ticketId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<SolutionsWithBlob> GetSolutionsByTicketId(Guid ticketId);

        #endregion Solution Methods

        #region Problem Methods
        /// <summary>
        /// List<Problem> GetProblems()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "problems",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Problem> GetProblems();

        /// <summary>
        /// Problem GetProblem(Guid problemId)
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "problem?problemId={problemId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        Problem GetProblem(Guid problemId);

        /// <summary>
        /// Problem AddProblem(Problem problem)
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "problem",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Problem AddProblem(Problem problem);

        /// <summary>
        /// Problem UpdateProblem(Problem problem, int problemId)
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="problemId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateProblem?problemId={problemId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Problem UpdateProblem(Problem problem, Guid problemId);

        /// <summary>
        /// bool DeleteProblem(int problemId)
        /// </summary>
        /// <param name="problemId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteProblem?problemId={problemId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeleteProblem(Guid problemId);

        ///<summary>
        ///List<ProblemsWithBlob> GetProblemsByTicketId(Guid ticketId)
        ///</summary>
        ///<param name="ticketId"></param>
        ///<returns></returns>

        [WebGet(UriTemplate = "problemsByTicket?ticketId={ticketId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<ProblemsWithBlob> GetProblemsByTicketId(Guid ticketId);
        #endregion Problem Methods

        #region Solution Blob Methods
        /// <summary>
        /// List<BlobEntry> GetSolutionBlobs(Guid solutionId)
        /// </summary>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "solutionBlobs?solutionId={solutionId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<SolutionBlobPacket> GetSolutionBlobs(Guid solutionId);

        /// <summary>
        /// SolutionBlob AddSolutionBlob(BlobPacket blobPacket)
        /// </summary>
        /// <param name="BlobBytes"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "solutionBlob",
                   Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        SolutionBlob AddSolutionBlob(BlobPacket blobPacket);

        /// <summary>
        /// bool UpdateSolutionBlob(BlobPacket blobPacket)
        /// </summary>
        /// <param name="BlobBytes"></param>
        /// <param name="SolutionBlobId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "solutionBlob",
                   Method = "PUT",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        bool UpdateSolutionBlob(BlobPacket blobPacket);

        /// <summary>
        /// bool DeleteSolutionBlob(Guid solutionBlobId)
        /// </summary>
        /// <param name="SolutionBlobId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "solutionBlob?SolutionBlobId={solutionBlobId}",
                   Method = "DELETE",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeleteSolutionBlob(Guid solutionBlobId);
        #endregion Solution Blob Methods

        #region Problem Blob Methods
        /// <summary>
        /// List<BlobEntry> GetProblemBlobs(Guid problemId)
        /// </summary>
        /// <param name="problemId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "problemBlobs?problemId={problemId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<ProblemBlobPacket> GetProblemBlobs(Guid problemId);

        /// <summary>
        /// ProblemBlob AddProblemBlob(BlobPacket blobPacket)
        /// </summary>
        /// <param name="BlobBytes"></param>
        /// <param name="problemId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "problemBlob",
                   Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        ProblemBlob AddProblemBlob(BlobPacket blobPacket);

        /// <summary>
        /// bool UpdateProblemBlob(BlobPacket blobPacket)
        /// </summary>
        /// <param name="BlobBytes"></param>
        /// <param name="ProblemBlobId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "problemBlob",
                   Method = "PUT",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        bool UpdateProblemBlob(BlobPacket blobPacket);

        /// <summary>
        /// bool DeleteProblemBlob(Guid problemBlobId)
        /// </summary>
        /// <param name="ProblemBlobId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "problemBlob?problemBlobId={problemBlobId}",
                   Method = "DELETE",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeleteProblemBlob(Guid problemBlobId);
        #endregion Problem Blob Methods

        #region Email Methods
        /// <summary>
        /// SendEmail(EmailElements emailElements)
        /// </summary>
        /// <param name="emailElements"></param>
        [WebInvoke(UriTemplate = "sendEmail",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        void SendEmail(EmailElements emailElements);
        #endregion Email Methods

        #region Knowledgebase Search Methods
        [WebInvoke(UriTemplate = "searchProblems",
                   Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        List<Problem> SearchProblems(List<string> searchTerms);

        [WebInvoke(UriTemplate = "problemKnowledgeSearch",
                   Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        List<Problem> ProblemKnowledgeSearch(KnowledgeSearch searchParms);

        [WebInvoke(UriTemplate = "searchSolutions",
                   Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        List<Solution> SearchSolutions(List<string> searchTerms);

        //[WebInvoke(UriTemplate = "solutionKnowledgeSearch",
        //           Method = "POST",
        //           RequestFormat = WebMessageFormat.Json,
        //           ResponseFormat = WebMessageFormat.Json,
        //           BodyStyle = WebMessageBodyStyle.Bare)]
        //List<Solution> SolutionKnowledgeSearch(KnowledgeSearch searchParms);
        [WebInvoke(UriTemplate = "solutionKnowledgeSearch",
                   Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        List<SolutionWithHelpDoc> SolutionKnowledgeSearch(KnowledgeSearch searchParms);
        #endregion Knowledgebase Search Methods

        #region Snooze Methods
        /// <summary>
        /// List<Snooze> GetSnoozes()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "snoozes",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Snooze> GetSnoozes();

        /// <summary>
        /// Snooze GetSnooze(Guid snoozeId)
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "snooze?snoozeId={snoozeId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        Snooze GetSnooze(Guid snoozeId);

        /// <summary>
        /// Snooze AddSnooze(Snooze snooze)
        /// </summary>
        /// <param name="snooze"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "snooze",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Snooze AddSnooze(Snooze snooze);

        /// <summary>
        /// Snooze UpdateSnooze(Snooze snooze, int snoozeId)
        /// </summary>
        /// <param name="snooze"></param>
        /// <param name="snoozeId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateSnooze?snoozeId={snoozeId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Snooze UpdateSnooze(Snooze snooze, Guid snoozeId);

        /// <summary>
        /// bool DeleteSnooze(int snoozeId)
        /// </summary>
        /// <param name="snoozeId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteSnooze?snoozeId={snoozeId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeleteSnooze(Guid snoozeId);
        #endregion Snooze Methods

        #region SnoozeReason Methods
        /// <summary>
        /// List<SnoozeReason> GetSnoozeReasons()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "snoozeReasons",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<SnoozeReason> GetSnoozeReasons();

        /// <summary>
        /// Snooze GetSnoozeReason(Guid snoozeReasonId)
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "snoozeReason?snoozeReasonId={snoozeReasonId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        SnoozeReason GetSnoozeReason(Guid snoozeReasonId);

        /// <summary>
        /// string AddSnoozeReason(SnoozeReason snoozeReason)
        /// </summary>
        /// <param name="snoozeReason"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "snoozeReason",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string AddSnoozeReason(SnoozeReason snoozeReason);

        /// <summary>
        /// string UpdateSnoozeReason(SnoozeReason snoozeReason, int snoozeReasonId)
        /// </summary>
        /// <param name="snoozeReason"></param>
        /// <param name="snoozeReasonId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateSnoozeReason?snoozeReasonId={snoozeReasonId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string UpdateSnoozeReason(SnoozeReason snoozeReason, Guid snoozeReasonId);

        /// <summary>
        /// string DeleteSnoozeReason(int snoozeReasonId)
        /// </summary>
        /// <param name="snoozeReasonId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteSnoozeReason?snoozeReasonId={snoozeReasonId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string DeleteSnoozeReason(Guid snoozeReasonId);
        #endregion SnoozeReason Methods

        #region IntervalType Methods
        /// <summary>
        /// List<IntervalType> GetIntervalTypes()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "intervalTypes",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<IntervalType> GetIntervalTypes();

        /// <summary>
        /// IntervalType GetIntervalType(Guid intervalTypeId)
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "intervalType?intervalTypeId={intervalTypeId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        IntervalType GetIntervalType(Guid intervalTypeId);

        /// <summary>
        /// string AddIntervalType(IntervalType intervalType)
        /// </summary>
        /// <param name="intervalType"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "intervalType",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string AddIntervalType(IntervalType intervalType);

        /// <summary>
        /// string UpdateIntervalType(IntervalType intervalType, int intervalTypeId)
        /// </summary>
        /// <param name="intervalType"></param>
        /// <param name="intervalTypeId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateIntervalType?intervalTypeId={intervalTypeId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string UpdateIntervalType(IntervalType intervalType, Guid intervalTypeId);

        /// <summary>
        /// string DeleteIntervalType(int intervalTypeId)
        /// </summary>
        /// <param name="intervalTypeId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteIntervalType?intervalTypeId={intervalTypeId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string DeleteIntervalType(Guid intervalTypeId);
        #endregion IntervalType Methods

        #region ApplicationType Methods
        /// <summary>
        /// List<ApplicationType> GetApplicationTypes()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "applicationTypes",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<ApplicationType> GetApplicationTypes();

        /// <summary>
        /// ApplicationType GetApplicationType(Guid applicationTypeId)
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "applicationType?applicationTypeId={applicationTypeId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        ApplicationType GetApplicationType(Guid applicationTypeId);

        /// <summary>
        /// ApplicationType AddApplicationType(ApplicationType applicationType)
        /// </summary>
        /// <param name="applicationType"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "applicationType",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        ApplicationType AddApplicationType(ApplicationType applicationType);

        /// <summary>
        /// ApplicationType UpdateApplicationType(ApplicationType applicationType, int applicationTypeId)
        /// </summary>
        /// <param name="applicationType"></param>
        /// <param name="applicationTypeId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateApplicationType?applicationTypeId={applicationTypeId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        ApplicationType UpdateApplicationType(ApplicationType applicationType, Guid applicationTypeId);

        /// <summary>
        /// bool DeleteApplicationType(int applicationTypeId)
        /// </summary>
        /// <param name="applicationTypeId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteApplicationType?applicationTypeId={applicationTypeId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeleteApplicationType(Guid applicationTypeId);
        #endregion ApplicationType Methods

        #region SystemConfig Methods
        /// <summary>
        /// List<SystemConfig> GetSystemConfigs()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "systemConfigs",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<SystemConfig> GetSystemConfigs();

        /// <summary>
        /// SystemConfig GetSystemConfig(Guid systemConfigId)
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "systemConfig?systemConfigId={systemConfigId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        SystemConfig GetSystemConfig(Guid systemConfigId);

        /// <summary>
        /// string GetSystemConfigValue(string configKey)
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "systemConfigValue?configKey={configKey}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        string GetSystemConfigValue(string configKey);

        /// <summary>
        /// SystemConfig AddSystemConfig(SystemConfig systemConfig)
        /// </summary>
        /// <param name="systemConfig"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "systemConfig",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        SystemConfig AddSystemConfig(SystemConfig systemConfig);

        /// <summary>
        /// SystemConfig UpdateSystemConfig(SystemConfig systemConfig, int systemConfigId)
        /// </summary>
        /// <param name="systemConfig"></param>
        /// <param name="systemConfigId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateSystemConfig?systemConfigId={systemConfigId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        SystemConfig UpdateSystemConfig(SystemConfig systemConfig, Guid systemConfigId);

        /// <summary>
        /// bool DeleteSystemConfig(int systemConfigId)
        /// </summary>
        /// <param name="systemConfigId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteSystemConfig?systemConfigId={systemConfigId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeleteSystemConfig(Guid systemConfigId);
        #endregion SystemConfig Methods

        #region Tech Performance Methods
        [WebGet(UriTemplate = "openTicketCountByTech?techId={techId}&startDate={startDate}&endDate={endDate}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        long GetOpenTickets(Guid techId, string startDate, string endDate);

        [WebGet(UriTemplate = "closedTicketCountByTech?techId={techId}&startDate={startDate}&endDate={endDate}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        long GetClosedTickets(Guid techId, string startDate, string endDate);

        [WebGet(UriTemplate = "averageTimeToCloseByTech?techId={techId}&startDate={startDate}&endDate={endDate}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        double GetAverageTimeToCloseByTech(Guid techId, string startDate, string endDate);

        [WebGet(UriTemplate = "averageResponseTimeByTech?techId={techId}&startDate={startDate}&endDate={endDate}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        double GetAverageResponseTimeByTech(Guid techId, string startDate, string endDate);

        /// <summary>
        /// List<TechMetrics> GetTechMetrics()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "techMetrics",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
        List<TechMetrics> GetTechMetrics();
        #endregion Tech Performance Methods

        #region Like/Unlike Methods
        /// <summary>
        /// GetLikesAndUnlikesForTech(Guid techId, string startDate, string endDate)
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "likeUnlikesForTech?techId={techId}&startDate={startDate}&endDate={endDate}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<LikeUnlike> GetLikesAndUnlikesForTech(Guid techId, string startDate, string endDate);

        /// <summary>
        /// List<LikeUnlike> GetLikesAndUnlikesByTechAndSolution(Guid techId, Guid solutionId)
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "likeUnlikesByTechAndSolution?techId={techId}&solutionId={solutionId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<LikeUnlike> GetLikesAndUnlikesByTechAndSolution(Guid techId, Guid solutionId);

        /// <summary>
        /// GetLikesAndUnlikesForSolution(Guid solutionId, string startDate, string endDate)
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "likeUnlikesForSolution?solutionId={solutionId}&startDate={startDate}&endDate={endDate}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<LikeUnlike> GetLikesAndUnlikesForSolution(Guid solutionId, string startDate, string endDate);

        /// <summary>
        /// LikeUnlike AddLikeUnlike(LikeUnlike likeUnlike)
        /// </summary>
        /// <param name="likeUnlike"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "likeUnlike", Method = "POST",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Bare)]
        LikeUnlike AddLikeUnlike(LikeUnlike likeUnlike);

        /// <summary>
        /// bool DeleteLikeUnlike(Guid likeUnlikeId)
        /// </summary>
        /// <param name="likeUnlikeId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteLikeUnlike?likeUnlikeId={likeUnlikeId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeleteLikeUnlike(Guid likeUnlikeId);

        /// <summary>
        /// LikeUnlike UpdateLikeUnlike(LikeUnlike likeUnlike, Guid likeUnlikeId)
        /// </summary>
        /// <param name="likeUnlike"></param>
        /// <param name="likeUnlikeId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateLikeUnlike?likeUnlikeId={likeUnlikeId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        LikeUnlike UpdateLikeUnlike(LikeUnlike likeUnlike, Guid likeUnlikeId);

        #endregion Like/Unlike Methods

        #region Daily Recap Methods
        /// <summary>
        /// GetDailyRecapByTech(Guid techId, string startDate, string endDate)
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "dailyRecapByTech?techId={techId}&startDate={startDate}&endDate={endDate}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<DailyRecap> GetDailyRecapByTech(Guid techId, string startDate, string endDate);

        /// <summary>
        /// AddDailyRecap(List<DailyRecap> dailyRecapList)
        /// </summary>
        /// <param name="dailyRecapList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "dailyRecap",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        List<DailyRecap> AddDailyRecap(List<DailyRecap> dailyRecapList);
        #endregion Daily Recap Methods

        #region RecapSetting Methods
        /// <summary>
        /// string AddRecapSetting(RecapSetting recapSetting)
        /// </summary>
        /// <param name="recapSetting"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "recapSetting", Method = "POST",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Bare)]
        string AddRecapSetting(RecapSetting recapSetting);

        /// <summary>
        /// string UpdateRecapSetting(RecapSetting recapSetting, Guid recapSettingId)
        /// </summary>
        /// <param name="recapSetting"></param>
        /// <param name="recapSettingId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateRecapSetting?recapSettingId={recapSettingId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string UpdateRecapSetting(RecapSetting recapSetting, Guid recapSettingId);

        /// <summary>
        /// string DeleteRecapSetting(Guid recapSettingId)
        /// </summary>
        /// <param name="recapSettingId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteRecapSetting?recapSettingId={recapSettingId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        string DeleteRecapSetting(Guid recapSettingId);

        /// <summary>
        /// RecapSetting GetRecapSetting(Guid recapSettingId)
        /// </summary>
        /// <param name="recapSettingId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "recapSetting?recapSettingId={recapSettingId}",
               RequestFormat = WebMessageFormat.Json,
               ResponseFormat = WebMessageFormat.Json)]
        RecapSetting GetRecapSetting(Guid recapSettingId);

        /// <summary>
        /// List<RecapSetting> GetRecapSettings()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "getRecapSettings",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<RecapSetting> GetRecapSettings();

        #endregion RecapSetting Methods

        #region SnoozedTicket Methods
        /// <summary>
        /// SnoozedTicket AddSnoozedTicket(SnoozedTicket snoozedTicket)
        /// </summary>
        /// <param name="snoozedTicket"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "snoozedTicket", Method = "POST",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Bare)]
        SnoozedTicket AddSnoozedTicket(SnoozedTicket snoozedTicket);

        /// <summary>
        /// List<SnoozedTicket> GetSnoozedTickets()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "snoozedTickets",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<SnoozedTicket> GetSnoozedTickets();

        /// <summary>
        /// SnoozedTicket UpdateSnoozedTicket(SnoozedTicket snoozedTicket, Guid snoozedTicketId)
        /// </summary>
        /// <param name="snoozedTicket"></param>
        /// <param name="snoozedTicketId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateSnoozedTicket?snoozedTicketId={snoozedTicketId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        SnoozedTicket UpdateSnoozedTicket(SnoozedTicket snoozedTicket, Guid snoozedTicketId);

        /// <summary>
        /// bool DeleteSnoozedTicket(Guid ticketId)
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteSnoozedTicket?ticketId={ticketId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeleteSnoozedTicket(Guid ticketId);

        /// <summary>
        /// SnoozedTicket GetSnoozedTicket(Guid snoozedTicketId)
        /// </summary>
        /// <param name="snoozedTicketId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "snoozedTicket?snoozedTicketId={snoozedTicketId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        SnoozedTicket GetSnoozedTicket(Guid snoozedTicketId);

        #endregion SnoozedTicket Methods

        #region Error Methods
        /// <summary>
        /// List<ErrorDetail> GetErrorDetails()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "errorDetails",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
        List<ErrorDetail> ErrorDetails();


        /// <summary>
        /// GetErrorDetail(string methodName, DateTime startDate, DateTime endDate)
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "errorDetail?methodName={methodName}&startDate={startDate}&endDate={endDate}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<ErrorDetail> GetErrorDetail(string methodName, DateTime startDate, DateTime endDate);

        /// <summary>
        /// bool DeleteErrorDetail(string methodName)
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteErrorDetail?methodName={methodName}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeleteErrorDetail(string methodName);

        #endregion Error Methods

        #region DeviceDetail Methods
        /// <summary>
        /// List<DeviceDetail> GetDeviceDetails()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "deviceDetails",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<DeviceDetail> GetDeviceDetails();

        /// <summary>
        /// List<DeviceDetail> GetDeviceDetail(string deviceDetailId)
        /// </summary>
        /// <returns></returns>likeUnlikesByTechAndSolution?techId={techId}
        [WebGet(UriTemplate = "deviceDetail?deviceDetailId={deviceDetailId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        DeviceDetail GetDeviceDetail(string deviceDetailId);

        /// <summary>
        /// DeviceDetail AddDeviceDetail(DeviceDetail deviceDetail)
        /// </summary>
        /// <param name="deviceDetail"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deviceDetail",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        DeviceDetail AddDeviceDetail(DeviceDetail deviceDetail);

        /// <summary>
        /// DeviceDetail UpdateDeviceDetail(DeviceDetail deviceDetail, string deviceDetailId)
        /// </summary>
        /// <param name="deviceDetail"></param>
        /// <param name="deviceDetailId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateDeviceDetail?deviceDetailId={deviceDetailId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        DeviceDetail UpdateDeviceDetail(DeviceDetail deviceDetail, Guid deviceDetailId);

        /// <summary>
        /// bool DeleteDeviceDetail(int deviceDetailId)
        /// </summary>
        /// <param name="deviceDetailId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteDeviceDetail?deviceDetailId={deviceDetailId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeleteDeviceDetail(Guid deviceDetailId);

        #endregion DeviceDetail Methods

        #region UserRole Methods
        /// <summary>
        /// UserRole GetUserRole(Guid userRoleId)
        /// </summary>
        /// <param name="userRoleId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "userRole?userRoleId={userRoleId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        UserRole GetUserRole(Guid userRoleId);

        /// <summary>
        /// UserRole AddUserRole(UserRole userRole)
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "userRole", Method = "POST",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Bare)]
        UserRole AddUserRole(UserRole userRole);

        /// <summary>
        /// UserRole UpdateUserRole(UserRole userRole, Guid userRoleId)
        /// </summary>
        /// <param name="userRole"></param>
        /// <param name="userRoleId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateUserRole?userRoleId={userRoleId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        UserRole UpdateUserRole(UserRole userRole, Guid userRoleId);

        /// <summary>
        /// bool DeleteUserRole(Guid userRoleId)
        /// </summary>
        /// <param name="userRoleId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteUserRole?userRoleId={userRoleId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeleteUserRole(Guid userRoleId);

        #endregion UserRole Methods

        #region Permission Methods
        /// <summary>
        /// List<Permission> GetPermissions()
        /// </summary>
        /// <returns></returns>
        /// <summary>
        [WebGet(UriTemplate = "permissions",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<Permission> GetPermissions();

        /// </summary>
        /// Permission GetPermission(Guid permissionId);
        /// <param name="permissionId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "permission?permissionId={permissionId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        Permission GetPermission(Guid permissionId);

        /// <summary>
        /// Permission AddPermission(Permission permission)
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "permission", Method = "POST",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Bare)]
        Permission AddPermission(Permission permission);

        /// <summary>
        /// Permission UpdatePermission(Permission permission, Guid permissionId)
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updatePermission?permissionId={permissionId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        Permission UpdatePermission(Permission permission, Guid permissionId);

        /// <summary>
        /// bool DeletePermission(Guid permissionId)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deletePermission?permissionId={permissionId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeletePermission(Guid permissionId);
        #endregion Permission Methods

        #region RolePermission Methods
        /// <summary>
        /// List<RolePermission> GetRolePermissions()
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "rolepermissions",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
        List<RolePermission> GetRolePermissions();

        /// <summary>
        /// RolePermission GetRolePermission(Guid rolepermissionId)
        /// </summary>
        /// <param name="rolepermissionId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "rolepermission?rolepermissionId={rolepermissionId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        RolePermission GetRolePermission(Guid rolepermissionId);

        /// <summary>
        /// RolePermission AddRolePermission(RolePermission rolepermission)
        /// </summary>
        /// <param name="rolepermission"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "rolepermission", Method = "POST",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Bare)]
        RolePermission AddRolePermission(RolePermission rolepermission);

        /// <summary>
        /// RolePermission UpdateRolePermission(RolePermission rolepermission, Guid rolepermissionId)
        /// </summary>
        /// <param name="rolepermission"></param>
        /// <param name="rolepermissionId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateRolePermission?rolepermissionId={rolepermissionId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        RolePermission UpdateRolePermission(RolePermission rolepermission, Guid rolepermissionId);

        /// <summary>
        /// bool DeleteRolePermission(Guid rolepermissionId)
        /// </summary>
        /// <param name="rolepermissionId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteRolePermission?rolepermissionId={rolepermissionId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeleteRolePermission(Guid rolepermissionId);
        #endregion RolePermission Methods

        #region RolePermission Detail
        /// <summary>
        /// List<RolePermissionDetail> GetRolePermissionDetail(Guid roleId)
        /// </summary>
        /// /// <param name="roleId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "getRolePermissionDetail?roleId={roleId}",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json)]
        List<RolePermissionDetail> GetRolePermissionDetail(Guid roleId);

        /// <summary>
        /// bool UpdateRolePermissionDetail(List<RolePermissionDetail> rolePermissiondetail);
        /// </summary>       
        /// <param name="rolePermissiondetail"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateRolePermissionDetail",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        bool UpdateRolePermissionDetail(List<RolePermissionDetail> rolePermissiondetail);

        #endregion RolePermission Detail

        #region TicketComment Methods
        /// <summary>
        /// List<TicketComment> GetTicketComments()
        /// </summary>
        /// <returns></returns>        
        [WebGet(UriTemplate = "ticketComments",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<TicketComment> GetTicketComments();

        ///// <summary>        
        ///// TicketComment GetTicketComment(Guid ticketCommentId)
        ///// </summary>
        ///// <param name="ticketCommentId"></param>
        ///// <returns></returns>
        //[WebGet(UriTemplate = "ticketComment?ticketCommentId={ticketCommentId}",
        //        RequestFormat = WebMessageFormat.Json,
        //        ResponseFormat = WebMessageFormat.Json)]
        //TicketComment GetTicketComment(Guid ticketCommentId);

        /// <summary>
        /// TicketComment AddTicketComment(TicketComment ticketComment)
        /// </summary>
        ///<param name="ticketComment"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "ticketComment", Method = "POST",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Bare)]
        TicketComment AddTicketComment(TicketComment ticketComment);

        /// <summary>
        /// TicketComment UpdateTicketComment(TicketComment ticketComment, Guid ticketCommentId)
        /// </summary>
        /// <param name="ticketComment"></param>
        /// <param name="ticketCommentId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "updateTicketComment?ticketCommentId={ticketCommentId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        TicketComment UpdateTicketComment(TicketComment ticketComment, Guid ticketCommentId);

        /// <summary>
        /// bool DeleteTicketComment(Guid ticketCommentId)
        /// </summary>
        /// <param name="ticketCommentId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "deleteTicketComment?ticketCommentId={ticketCommentId}",
                    Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare)]
        bool DeleteTicketComment(Guid ticketCommentId);

        /// <summary>        
        /// List<TicketComment> GetTicketComment(Guid ticketId)
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "ticketComment?ticketId={ticketId}",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json)]
        List<TicketComment> GetTicketComment(Guid ticketId);

        #endregion TicketComment Methods
    }
}
