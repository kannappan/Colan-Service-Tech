using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.ClientServices;
using WebServiceTest.Models;
using System.Web.Script.Serialization;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using ServicePleaseServiceLibrary.Model;
using System.Runtime.Serialization.Json;

namespace WebServiceTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            Console.WriteLine();
            Console.WriteLine("Processing new Category. . .");
            Console.WriteLine();

			// uploadProblemBlob();
			// uploadSolutionBlob();

			//searchProblems(new List<String>() { "audio", "blob" });

            //List<DailyRecap> dailyRecapList = new List<DailyRecap>();

            //DailyRecap dlyRecap = new DailyRecap();

            //dlyRecap.TechId = Guid.Parse("90a016d7-8ce1-446b-a752-60a86be1a525");
            //dlyRecap.Action = "Test";
            //dlyRecap.Object = "Solution";
            //dlyRecap.Value = "Test2";
            //dlyRecap.TimeStamp = DateTime.Now;
            //dlyRecap.CreateDate = DateTime.Now;

            //dailyRecapList.Add(dlyRecap);

            //dlyRecap = new DailyRecap();

            //dlyRecap.TechId = Guid.Parse("90a016d7-8ce1-446b-a752-60a86be1a525");
            //dlyRecap.Action = "Test";
            //dlyRecap.Object = "Solution";
            //dlyRecap.Value = "Test2";
            //dlyRecap.TimeStamp = DateTime.Now;
            //dlyRecap.CreateDate = DateTime.Now;

            //dailyRecapList.Add(dlyRecap);

            //AddDailyRecapList(dailyRecapList);

            //Guid techId = Guid.Parse("1627aea5-8e0a-4371-9022-9b504344e724");

            //DateTime fromDate = DateTime.Parse("1900-01-01");

            //DateTime toDate = DateTime.Parse("2100-01-01");

            //List<DailyRecap> dlyRecapList = GetDailyRecapList(techId, fromDate, toDate);

            SolutionBlob sb = AddSolutionBlob(new BlobPacket());

            //List<Guid> locationIds = new List<Guid>() { Guid.Parse("29A2B522-E8C7-4406-BF22-5EBDE960A17E"), Guid.Parse("5B9D02BA-CB1F-42D9-BD6A-4AF0B39F62E1") };
            //List<Guid> categoryIds = new List<Guid>() { Guid.Parse("6A2A4EFF-9B01-4148-9122-498D7CF62216") };
            //List<string> searchTerms = new List<string>() { "Text Message", "ticket", "Test" };

            //KnowledgeSearch knowledgeSearch = new KnowledgeSearch() { CategoryIds = categoryIds, LocationIds = locationIds, SearchTerms = searchTerms, HelpDocOption = HelpDocOptions.NoHelpDocs };

            //List<Problem> problems = problemKnowledgeSearch(knowledgeSearch);
            // List<Problem> problems = searchProblemsByCategories(knowledgeSearch);

			// deleteProblemBlob();

			// updateProblemBlob();

            Console.WriteLine();
            Console.WriteLine("Please press a key to quit. . .");
            Console.WriteLine();

            Console.ReadKey();
        }

        private static List<DailyRecap> GetDailyRecapList(Guid techId, DateTime fromDate, DateTime toDate)
        {
            string urlString = string.Format("http://localhost:1923/dailyRecapByTech?techId={0}&startDate={1}&endDate={2}", techId, fromDate, toDate);
            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";

            List<DailyRecap> dailyRecapList = null;

            

            byte[] data = client.DownloadData(urlString);

            Stream stream = new MemoryStream(data);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(DailyRecap[]));
            DailyRecap[] result = (DailyRecap[])serializer.ReadObject(stream);

            dailyRecapList = new List<DailyRecap>(result.ToList());

            return dailyRecapList;
        }

        private static List<DailyRecap> AddDailyRecapList(List<DailyRecap> dailyRecapList)
        {
            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";

            
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(DailyRecap[]));
            serializer.WriteObject(stream, dailyRecapList.ToArray());

            byte[] data = client.UploadData("http://servicetech.colandevelopmentteam.com/dailyRecap", "POST", stream.ToArray());

            stream = new MemoryStream(data);
            serializer = new DataContractJsonSerializer(typeof(DailyRecap[]));
            DailyRecap[] result = (DailyRecap[])serializer.ReadObject(stream);

            dailyRecapList = new List<DailyRecap>(result.ToList());

            return dailyRecapList;
        }

        private static SolutionBlob AddSolutionBlob(BlobPacket blobPacket)
        {
            blobPacket.BlobTypeId = Guid.Parse("A2C27AD5-D614-4AFE-A353-B3F7268D82A6");
            blobPacket.EntityId = Guid.Parse("da13374e-1942-4f83-8513-a8861245ab41");
            blobPacket.BlobBytes = "test";
            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";


            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(BlobPacket));
            serializer.WriteObject(stream, blobPacket);

            byte[] data = client.UploadData("http://localhost:1923/solutionBlob", "POST", stream.ToArray());

            stream = new MemoryStream(data);
            serializer = new DataContractJsonSerializer(typeof(SolutionBlob));
            SolutionBlob result = (SolutionBlob)serializer.ReadObject(stream);

            

            return result;
        }

		private static List<Problem> searchProblems(List<String> searchTerms)
		{
			List<Problem> problemList = null;

			string jsonString = string.Empty;

			string urlString = "http://www.ekeservices.net/ServicePleaseWebService/searchProblems";
			// string urlString = "http://localhost:1923/searchProblems";

			JavaScriptSerializer serializer = new JavaScriptSerializer();

			serializer.MaxJsonLength = 2147483647;

			jsonString = serializer.Serialize(searchTerms);

			Byte[] requestData = Encoding.ASCII.GetBytes(jsonString);

			HttpWebRequest request = CreateWebRequest(urlString, "POST", requestData.Length);

			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(requestData, 0, requestData.Length);
			}

			using (var response = (HttpWebResponse)request.GetResponse())
			{
				if (response.StatusCode != HttpStatusCode.OK)
				{
					string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
					throw new ApplicationException(message);
				}
				else
				{
					using (StreamReader streamRdr = new StreamReader(response.GetResponseStream()))
					{
						jsonString = streamRdr.ReadToEnd();

						streamRdr.Close();
					}
				}
			}

			Console.WriteLine(jsonString);

			return problemList;
		}

        private static List<Problem> problemKnowledgeSearch(KnowledgeSearch knowledgeSearch)
        {
            List<Problem> problemList = null;

            string jsonString = string.Empty;

            string urlString = "http://stsprod.servicetrackingsystems.net/ServiceTechService/problemKnowledgeSearch";
            // string urlString = "http://localhost:1923/problemKnowledgeSearch";

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            serializer.MaxJsonLength = 2147483647;

            jsonString = serializer.Serialize(knowledgeSearch);

            Byte[] requestData = Encoding.ASCII.GetBytes(jsonString);

            HttpWebRequest request = CreateWebRequest(urlString, "POST", requestData.Length);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(requestData, 0, requestData.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }
                else
                {
                    using (StreamReader streamRdr = new StreamReader(response.GetResponseStream()))
                    {
                        jsonString = streamRdr.ReadToEnd();

                        int length = jsonString.Length;

                        streamRdr.Close();
                    }
                }
            }

            Console.WriteLine(jsonString);

            return problemList;
        }

        private static List<Problem> searchProblemsByCategories(KnowledgeSearch knowledgeSearch)
        {
            List<Problem> problemList = null;

            string jsonString = string.Empty;

            string urlString = "http://stsprod.servicetrackingsystems.net/ServiceTechService/searchProblemsByCategories";
            // string urlString = "http://localhost:1923/searchProblemsByCategories";

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            serializer.MaxJsonLength = 2147483647;

            jsonString = serializer.Serialize(knowledgeSearch);

            Byte[] requestData = Encoding.ASCII.GetBytes(jsonString);

            HttpWebRequest request = CreateWebRequest(urlString, "POST", requestData.Length);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(requestData, 0, requestData.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }
                else
                {
                    using (StreamReader streamRdr = new StreamReader(response.GetResponseStream()))
                    {
                        jsonString = streamRdr.ReadToEnd();

                        streamRdr.Close();
                    }
                }
            }

            Console.WriteLine(jsonString);

            return problemList;
        }

        private static void uploadProblemBlob()
		{
			Byte[] BlobBytes = File.ReadAllBytes(".\\EvoApp_logo.png");

			//Byte[] BlobBytes = new Byte[]
			//{
			//    23, 25, 66
			//};

			Guid problemId = Guid.Parse("82712592-B422-4138-B551-72E37291A4F5");

			BlobPacket BlobPacket = new BlobPacket()
			{
				EntityId = problemId,
				BlobTypeId = Guid.Parse("A2C27AD5-D614-4AFE-A353-B3F7268D82A6"),
				BlobBytes = Convert.ToBase64String(BlobBytes)
			};

			JavaScriptSerializer serializer = new JavaScriptSerializer();

			serializer.MaxJsonLength = 2147483647;

			string jsonString = serializer.Serialize(BlobPacket);

			Byte[] requestData = Encoding.ASCII.GetBytes(jsonString);

			// string urlString = "http://www.ekeservices.net/ServicePleaseWebService/ProblemBlob";
			string urlString = "http://127.0.0.1:1923/ProblemBlob";

			HttpWebRequest request = CreateWebRequest(urlString, "POST", requestData.Length);

			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(requestData, 0, requestData.Length);
			}

			try
			{
				using (var response = (HttpWebResponse)request.GetResponse())
				{
					if (response.StatusCode != HttpStatusCode.OK)
					{
						string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
						throw new ApplicationException(message);
					}
				}
			}
			catch (WebException wex)
			{
				Console.WriteLine(string.Format("Web Exception occurred in uploadProblemBlob(). Error: {0}\n{1}",
												wex.Message,
												wex.InnerException != null ? wex.InnerException.Message : string.Empty));

				using (StreamReader streamReader = new StreamReader(wex.Response.GetResponseStream()))
				{
					string responseMessage = streamReader.ReadToEnd();

					Console.WriteLine(responseMessage);
				}
			}
		}

		private static void updateProblemBlob()
		{
			Byte[] BlobBytes = File.ReadAllBytes(".\\JackGuthrie1.jpg");

			//Byte[] BlobBytes = new Byte[]
			//{
			//    23, 25, 66
			//};

			Guid problemBlobId = Guid.Parse("F8099240-B512-412B-8DE1-3775CD286E4B");

			BlobPacket BlobPacket = new BlobPacket()
			{
				EntityId = problemBlobId,
				BlobTypeId = Guid.Parse("A2C27AD5-D614-4AFE-A353-B3F7268D82A6"),
				BlobBytes = Convert.ToBase64String(BlobBytes)
			};

			JavaScriptSerializer serializer = new JavaScriptSerializer();

			serializer.MaxJsonLength = 2147483647;

			string jsonString = serializer.Serialize(BlobPacket);

			Byte[] requestData = Encoding.ASCII.GetBytes(jsonString);

			string urlString = "http://www.ekeservices.net/ServicePleaseWebService/problemBlob";
			// string urlString = "http://127.0.0.1:1923/problemBlob";

			HttpWebRequest request = CreateWebRequest(urlString, "PUT", requestData.Length);

			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(requestData, 0, requestData.Length);
			}

			try
			{
				using (var response = (HttpWebResponse)request.GetResponse())
				{
					if (response.StatusCode != HttpStatusCode.OK)
					{
						string message = String.Format("PUT failed. Received HTTP {0}", response.StatusCode);
						throw new ApplicationException(message);
					}
				}
			}
			catch (WebException wex)
			{
				Console.WriteLine(string.Format("Web Exception occurred in updateProblemBlob(). Error: {0}\n{1}",
												wex.Message,
												wex.InnerException != null ? wex.InnerException.Message : string.Empty));

				using (StreamReader streamReader = new StreamReader(wex.Response.GetResponseStream()))
				{
					string responseMessage = streamReader.ReadToEnd();

					Console.WriteLine(responseMessage);
				}
			}
		}

		private static void deleteProblemBlob()
		{
			Guid problemBlobId = Guid.Parse("E6E683EB-FAF3-4490-BFE7-5F51CDF5AEAA");

			// string urlString = string.Format("http://www.ekeservices.net/ServicePleaseWebService/problemBlob?problemBlobId={0}", problemBlobId);
			string urlString = string.Format("http://127.0.0.1:1923/problemBlob?problemBlobId={0}", problemBlobId);

			HttpWebRequest request = CreateWebRequest(urlString, "DELETE", 0);

			try
			{
				using (var response = (HttpWebResponse)request.GetResponse())
				{
					if (response.StatusCode != HttpStatusCode.OK)
					{
						string message = String.Format("DELETE failed. Received HTTP {0}", response.StatusCode);
						throw new ApplicationException(message);
					}
				}
			}
			catch (WebException wex)
			{
				Console.WriteLine(string.Format("Web Exception occurred in deleteProblemBlob(). Error: {0}\n{1}",
												wex.Message,
												wex.InnerException != null ? wex.InnerException.Message : string.Empty));

				using (StreamReader streamReader = new StreamReader(wex.Response.GetResponseStream()))
				{
					string responseMessage = streamReader.ReadToEnd();

					Console.WriteLine(responseMessage);
				}
			}
		}

		private static void uploadSolutionBlob()
		{
			Byte[] BlobBytes = File.ReadAllBytes(".\\EvoApp_logo.png");

			//Byte[] BlobBytes = new Byte[]
			//{
			//    23, 25, 66
			//};

			Guid problemId = Guid.Parse("60DBAC42-E27A-4228-A677-EC1635BBAF0B");

			BlobPacket blobPacket = new BlobPacket()
			{
				EntityId = problemId,
				BlobTypeId = Guid.Parse("A2C27AD5-D614-4AFE-A353-B3F7268D82A6"),
				BlobBytes = Convert.ToBase64String(BlobBytes)
			};

			JavaScriptSerializer serializer = new JavaScriptSerializer();

			serializer.MaxJsonLength = 2147483647;

			string jsonString = serializer.Serialize(blobPacket);

			Byte[] requestData = Encoding.UTF8.GetBytes(jsonString);

			// string urlString = "http://www.ekeservices.net/ServicePleaseWebService/SolutionBlob";
			string urlString = "http://127.0.0.1.:1923/SolutionBlob";

			HttpWebRequest request = CreateWebRequest(urlString, "POST", requestData.Length);

			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(requestData, 0, requestData.Length);
			}

			try
			{
				using (var response = (HttpWebResponse)request.GetResponse())
				{
					if (response.StatusCode != HttpStatusCode.OK)
					{
						string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
						throw new ApplicationException(message);
					}
				}
			}
			catch (WebException wex)
			{
				Console.WriteLine(string.Format("Web Exception occurred in uploadProblemBlob(). Error: {0}\n{1}",
												wex.Message,
												wex.InnerException != null ? wex.InnerException.Message : string.Empty));

				using (StreamReader streamReader = new StreamReader(wex.Response.GetResponseStream()))
				{
					string responseMessage = streamReader.ReadToEnd();

					Console.WriteLine(responseMessage);
				}
			}
		}

        private static void sendEmailTest()
        {
            EmailElements emailElements = new EmailElements()
            {
                FromAddress = "edelliott@nc.rr.com",
                ToAddresses = new StringCollection() { "edelliott@nc.rr.com", "eelliott57@nc.rr.com", "ed.elliott@evoapp.com" },
                CCAddresses = new StringCollection() { "ncedelliott@gmail.com" },
                BodyText = "This is some body text for this test message.",
                SubjectText = "Test Email Message"
            };

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string jsonString = serializer.Serialize(emailElements);

            // string urlString = "http://www.ekeservices.net/ServicePleaseWebService/sendEmail";
            string urlString = "http://127.0.0.1:1372/sendEmail";

            byte[] bytes = Encoding.UTF8.GetBytes(jsonString);

            HttpWebRequest request = CreateWebRequest(urlString, "POST", bytes.Length);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    string message = String.Format("sendMailTest failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }
            }

            Console.WriteLine(jsonString);
        }

        private static void createNewCategory()
        {
            Category category = new Category();

            category.CategoryId = Guid.NewGuid();
            category.OrganizationId = Guid.Parse("0acfd1f3-6963-4025-ab2f-7028b06b11fd");
            category.CategoryIcon = null;
            category.CategoryName = "Eds Test Category";
            category.CreateDate = DateTime.UtcNow;
            category.EditDate = DateTime.UtcNow;

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string jsonString = serializer.Serialize(category);

            string urlString = "http://www.ekeservices.net/ServicePleaseWebService/category";

            byte[] bytes = Encoding.UTF8.GetBytes(jsonString);

            HttpWebRequest request = CreateWebRequest(urlString, "POST", bytes.Length);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }
            }  

            Console.WriteLine(jsonString);
        }

        private static void getTicketsByCreateDate()
        {
            string jsonString = string.Empty;

            string urlString = "http://www.ekeservices.net/ServicePleaseWebService/ticketsByHourElapsed?sortOrder=Oldest";
            // string urlString = "http://localhost:1372/ticketsByHourElapsed?sortOrder=Oldest";

            HttpWebRequest request = CreateWebRequest(urlString, "GET", 0);

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }
                else
                {
                    using (StreamReader streamRdr = new StreamReader(response.GetResponseStream()))
                    {
                        jsonString = streamRdr.ReadToEnd();

                        streamRdr.Close();
                    }
                }
            }

            Console.WriteLine(jsonString);
        }

        private static HttpWebRequest CreateWebRequest(string endPoint, string method, Int32 contentLength)
        {
            var request = (HttpWebRequest)WebRequest.Create(endPoint);

            try
            {
                request.Method = method;
                request.ContentLength = contentLength;
                request.ContentType = "application/json";
                request.Accept = "application/json";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in CreateWebRequest.  Error: {0}", ex.Message);

                return null;
            }

            return request;
        }

        private static void getTicketsByLocations()
        {
            string urlString = "http://www.ekeservices.net/ServicePleaseWebService/ticketsByLocations";
            // string urlString = "http://localhost:1923/ticketsByLocations";

            List<Guid> locationList = new List<Guid>();

            locationList.Add(Guid.Parse("6C3A6C98-5E73-49D1-A3F8-934B63A7A9D9"));
            locationList.Add(Guid.Parse("A8F867DA-D706-4B60-ACED-8C911D3A9A4F"));

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string jsonString = serializer.Serialize(locationList);

            byte[] bytes = Encoding.UTF8.GetBytes(jsonString);

            HttpWebRequest request = CreateWebRequest(urlString, "POST", bytes.Length);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }
                else
                {
                    using (StreamReader streamRdr = new StreamReader(response.GetResponseStream()))
                    {
                        jsonString = streamRdr.ReadToEnd();

                        streamRdr.Close();
                    }
                }
            }

            Console.WriteLine(jsonString);
        }
    }
}
