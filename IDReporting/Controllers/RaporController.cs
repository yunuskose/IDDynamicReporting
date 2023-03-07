using IDReporting.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IDReporting.Controllers
{
	public class RaporController : Controller
	{
		IDReportingEntities db = new IDReportingEntities();
		// GET: Sorgu
		public ActionResult Sorgu(int id = 0)
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");
			if (id == 0)
			{
				return Redirect("~/ID/Giris");
			}
			Sorgular entity = db.Sorgulars.Where((e) => e.ID == id).First();
			return View(entity);
		}

		public ActionResult Goster(int id, FormCollection formCollection)
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");

			Sorgular entitySorgu = db.Sorgulars.Where((e) => e.ID == id).First();
			ViewBag.SorguID = entitySorgu.ID;

			Baglantilar entityBaglanti = db.Baglantilars.Where((e) => e.ID == entitySorgu.BaglantiID).FirstOrDefault();

			db.Filtrelers.RemoveRange(db.Filtrelers.Where((e)=>e.SorguID == entitySorgu.ID));

			DataSet ds = new DataSet();

			string kisit = " Where 1=1";
			string kisitProcedure = " ";
			#region Kisit Ayarlanması
			foreach (var item in formCollection.AllKeys)
			{
				if (item.StartsWith("Secim_"))
				{
					string _id = item.Split('_')[2];
					string kolonadi = formCollection.AllKeys.Where((e) => e.EndsWith("_" + _id)).ToList()[1].Split('_')[1];
					string kolontipi = formCollection.AllKeys.Where((e) => e.EndsWith("_" + _id)).ToList()[1].Split('_')[2];
					string FiltreTipi = formCollection.AllKeys.Where((e) => e.EndsWith("_" + _id)).ToList()[2];
					string deger = "";
					try
					{
						deger = formCollection.AllKeys.Where((e) => e.EndsWith("_" + _id)).ToList()[3];
					}
					catch
					{
						deger = "Deger_"+ kolonadi + "_"+_id;
					}
					string isaret = formCollection.Get(FiltreTipi);
					string kisitDeger = formCollection.Get(deger);
					if (kolontipi == "Boolean" && kisitDeger == null)
					{
						kisitDeger = "0";
					}
					else if (kolontipi == "Boolean" && kisitDeger != null)
					{
						kisitDeger = "1";
					}
					kisit += " and " + kolonadi + " " + isaret + " '" + kisitDeger + "'";
					kisitProcedure += " '" + kisitDeger + "' ";
					Filtreler entityFiltre = new Filtreler();
					entityFiltre.Silindi = false;
					entityFiltre.KayitTarihi = DateTime.Now;
					entityFiltre.KayitYapanKullanici = GetCookie("KullaniciAdi");
					entityFiltre.SorguID = entitySorgu.ID;
					entityFiltre.FiltreTipi = isaret;
					entityFiltre.ParemetreAdi = kolonadi;
					entityFiltre.ParametreDegeri = kisitDeger;
					db.Filtrelers.Add(entityFiltre);
				}
			}
			db.SaveChanges();

			#endregion


		SqlCommand cmd = new SqlCommand();
			switch (entitySorgu.SorguTipi)
			{
				case "Table":
					cmd.CommandText = @"select * from [" + entityBaglanti.Veritabani + "]." + entityBaglanti.Sema + "." + entitySorgu.Sorgu+" "+kisit;
					break;
				case "View":
					cmd.CommandText = @"select * from [" + entityBaglanti.Veritabani + "]." + entityBaglanti.Sema + "." + entitySorgu.Sorgu + " " + kisit;
					break;
				case "Procedure":
					cmd.CommandText = @"Exec [" + entityBaglanti.Veritabani + "]."+ entityBaglanti.Sema + "."+entitySorgu.Sorgu + " " + kisitProcedure;
					break;
				default:
					break;
			}

			ds = (DataSet)IDVeritabani.Sorgula(cmd, SorgulaTuru.DataSet,entityBaglanti.ID);
			
			return View(ds);
		}
		
		public ActionResult GorunumKaydet(int SorguID, string Basliklar)
		{
			string _kullanici = GetCookie("KullaniciAdi");
			if (db.Gorunumlers.Where((e) => e.Kullanici == _kullanici && e.SorguID == SorguID).Count() > 0)
			{
				db.Gorunumlers.RemoveRange(db.Gorunumlers.Where((e) => e.Kullanici == _kullanici && e.SorguID == SorguID));
			}
			db.Gorunumlers.Add(new Gorunumler() {
					Kullanici = GetCookie("KullaniciAdi"),
					SorguID = SorguID,
					EkAciklama1 = Basliklar
			});
			db.SaveChanges();
			return Json("", JsonRequestBehavior.AllowGet);
		}


		#region Kullanıcı Giriş İşlemleri

		public bool AutoGirisKontrol()
		{
			bool GirisKontrol = false;

			string KullaniciAdi = GetCookie("KullaniciAdi");
			string Parola = GetCookie("Parola");

			if (KullaniciAdi != null)
			{
				if (db.Kullanicilars.Where((e) => e.KullaniciAdi == KullaniciAdi && e.Parola == Parola && e.Silindi == false && e.Aktif == true).Count() > 0)
				{
					GirisKontrol = true;
				}
				else
				{
					GirisKontrol = false;
				}
			}

			return GirisKontrol;
		}

		#endregion

		#region Cookie İşlemleri

		private void CreateCookie(string name, string value)
		{
			HttpCookie cookieVisitor = new HttpCookie(name, Server.UrlEncode(value));
			// cookieVisitor.Expires = DateTime.Now.AddDays(2);
			Response.Cookies.Add(cookieVisitor);
		}
		private string GetCookie(string name)
		{
			//Böyle bir cookie mevcut mu kontrol ediyoruz
			if (Request.Cookies.AllKeys.Contains(name))
			{
				//böyle bir cookie varsa bize geri değeri döndürsün
				return Server.UrlDecode(Request.Cookies[name].Value);
			}
			return null;
		}
		private void DeleteCookie(string name)
		{
			//Böyle bir cookie var mı kontrol ediyoruz
			if (GetCookie(name) != null)
			{
				//Varsa cookiemizi temizliyoruz
				Response.Cookies.Remove(name);
				//ya da 
				Response.Cookies[name].Expires = DateTime.Now.AddDays(-1);
			}
		}

		#endregion
	}
}