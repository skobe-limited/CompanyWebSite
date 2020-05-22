using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using Dapper;
using SkobeApi.Common;

namespace SkobeApi.Controllers
{
	[EnableCors(origins: "*", headers: "*", methods: "*")]
	public class ServicesController : ApiController
    {
        IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString);
		Helpers h = new Helpers();

        public class OilSampling
        {
            public string CustomerName { get; set; }
            public string SubstationName { get; set; }
            public string Country { get; set; }
            public string SamplingDate { get; set; }
            public string CompanyName { get; set; }
            public string ContactPersonName { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string CompanyAddress { get; set; }

			public List<OilSample> samples { get; set; }
        }

		public class OilSample
		{
			public string SampleNr { get; set; }
			public string OilTemp { get; set; }
			public string AmbientTemp { get; set; }
			public string OilName { get; set; }
			public string Language { get; set; }
			public string LanguageOther { get; set; }
			public string OilReportStandard { get; set; }
			public string PlaceOfSampling { get; set; }
			public string Rating { get; set; }
			public string PrimaryVoltage { get; set; }
			public string SecondaryVoltage { get; set; }
			public string Frequency { get; set; }
			public string SerialNumber { get; set; }
			public string YearOfManufacture { get; set; }
			public string OffLoad { get; set; }
			public string OnLoad { get; set; }
			public string OilAge { get; set; }
			public string CoolingType { get; set; }
			public string OilType { get; set; }
			public string EquipmentType { get; set; }
			public string Humidity { get; set; }
			public string RequestedTests { get; set; }
			public string ReasonForSampling { get; set; }
		}

		[HttpGet]
        [ActionName("Test")]
        public IHttpActionResult Test()
        {
			//StringBuilder body = new StringBuilder();
			//body.Append("<h3>title</h3>");
			//body.Append("<b>date</b>");
			//SendMail("cagatay.bulanik@gmail.com", "Oil Sampling Form", body.ToString());
			return Ok("Test Ok");
        }

		[HttpGet]
		[ActionName("ViewForm")]
		public IHttpActionResult ViewForm(string FormId)
		{
			OilSampling os = db.QuerySingle<OilSampling>("SELECT * FROM [Services-Aydas-OilSampling] WHERE Id=" + FormId);
			os.samples = db.Query<OilSample>("SELECT * FROM [Services-Aydas-OilSampling-Detail] WHERE SampleId = " + FormId + " ORDER BY Id").ToList();
			return Ok(os);
		}

		[HttpPost]
        [ActionName("OilSamplingSave")]
        public IHttpActionResult OilSamplingSave(OilSampling os)
        {
            if (os != null)
            {
                StringBuilder sb = new StringBuilder();
				sb.Append("DECLARE @Id int;");
				sb.Append("\n INSERT INTO [Services-Aydas-OilSampling] (RecordDate, CustomerName, SubstationName, Country, SamplingDate, CompanyName, ContactPersonName, Phone, Email, CompanyAddress) ");
				sb.Append("\n VALUES (GETDATE(),'" + h.Clean(os.CustomerName) + "', '" + h.Clean(os.SubstationName) + "', '" + h.Clean(os.Country) + "', '" + h.Clean(os.SamplingDate) + "', '" + h.Clean(os.CompanyName) + "', '" + h.Clean(os.ContactPersonName) + "', '" + h.Clean(os.Phone) + "', '" + h.Clean(os.Email) + "', '" + h.Clean(os.CompanyAddress) + "')");
				sb.Append("\n SET @Id=@@IDENTITY; ");
				for (int i = 0; i < os.samples.Count; i++)
				{
					sb.Append("\n INSERT INTO [Services-Aydas-OilSampling-Detail] (SampleId, SampleNr, OilTemp, AmbientTemp, OilName, Language, LanguageOther, OilReportStandard, PlaceOfSampling, Rating, PrimaryVoltage, SecondaryVoltage, Frequency, SerialNumber, YearOfManufacture, OffLoad, OnLoad, OilAge, CoolingType, OilType, EquipmentType,Humidity,RequestedTests,ReasonForSampling) ");
					sb.Append("\n VALUES (@Id,'" + h.Clean(os.samples[i].SampleNr) + "', '" + h.Clean(os.samples[i].OilTemp) + "', '" + h.Clean(os.samples[i].AmbientTemp) + "', '" + h.Clean(os.samples[i].OilName) + "', '" + h.Clean(os.samples[i].Language) + "', '" + h.Clean(os.samples[i].LanguageOther) + "', '" + h.Clean(os.samples[i].OilReportStandard) + "', '" + h.Clean(os.samples[i].PlaceOfSampling) + "', '" + h.Clean(os.samples[i].Rating) + "', '" + h.Clean(os.samples[i].PrimaryVoltage) + "', '" + h.Clean(os.samples[i].SecondaryVoltage) + "', '" + h.Clean(os.samples[i].Frequency) + "', '" + h.Clean(os.samples[i].SerialNumber) + "', '" + h.Clean(os.samples[i].YearOfManufacture) + "', '" + h.Clean(os.samples[i].OffLoad) + "', '" + h.Clean(os.samples[i].OnLoad) + "', '" + h.Clean(os.samples[i].OilAge) + "', '" + h.Clean(os.samples[i].CoolingType) + "', '" + h.Clean(os.samples[i].OilType) + "', '" + h.Clean(os.samples[i].EquipmentType) + "', '" + h.Clean(os.samples[i].Humidity) + "', '" + h.Clean(os.samples[i].RequestedTests) + "', '" + h.Clean(os.samples[i].ReasonForSampling) + "')");
				}
				sb.Append("\n SELECT @Id as Id ");
				int newId=Convert.ToInt32(db.ExecuteScalar(sb.ToString()));
				StringBuilder body = new StringBuilder();
				body.Append("<h3>" + os.CustomerName + "</h3>");
				body.Append("<p>Sampling Date: <b>" + os.SamplingDate + "</b></p>");
				body.Append("<p><a href='http://skobe.co.uk/services/aydas/oilsampling/ViewForm.html?FormId="+newId.ToString()+"'>Click here</a> to view form...</p>");
				h.SendMail("ahmet@aydas.co.uk", "Oil Sampling Form", body.ToString());
				h.SendMail("cagatay.bulanik@gmail.com", "Oil Sampling Form", body.ToString());
				return Ok("OK");
            }
            else
            {
                return Ok(0);
            }
        }

    }
}
