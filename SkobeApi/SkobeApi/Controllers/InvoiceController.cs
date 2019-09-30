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
	public class InvoiceController : ApiController
    {
		IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString);
		Helpers h = new Helpers();

		public class Invoice
		{
			public int InvoiceNumber { get; set; }
			public string BilledTo { get; set; }
			public string BilledToAddress { get; set; }
			public int BilledToCompanyRegNr { get; set; }
			public DateTime InvoiceDate { get; set; }

			public int Status { get; set; }
			public List<InvoiceDetail> details { get; set; }
		}

		public class InvoiceDetail
		{
			public string Description { get; set; }
			public float Rate { get; set; }
			public int Quantity { get; set; }
		}

		public class InvoiceListItem
		{
			public int InvoiceNumber { get; set; }
			public string BilledTo { get; set; }
			public string BilledToAddress { get; set; }
			public int BilledToCompanyRegNr { get; set; }
			public DateTime InvoiceDate { get; set; }
			public int Status { get; set; }
			public double InvoiceTotal { get; set; }
		}

		[HttpGet]
		[ActionName("ViewInvoice")]
		public IHttpActionResult ViewInvoice(string InvoiceNumber)
		{
			Invoice inv = db.QuerySingle<Invoice>("SELECT * FROM [skobeInvoice] WHERE InvoiceNumber=" + InvoiceNumber);
			inv.details = db.Query<InvoiceDetail>("SELECT * FROM [skobeInvoiceDetail] WHERE InvoiceNumber = " + InvoiceNumber + " ORDER BY Id").ToList();
			return Ok(inv);
		}

		[HttpGet]
		[ActionName("InvoiceList")]
		public IHttpActionResult InvoiceList()
		{
			string SQL = string.Format(@"
				SELECT T1.*,T2.InvoiceTotal
				FROM skobeInvoice T1
				INNER JOIN (
					SELECT InvoiceNumber,SUM(Rate*Quantity) as InvoiceTotal
					FROM skobeInvoiceDetail
					GROUP BY InvoiceNumber
				) T2 ON T1.InvoiceNumber=T2.InvoiceNumber
			");
			return Ok(db.Query<InvoiceListItem>(SQL).ToList());
		}


		[HttpPost]
		[ActionName("InvoiceSave")]
		public IHttpActionResult InvoiceSave(Invoice inv)
		{
			if (inv != null && inv.details!=null && inv.details.Count>0)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(" DECLARE @InvoiceNumber int;");
				//sb.Append(" SET LANGUAGE 'turkish'; SET DATEFORMAT DMY; ");
				sb.Append("\n INSERT INTO [skobeInvoice] (BilledTo,BilledToAddress,BilledToCompanyRegNr,InvoiceDate,Status) ");
				sb.Append("\n VALUES ('" + h.Capitalize(h.Clean(inv.BilledTo)) + "', '" + h.Capitalize(h.Clean(inv.BilledToAddress)) + "', " + inv.BilledToCompanyRegNr + ", '" + inv.InvoiceDate.ToString("yyyy-MM-dd") + "',"+inv.Status+")");
				sb.Append("\n SET @InvoiceNumber=@@IDENTITY; ");
				for (int i = 0; i < inv.details.Count; i++)
				{
					sb.Append("\n INSERT INTO [skobeInvoiceDetail] (InvoiceNumber, Description,Rate,Quantity) ");
					sb.Append("\n VALUES (@InvoiceNumber,'" + h.Capitalize(h.Clean(inv.details[i].Description)) + "', " + inv.details[i].Rate.ToString().Replace(",",".") + ", " + inv.details[i].Quantity + ")");
				}
				sb.Append("\n SELECT @InvoiceNumber as Id ");
				int newInvoiceNumber = Convert.ToInt32(db.ExecuteScalar(sb.ToString()));

				return Ok("OK");
			}
			else
			{
				return Ok(0);
			}
		}

	}
}
