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
	public class IDController : Controller
	{

		IDReportingEntities db = new IDReportingEntities();


		#region Genel Parametreler

		[HttpGet]
		public ActionResult GenelParametreler()
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");
			return View(db.Parametrelers.Where((e) => e.Tip.Contains("Parametre_") && e.Silindi == false).ToList());
		}

		[HttpPost]
		public ActionResult GenelParametreler(string Parametre_MailEmail, string Parametre_MailKullaniciAdi, string Parametre_MailParola,
			string Parametre_MailServer, string Parametre_MailPort, string Parametre_MailSSL, string Parametre_Lisans)
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");
			{
				string tip = "Parametre_MailEmail";
				if (db.Parametrelers.Where((e) => e.Tip == tip).Count() > 0)
					db.Parametrelers.Remove(db.Parametrelers.Where((e) => e.Tip == tip).First());
				db.Parametrelers.Add(new Parametreler() { Tip = tip, Deger = Parametre_MailEmail, Silindi = false });
			}
			{
				string tip = "Parametre_MailKullaniciAdi";
				if (db.Parametrelers.Where((e) => e.Tip == tip).Count() > 0)
					db.Parametrelers.Remove(db.Parametrelers.Where((e) => e.Tip == tip).First());
				db.Parametrelers.Add(new Parametreler() { Tip = tip, Deger = Parametre_MailKullaniciAdi, Silindi = false });
			}
			{
				string tip = "Parametre_MailParola";
				if (db.Parametrelers.Where((e) => e.Tip == tip).Count() > 0)
					db.Parametrelers.Remove(db.Parametrelers.Where((e) => e.Tip == tip).First());
				db.Parametrelers.Add(new Parametreler() { Tip = tip, Deger = Parametre_MailParola, Silindi = false });
			}
			{
				string tip = "Parametre_MailServer";
				if (db.Parametrelers.Where((e) => e.Tip == tip).Count() > 0)
					db.Parametrelers.Remove(db.Parametrelers.Where((e) => e.Tip == tip).First());
				db.Parametrelers.Add(new Parametreler() { Tip = tip, Deger = Parametre_MailServer, Silindi = false });
			}
			{
				string tip = "Parametre_MailPort";
				if (db.Parametrelers.Where((e) => e.Tip == tip).Count() > 0)
					db.Parametrelers.Remove(db.Parametrelers.Where((e) => e.Tip == tip).First());
				db.Parametrelers.Add(new Parametreler() { Tip = tip, Deger = Parametre_MailPort, Silindi = false });
			}
			{
				string tip = "Parametre_MailSSL";
				if (db.Parametrelers.Where((e) => e.Tip == tip).Count() > 0)
					db.Parametrelers.Remove(db.Parametrelers.Where((e) => e.Tip == tip).First());
				db.Parametrelers.Add(new Parametreler() { Tip = tip, Deger = Parametre_MailSSL == "E" ? "E" : "H", Silindi = false });
			}

			db.SaveChanges();
			ViewBag.Mesaj = "Parametreler kaydedilmiştir.";
			return View(db.Parametrelers.Where((e) => e.Tip.Contains("Parametre_") && e.Silindi == false).ToList());
		}

		#endregion

		#region Sorgu İşlemleri

		public ActionResult Kategoriler(int KategoriID)
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");
			return View(db.Sorgulars.Where((e)=>e.KategoriID == KategoriID && e.Silindi == false && e.Aktif == true).ToList());
		}

		public JsonResult SorguTipiSec(string SorguTipi, int BaglantiID)
		{
			db.Configuration.ProxyCreationEnabled = false;
			Baglantilar entityBaglanti = db.Baglantilars.Where((e) => e.ID == BaglantiID).FirstOrDefault();
			SqlCommand cmd = new SqlCommand();
			switch (SorguTipi)
			{
				case "Table":
					cmd.CommandText = @"select * from ["+entityBaglanti.Veritabani+"].sys.tables";
					break;
				case "View":
					cmd.CommandText = @"select * from [" + entityBaglanti.Veritabani + "].sys.views";
					break;
				case "Procedure":
					cmd.CommandText = @"select * from [" + entityBaglanti.Veritabani + "].sys.procedures";
					break;
				default:
					break;
			}
			DataTable dt = (DataTable)IDVeritabani.Sorgula(cmd, SorgulaTuru.Tablo,BaglantiID);
			List<IDGrupKodu> results = new List<IDGrupKodu>();
			foreach (DataRow item in dt.Rows)
			{
				results.Add(new IDGrupKodu() { Isim = Convert.ToString(item["name"]) });
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Sorgular(string Tip = "")
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");
			return View(db.Sorgulars.Where((e) => e.Silindi == false).OrderBy((o) => o.KategoriID).OrderBy((o)=> o.Isim).ToList());
		}

		[HttpGet]
		public ActionResult Sorgu(int id = 0)
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");

			ViewBag.Baglantilar = db.Baglantilars.Where((e) => e.Silindi == false).ToList();
			ViewBag.Kategoriler = db.Parametrelers.Where((e) => e.Silindi == false && e.Tip == "Kategori").ToList();

			return View(db.Sorgulars.Where((e) => e.ID == id).FirstOrDefault());
		}
		[HttpPost]
		public ActionResult Sorgu(int id, string BaglantiID, string Aktif,string Isim,string KategoriID, string SorguMetni, string SorguTipi, string Sube)
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");
			if (id == 0)
			{
				Sorgular entity = new Sorgular();
				entity.SubeID = Convert.ToInt32(Sube);
				entity.KategoriID = Convert.ToInt32(KategoriID);
				entity.SorguTipi = SorguTipi;
				entity.Isim = Isim;
				entity.BaglantiID = Convert.ToInt32(BaglantiID);
				entity.Sorgu = SorguMetni;
				entity.Aktif = Aktif == "1" ? true : false;

				entity.KayitTarihi = DateTime.Now;
				entity.KayitYapanKullanici = GetCookie("KullaniciAdi");
				entity.Silindi = false;
				db.Sorgulars.Add(entity);
			}
			else
			{
				Sorgular entity = db.Sorgulars.Where((e) => e.ID == id).FirstOrDefault();
				entity.BaglantiID = Convert.ToInt32(BaglantiID);
				entity.Aktif = Aktif == "1" ? true : false;
				entity.Isim = Isim;
				entity.KategoriID = Convert.ToInt32(KategoriID);
				entity.Sorgu = SorguMetni;
				entity.SorguTipi = SorguTipi;
				entity.SubeID = Convert.ToInt32(Sube);

				entity.DuzenlemeTarihi = DateTime.Now;
				entity.DuzenlemeYapanKullanici = GetCookie("KullaniciAdi");
				entity.Silindi = false;
			}
			var degerler = db.GetValidationErrors();
			db.SaveChanges();
			return Redirect("~/ID/Sorgular");
		}

		public ActionResult SorguSil(int id)
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");
			Sorgular entity = db.Sorgulars.Where((e) => e.ID == id).First();
			entity.Silindi = true;
			entity.SilinenTarih = DateTime.Now;
			entity.SilenKullanici = GetCookie("KullaniciAdi");
			db.SaveChanges();
			return Redirect("~/ID/Sorgular");

		}

		#endregion

		#region Parametre İşlemleri

		public ActionResult Parametreler(string Tip="")
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");
			return View(db.Parametrelers.Where((e) => e.Silindi == false && e.Tip == Tip).OrderBy((o) => o.Deger).ToList());
		}

		[HttpGet]
		public ActionResult Parametre(int id = 0, string Tip = "")
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");

			return View(db.Parametrelers.Where((e) => e.ID == id).FirstOrDefault());
		}
		[HttpPost]
		public ActionResult Parametre(int id, string Tip = "", string Deger="")
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");
			if (id == 0)
			{
				Parametreler entity = new Parametreler();
				entity.Tip = Tip;
				entity.Deger = Deger;
				entity.KayitTarihi = DateTime.Now;
				entity.KayitYapanKullanici = GetCookie("KullaniciAdi");
				entity.Silindi = false;
				db.Parametrelers.Add(entity);
			}
			else
			{
				Parametreler entity = db.Parametrelers.Where((e) => e.ID == id).FirstOrDefault();
				entity.Deger = Deger;
				entity.DuzenlemeTarihi = DateTime.Now;
				entity.DuzenlemeYapanKullanici = GetCookie("KullaniciAdi");
				entity.Silindi = false;
			}
			db.SaveChanges();
			return Redirect("~/ID/Parametreler/?Tip=" + Tip);
		}

		public ActionResult ParametreSil(int id, string Tip = "")
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");
			Parametreler entity = db.Parametrelers.Where((e) => e.ID == id).First();
			entity.Silindi = true;
			entity.SilinenTarih = DateTime.Now;
			entity.SilenKullanici = GetCookie("KullaniciAdi");
			db.SaveChanges();
			return Redirect("~/ID/Parametreler/?Tip="+Tip);

		}

		#endregion

		#region Bağlantı İşlemleri

		public ActionResult Baglantilar()
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");

			return View(db.Baglantilars.Where((e) => e.Silindi == false).OrderBy((o) => o.Isim).ToList());
		}

		[HttpGet]
		public ActionResult Baglanti(int id = 0)
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");

			return View(db.Baglantilars.Where((e) => e.ID == id).FirstOrDefault());
		}
		[HttpPost]
		public ActionResult Baglanti(int id, string Sunucu, string Veritabani, string KullaniciAdi, string Parola, string Isim, int Port, string Sema, string Sube = "0")
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");
			if (id == 0)
			{
				Baglantilar entity = new Baglantilar();
				entity.SubeID = Convert.ToInt32(Sube);
				entity.Sunucu = Sunucu;
				entity.Veritabani = Veritabani;
				entity.KullaniciAdi = KullaniciAdi;
				entity.Parola = Parola;
				entity.Isim = Isim;
				entity.Port = Port;
				entity.Sema = Sema;
				entity.KayitTarihi = DateTime.Now;
				entity.KayitYapanKullanici = GetCookie("KullaniciAdi");
				entity.Silindi = false;
				db.Baglantilars.Add(entity);
			}
			else
			{
				Baglantilar entity = db.Baglantilars.Where((e) => e.ID == id).FirstOrDefault();
				entity.SubeID = Convert.ToInt32(Sube);
				entity.Sunucu = Sunucu;
				entity.Veritabani = Veritabani;
				entity.KullaniciAdi = KullaniciAdi;
				entity.Parola = Parola;
				entity.Isim = Isim;
				entity.Port = Port;
				entity.Sema = Sema;
				entity.DuzenlemeTarihi = DateTime.Now;
				entity.DuzenlemeYapanKullanici = GetCookie("KullaniciAdi");
				entity.Silindi = false;
			}
			db.SaveChanges();
			return Redirect("~/ID/Baglantilar");
		}

		public ActionResult BaglantiSil(int id)
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");
			Baglantilar entity = db.Baglantilars.Where((e) => e.ID == id).First();
			entity.Silindi = true;
			entity.SilinenTarih = DateTime.Now;
			entity.SilenKullanici = GetCookie("KullaniciAdi");
			db.SaveChanges();
			return Redirect("~/ID/Baglantilar");

		}

		#endregion

		#region Kullanıcı İşlemleri

		public ActionResult Kullanicilar()
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");

			return View(db.Kullanicilars.Where((e)=>e.Silindi == false).OrderBy((o)=>o.Isim).ToList());
		}

		[HttpGet]
		public ActionResult Kullanici(int id = 0)
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");

			return View(db.Kullanicilars.Where((e)=>e.ID == id).FirstOrDefault());
		}
		[HttpPost]
		public ActionResult Kullanici(int id,string KullaniciAdi, string Parola, string Isim, string Email, string Aktif="", string Sube="0")
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");
			if (id == 0)
			{
				Kullanicilar entity = new Kullanicilar();
				entity.Aktif = Aktif == "1" ? true : false;
				entity.Sube = Sube;
				entity.KullaniciAdi = KullaniciAdi;
				entity.Parola = Parola;
				entity.Isim = Isim;
				entity.Email = Email;
				entity.KayitTarihi = DateTime.Now;
				entity.KayitYapanKullanici = GetCookie("KullaniciAdi");
				entity.Silindi = false;
				db.Kullanicilars.Add(entity);
			}
			else
			{
				Kullanicilar entity = db.Kullanicilars.Where((e)=>e.ID == id).FirstOrDefault();
				entity.Aktif = Aktif == "1" ? true : false;
				entity.Sube = Sube;
				entity.KullaniciAdi = KullaniciAdi;
				entity.Parola = Parola;
				entity.Isim = Isim;
				entity.Email = Email;
				entity.DuzenlemeTarihi = DateTime.Now;
				entity.DuzenlemeYapanKullanici = GetCookie("KullaniciAdi");
				entity.Silindi = false;
			}
			db.SaveChanges();
			return Redirect("~/ID/Kullanicilar");
		}

		public ActionResult KullaniciSil(int id)
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");
			Kullanicilar entity = db.Kullanicilars.Where((e) => e.ID == id).First();
			entity.Silindi = true;
			entity.SilinenTarih = DateTime.Now;
			entity.SilenKullanici = GetCookie("KullaniciAdi");
			db.SaveChanges();
			return Redirect("~/ID/Kullanicilar");

		}

		#endregion



		#region Kullanıcı Girişi
		[HttpGet]
		public ActionResult Giris()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Giris(string KullaniciAdi, string Parola)
		{
			Kullanicilar entity = db.Kullanicilars.Where((e) => e.KullaniciAdi == KullaniciAdi && e.Parola == Parola && e.Silindi == false && e.Aktif == true).FirstOrDefault();
			if (entity != null)
			{
				CreateCookie("Isim", entity.Isim);
				CreateCookie("KullaniciAdi", KullaniciAdi);
				CreateCookie("Parola", Parola);
				return Redirect("~/ID/AnaSayfa");
			}
			else
			{
				ViewBag.Mesaj = "Kullanıcı adı veya parola yanlış.";
			}

			return View();
		}
		public ActionResult Cikis()
		{
			DeleteCookie("KullaniciAdi");
			DeleteCookie("Parola");
			return Redirect("~/ID/AnaSayfa");
		}
		#endregion
		public ActionResult AnaSayfa()
		{
			if (!AutoGirisKontrol())
				return Redirect("~/ID/Giris");
			
			return View();
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