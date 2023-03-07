# IDDynamicReporting

PROJENİN ÖZELLİKLERİ
=========================================================
- Sql Bağlantısı Ekleyebilme
- Sql Bağlantısında Rapor Oluşturma (table, view, procedure)
- Raporları alırken dinamik olarak tablo kolonlarını getirerek filtrelemesinin sağlanması.
- Raporları alırken dinamik olarak view kolonlarını getirerek  filtrelenmesinin sağlanması.
- Raporları alırken dinamik olarak procedure parametrelerinin filtreleme kısmına getirilmesi.
- Kategori Oluşturma ve kategori altına raporların oluşturulması.

Web Configteki "IDReportingEntities" bağlantısının sql veritabanı bilgilerini kendinize göre düzenlemeniz gerekmektedir.

Projenin çalışması için veritabanında aşağıdaki tabloların oluşturulması gerekmektedir. 

USE [DATABASENAME]
GO
/****** Object:  Table [dbo].[Baglantilar]    Script Date: 7.3.2023 11:10:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Baglantilar](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SubeID] [int] NULL,
	[Isim] [nvarchar](100) NULL,
	[Sunucu] [nvarchar](100) NULL,
	[Veritabani] [nvarchar](100) NULL,
	[KullaniciAdi] [nvarchar](100) NULL,
	[Parola] [nvarchar](100) NULL,
	[Port] [int] NULL,
	[Sema] [nvarchar](50) NULL CONSTRAINT [DF_Baglantilar_Sema]  DEFAULT (N'dbo'),
	[KayitTarihi] [datetime] NULL CONSTRAINT [DF_Baglantilar_KayitTarihi]  DEFAULT (getdate()),
	[KayitYapanKullanici] [nvarchar](max) NULL,
	[DuzenlemeTarihi] [datetime] NULL,
	[DuzenlemeYapanKullanici] [nvarchar](100) NULL,
	[Silindi] [bit] NULL CONSTRAINT [DF_Baglantilar_Silindi]  DEFAULT ((0)),
	[SilinenTarih] [datetime] NULL,
	[SilenKullanici] [nvarchar](100) NULL,
 CONSTRAINT [PK_Baglantilar] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Filtreler]    Script Date: 7.3.2023 11:10:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Filtreler](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SorguID] [int] NULL,
	[ParametreTipi] [nvarchar](100) NULL,
	[FiltreTipi] [nvarchar](100) NULL,
	[ParemetreAdi] [nvarchar](100) NULL,
	[ParametreDegeri] [nvarchar](100) NULL,
	[KayitTarihi] [datetime] NULL,
	[KayitYapanKullanici] [nvarchar](max) NULL,
	[DuzenlemeTarihi] [datetime] NULL,
	[DuzenlemeYapanKullanici] [nvarchar](100) NULL,
	[Silindi] [bit] NULL CONSTRAINT [DF_Filtreler_Silindi]  DEFAULT ((0)),
	[SilinenTarih] [datetime] NULL,
	[SilenKullanici] [nvarchar](100) NULL,
 CONSTRAINT [PK_Filtreler] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Gorunumler]    Script Date: 7.3.2023 11:10:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gorunumler](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Kullanici] [nvarchar](100) NULL,
	[SorguID] [int] NULL,
	[SonucTipi] [nvarchar](100) NULL,
	[EkAciklama1] [nvarchar](max) NULL,
	[EkAciklama2] [nvarchar](max) NULL,
	[EkAciklama3] [nvarchar](max) NULL,
	[EkAciklama4] [nvarchar](max) NULL,
	[EkAciklama5] [nvarchar](max) NULL,
 CONSTRAINT [PK_Gorunumler] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Kullanicilar]    Script Date: 7.3.2023 11:10:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Kullanicilar](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Sube] [nvarchar](100) NULL,
	[KullaniciAdi] [nvarchar](100) NULL,
	[Parola] [nvarchar](100) NULL,
	[Isim] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Aktif] [bit] NULL CONSTRAINT [DF_Kullanicilar_Aktif]  DEFAULT ((1)),
	[KayitTarihi] [datetime] NULL,
	[KayitYapanKullanici] [nvarchar](max) NULL,
	[DuzenlemeTarihi] [datetime] NULL,
	[DuzenlemeYapanKullanici] [nvarchar](100) NULL,
	[Silindi] [bit] NULL CONSTRAINT [DF_Kullanicilar_Silindi]  DEFAULT ((0)),
	[SilinenTarih] [datetime] NULL,
	[SilenKullanici] [nvarchar](100) NULL,
 CONSTRAINT [PK_Kullanicilar] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Parametreler]    Script Date: 7.3.2023 11:10:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Parametreler](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Tip] [nvarchar](100) NULL,
	[Deger] [nvarchar](100) NULL,
	[KayitTarihi] [datetime] NULL,
	[KayitYapanKullanici] [nvarchar](max) NULL,
	[DuzenlemeTarihi] [datetime] NULL,
	[DuzenlemeYapanKullanici] [nvarchar](100) NULL,
	[Silindi] [bit] NULL CONSTRAINT [DF_Parametreler_Silindi]  DEFAULT ((0)),
	[SilinenTarih] [datetime] NULL,
	[SilenKullanici] [nvarchar](100) NULL,
 CONSTRAINT [PK_Parametreler] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Sorgular]    Script Date: 7.3.2023 11:10:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sorgular](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SubeID] [int] NULL,
	[KategoriID] [int] NULL,
	[SorguTipi] [nvarchar](max) NULL,
	[Isim] [nvarchar](max) NULL,
	[BaglantiID] [int] NULL,
	[Sorgu] [nvarchar](max) NULL,
	[Aktif] [bit] NULL CONSTRAINT [DF_Sorgular_Aktif]  DEFAULT ((1)),
	[KayitTarihi] [datetime] NULL,
	[KayitYapanKullanici] [nvarchar](max) NULL,
	[DuzenlemeTarihi] [datetime] NULL,
	[DuzenlemeYapanKullanici] [nvarchar](100) NULL,
	[Silindi] [bit] NULL CONSTRAINT [DF_Sorgular_Silindi]  DEFAULT ((0)),
	[SilinenTarih] [datetime] NULL,
	[SilenKullanici] [nvarchar](100) NULL,
 CONSTRAINT [PK_Sorgular] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Yenilikler]    Script Date: 7.3.2023 11:10:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Yenilikler](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Tarih] [datetime] NULL CONSTRAINT [DF_Yenilikler_Tarih]  DEFAULT (getdate()),
	[Aciklama] [nvarchar](max) NULL,
 CONSTRAINT [PK_Yenilikler] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
