using ServicePleaseServiceLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ServicePleaseServiceLibrary.Model;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;

namespace ServicePleaseServiceTest
{   
    /// <summary>
    ///This is a test class for ServicePleaseServiceTest and is intended
    ///to contain all ServicePleaseServiceTest Unit Tests
    ///</summary>
	[TestClass()]
	public class ServicePleaseServiceTest
	{
		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///A test for GetUserOrganization
		///</summary>
		[TestMethod()]
		public void GetUserOrganizationTest()
		{
            ServicePleaseServiceLibrary.ServicePleaseService target = new ServicePleaseServiceLibrary.ServicePleaseService();

			Guid userId = new Guid("FD7E849F-39BD-42A5-9199-98965B39545C"); 
			
			Organization expected = null; 

			Organization actual;
			
			actual = target.GetUserOrganization(userId);
			
			Assert.AreEqual(expected, actual);
		}

        /// <summary>
        ///A test for CreateUser
        ///</summary>
        [TestMethod()]
        public void CreateUserTest()
        {
            ServicePleaseServiceLibrary.ServicePleaseService target = new ServicePleaseServiceLibrary.ServicePleaseService();

            string userName = "EdElliott";
            string password = "AZ7mad7max";
            string firstName = "Ed";
            string middleName = "";
            string lastName = "Elliott";
            Guid locationId = Guid.Parse("DC2C260C-A35E-42DE-B2B3-331BFC3164EB");
            
            User expected = null; 
            
            User actual;
            
            actual = target.CreateUser(userName, password, firstName, middleName, lastName, locationId);

            userName = "KaiSchuette";
            password = "Schuette";
            firstName = "Kai";
            middleName = "";
            lastName = "Schuette";
            locationId = Guid.Parse("6C3A6C98-5E73-49D1-A3F8-934B63A7A9D9");

            expected = null;

            actual = target.CreateUser(userName, password, firstName, middleName, lastName, locationId);
            
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ValidateUser
        ///</summary>
        [TestMethod()]
        public void ValidateUserTest()
        {
            ServicePleaseServiceLibrary.ServicePleaseService target = new ServicePleaseServiceLibrary.ServicePleaseService(); 
            
            string userId = "EdElliott";
            string password = "AZ7mad7max!";

            bool expected = true;

            bool actual;
            
            actual = target.ValidateUser(userId, password);
            
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for AddTicket
        ///</summary>
        [TestMethod()]
        public void AddTicketTest()
        {
            ServicePleaseServiceLibrary.ServicePleaseService target = new ServicePleaseServiceLibrary.ServicePleaseService();

            Ticket ticket = new Ticket()
            {
                TicketId = Guid.NewGuid(),
                CategoryId = Guid.Parse("D8EB7052-DE67-4177-AA26-B7E274F2C517"),
                CloseDate = DateTime.UtcNow.AddDays(3).ToShortDateString(),
                CreateDate = DateTime.UtcNow.ToString(),
                EditDate = DateTime.UtcNow.ToString(),
                LocationId = Guid.Parse("6C3A6C98-5E73-49D1-A3F8-934B63A7A9D9"),
                Status = "Open",
                OrganizationId = Guid.Parse("39C4DE8E-64F9-47DD-AD06-B790124A3546"),
                TicketName = "Parking system failed.",
                UserId = Guid.Parse("76C37392-35F0-4B4A-930D-B1E9669D16E4")
            };

            Ticket expected = null; // TODO: Initialize to an appropriate value

            Ticket actual;
            
            actual = target.AddTicket(ticket);
            
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SendEmail
        ///</summary>
        [TestMethod()]
        public void SendEmailTest()
        {
            ServicePleaseServiceLibrary.ServicePleaseService target = new ServicePleaseServiceLibrary.ServicePleaseService();

            EmailElements emailElements = new EmailElements()
            {
                FromAddress = "eelliott57@nc.rr.com",
                ToAddresses = new StringCollection() { "edelliott@nc.rr.com", "eelliott57@nc.rr.com", "ed.elliott@evoapp.com" },
                CCAddresses = new StringCollection() { "ncedelliott@gmail.com" },
                BodyText = "This is some body text for this test message.",
                SubjectText = "Test Email Message"
            };

            target.SendEmail(emailElements);
        }

        /// <summary>
        ///A test for GetTicketsByLocation
        ///</summary>
        [TestMethod()]
        public void GetTicketsByLocationTest()
        {
            ServicePleaseServiceLibrary.ServicePleaseService target = new ServicePleaseServiceLibrary.ServicePleaseService();

            Guid locationId = Guid.Parse("6C3A6C98-5E73-49D1-A3F8-934B63A7A9D9");

            Guid contactId = Guid.Parse("B932BC4D-CA91-4CD6-BCD4-0D33332EB803");
            
            List<Ticket> expected = null;
            
            List<Ticket> actual;
            
            actual = target.GetTicketsByLocation(locationId, contactId);
            
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetTicketsByHourElapsed
        ///</summary>
        [TestMethod()]
        public void GetTicketsByHourElapsedTest()
        {
            ServicePleaseServiceLibrary.ServicePleaseService target = new ServicePleaseServiceLibrary.ServicePleaseService();

            string sortOrder = "newest";
 
            List<Ticket> expected = null; 

            List<Ticket> actual;
            
            actual = target.GetTicketsByHourElapsed(sortOrder);
            
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetTicketsByLocations
        ///</summary>
        [TestMethod()]
        public void GetTicketsByLocationsTest()
        {
            ServicePleaseServiceLibrary.ServicePleaseService target = new ServicePleaseServiceLibrary.ServicePleaseService();

            List<Guid> locationIds = new List<Guid>();

            locationIds.Add(Guid.Parse("6C3A6C98-5E73-49D1-A3F8-934B63A7A9D9"));
            locationIds.Add(Guid.Parse("A8F867DA-D706-4B60-ACED-8C911D3A9A4F"));
            
            List<Ticket> expected = null; 
            
            List<Ticket> actual;
            
            actual = target.GetTicketsByLocations(locationIds);
            
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetTicketMonitorRows
        ///</summary>
        [TestMethod()]
        public void GetTicketMonitorRowsTest()
        {
            ServicePleaseServiceLibrary.ServicePleaseService target = new ServicePleaseServiceLibrary.ServicePleaseService(); 
            
            List<TicketMonitorRow> expected = null; 
            
            List<TicketMonitorRow> actual;
            
            actual = target.GetTicketMonitorRows();
            
            CollectionAssert.AreEqual(expected, actual);
        }

		/// <summary>
		///A test for AddSolutionBlob
		///</summary>
		[TestMethod()]
		public void AddSolutionBlobTest()
		{
			ServicePleaseServiceLibrary.ServicePleaseService target = new ServicePleaseServiceLibrary.ServicePleaseService();

			Byte[] BlobBytes = File.ReadAllBytes(@"C:\Development\ServiceTrackingSystems\ServicePlease\Windows\ServicePleaseWebService\ServicePleaseService\WebServiceTest\EvoApp_logo.png");

			// FileStream BlobBytes = File.Open(@"C:\Development\ServiceTrackingSystems\ServicePlease\Windows\ServicePleaseWebService\ServicePleaseService\WebServiceTest\EvoApp_logo.png", FileMode.Open, FileAccess.Read);

			Guid problemId = Guid.Parse("60DBAC42-E27A-4228-A677-EC1635BBAF0B");

			BlobPacket BlobPacket = new BlobPacket()
			{
				EntityId = problemId,
				BlobBytes = Convert.ToBase64String(BlobBytes)
			};

			
			SolutionBlob expected = null; 
			
			SolutionBlob actual;
			
			actual = target.AddSolutionBlob(BlobPacket);
			
			Assert.AreEqual(expected, actual);
		}
	}
}
