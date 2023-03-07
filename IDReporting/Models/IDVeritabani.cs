using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace IDReporting.Models
{
	public class IDVeritabani
	{
		public static IDReportingEntities db = new IDReportingEntities();
		public static object Sorgula(SqlCommand cmd, SorgulaTuru tip, int BaglantiID)
		{
			object sonuc = null;
			Baglantilar entity = db.Baglantilars.Where((e)=>e.ID == BaglantiID).FirstOrDefault();
			string connectionString = "data source=" + entity.Sunucu+","+entity.Port+ ";initial catalog=" + entity.Veritabani+ ";user id=" + entity.KullaniciAdi+";password="+entity.Parola+";";
			using (SqlConnection baglanti = new SqlConnection(connectionString))
			{
				baglanti.Open();
				cmd.Connection = baglanti;
				cmd.CommandTimeout = 300;

				switch (tip)
				{
					case SorgulaTuru.Bos:
						return cmd.ExecuteNonQuery();
						break;
					case SorgulaTuru.Tek:
						return cmd.ExecuteScalar();
						break;
					case SorgulaTuru.Tablo:
						DataTable dt = new DataTable();
						SqlDataAdapter adapter = new SqlDataAdapter();
						adapter.SelectCommand = cmd;
						adapter.SelectCommand.CommandTimeout = 30;
						adapter.Fill(dt);
						return dt;
						break;
					case SorgulaTuru.DataSet:
						DataSet ds = new DataSet();
						SqlDataAdapter adapter2 = new SqlDataAdapter();
						adapter2.SelectCommand = cmd;
						adapter2.SelectCommand.CommandTimeout = 60;
						adapter2.Fill(ds);
						return ds;
						break;
				}
			}


			return sonuc;
		}
	}

	public enum SorgulaTuru
	{
		Bos,
		Tek,
		Tablo,
		DataSet
	}
}