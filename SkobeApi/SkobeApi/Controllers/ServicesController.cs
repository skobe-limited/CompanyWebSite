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
using Dapper;

namespace SkobeApi.Controllers
{
    public class ServicesController : ApiController
    {
        IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString);

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
        }

        [HttpGet]
        [ActionName("Test")]
        public IHttpActionResult Test()
        {
            return Ok("Test Ok");
        }

        [HttpPost]
        [ActionName("OilSamplingSave")]
        public IHttpActionResult OilSamplingSave(OilSampling os)
        {
            if (os != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\n INSERT INTO [Services-Aydas-OilSampling] (RecordDate, CustomerName, SubstationName, Country, SamplingDate, CompanyName, ContactPersonName, Phone, Email, CompanyAddress, SampleNr, OilTemp, AmbientTemp, OilName, Language, LanguageOther, OilReportStandard, PlaceOfSampling, Rating, PrimaryVoltage, SecondaryVoltage, Frequency, SerialNumber, YearOfManufacture, OffLoad, OnLoad, OilAge, CoolingType, OilType, EquipmentType) ");
                sb.Append("\n VALUES (GETDATE(),'"+ Clean(os.CustomerName) + "', '"+Clean(os.SubstationName) +"', '"+ Clean(os.Country) +"', '"+Clean(os.SamplingDate) +"', '"+Clean(os.CompanyName) +"', '"+Clean(os.ContactPersonName) +"', '"+ Clean(os.Phone) +"', '"+Clean(os.Email) +"', '"+Clean(os.CompanyAddress) +"', '"+ Clean(os.SampleNr) +"', '"+ Clean(os.OilTemp) +"', '"+Clean(os.AmbientTemp) +"', '"+ Clean(os.OilName) +"', '"+ Clean(os.Language) +"', '"+ Clean(os.LanguageOther) +"', '"+ Clean(os.OilReportStandard) +"', '"+ Clean(os.PlaceOfSampling) +"', '"+ Clean(os.Rating) +"', '"+ Clean(os.PrimaryVoltage) + "', '"+ Clean(os.SecondaryVoltage) +"', '"+ Clean(os.Frequency) +"', '"+ Clean(os.SerialNumber) +"', '"+ Clean(os.YearOfManufacture) +"', '"+ Clean(os.OffLoad) +"', '"+ Clean(os.OnLoad) +"', '"+ Clean(os.OilAge) +"', '"+ Clean(os.CoolingType) +"', '"+Clean(os.OilType) +"', '"+ Clean(os.EquipmentType) +"')");
                db.Execute(sb.ToString());
                return Ok("OK");
            }
            else
            {
                return Ok(0);
            }
        }


        public string Clean(string StringToClean)
        {
            if (StringToClean != null)
            {
                StringToClean = StringToClean.Replace("'", "´");
                StringToClean = StringToClean.Replace("<", "");
                StringToClean = StringToClean.Replace(">", "");
                StringToClean = StringToClean.Trim();
            }
            else
                StringToClean = "";
            return StringToClean;
        }

    }
}
