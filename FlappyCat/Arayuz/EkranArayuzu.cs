using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlappyCat
{
    /// <summary>
    /// Ekranların (karakter seçimi, oyun içi göstergeler, menü, oyun bitti)
    /// çizimini ve fare/klavye girdilerini yönetir.
    /// </summary>
    public class EkranArayuzu
    {
        private const float BitisBekleme = 30f;   // oyun bittikten sonra butonların gelmesi için beklenen kare

        private static readonly KarakterTuru[] Karakterler = { KarakterTuru.Gri, KarakterTuru.Turuncu, KarakterTuru.Siyah };

        private readonly OyunYoneticisi oyun;
        private readonly RectangleF[] karakterKartlari = new RectangleF[3];

        private readonly Buton btnPlay;
        private readonly Buton btnMenu;
        private readonly Buton btnDevam;
        private readonly Buton btnSes;
        private readonly Buton btnCik;
        private readonly Buton btnTekrar;
        private readonly Buton btnAnaMenu;
        private readonly Buton[] tumButonlar;

        public EkranArayuzu(OyunYoneticisi oyun)
        {
            this.oyun = oyun;
            float orta = Ayarlar.EkranGenislik / 2f;

            for (int i = 0; i < karakterKartlari.Length; i++)
                karakterKartlari[i] = new RectangleF(orta - 66 + (i - 1) * 146, 222, 132, 142);

            Size play = Varliklar.ButonPlay.Size, menu = Varliklar.ButonMenu.Size, kucuk = Varliklar.ButonMenuKucuk.Size;
            btnPlay = new Buton(new RectangleF(orta - play.Width / 2f, 400, play.Width, play.Height), Varliklar.ButonPlay);
            btnMenu = new Buton(new RectangleF(Ayarlar.EkranGenislik - kucuk.Width - 12, 24, kucuk.Width, kucuk.Height), Varliklar.ButonMenuKucuk);

            btnDevam = new Buton(new RectangleF(orta - 125, 272, 250, 62), "Devam Et", Varliklar.ButonDevam);
            btnSes = new Buton(new RectangleF(orta - 125, 348, 250, 62), "Ses: Açık", Varliklar.PatiPembe);
            btnCik = new Buton(new RectangleF(orta - 125, 424, 250, 62), "Çık", Varliklar.PatiKrem);

            btnTekrar = new Buton(new RectangleF(orta - play.Width / 2f, 452, play.Width, play.Height), Varliklar.ButonPlay);
            btnAnaMenu = new Buton(new RectangleF(orta - menu.Width / 2f, 540, menu.Width, menu.Height), Varliklar.ButonMenu);

            tumButonlar = new[] { btnPlay, btnMenu, btnDevam, btnSes, btnCik, btnTekrar, btnAnaMenu };
        }

        private bool BitisButonlariHazir
        {
            get { return oyun.Durum == OyunDurumu.OyunBitti && oyun.BitisSuresi >= BitisBekleme; }
        }

        /// <summary>O anki ekranda görünen butonlar.</summary>
        private Buton[] AktifButonlar()
        {
            switch (oyun.Durum)
            {
                case OyunDurumu.AnaMenu: return new[] { btnPlay };
                case OyunDurumu.Hazir:
                case OyunDurumu.Oynuyor: return new[] { btnMenu };
                case OyunDurumu.Duraklatildi: return new[] { btnDevam, btnSes, btnCik };
                case OyunDurumu.OyunBitti: return BitisButonlariHazir ? new[] { btnTekrar, btnAnaMenu } : new Buton[0];
                default: return new Buton[0];
            }
        }

        // ---------------- Girdiler ----------------

        public void FareTiklandi(Point nokta)
        {
            switch (oyun.Durum)
            {
                case OyunDurumu.AnaMenu:
                    for (int i = 0; i < karakterKartlari.Length; i++)
                        if (karakterKartlari[i].Contains(nokta)) oyun.KarakterSec(Karakterler[i]);
                    if (btnPlay.Icerir(nokta)) PlayaBasildi();
                    break;

                case OyunDurumu.Hazir:
                case OyunDurumu.Oynuyor:
                    if (btnMenu.Icerir(nokta)) MenuyeBasildi();
                    else oyun.Zipla();                           // ekrana tıklamak da zıplatır
                    break;

                case OyunDurumu.Duraklatildi:
                    if (btnDevam.Icerir(nokta)) oyun.DevamEt();
                    else if (btnSes.Icerir(nokta)) oyun.SesiAcKapat();
                    else if (btnCik.Icerir(nokta)) oyun.AnaMenuyeDon();
                    break;

                case OyunDurumu.OyunBitti:
                    if (!BitisButonlariHazir) break;
                    if (btnTekrar.Icerir(nokta)) PlayaBasildi();
                    else if (btnAnaMenu.Icerir(nokta)) AnaMenuyeBasildi();
                    break;
            }
        }

        /// <summary>Fare bir butonun veya karakter kartının üstündeyse true döner.</summary>
        public bool FareHareket(Point nokta)
        {
            foreach (Buton b in tumButonlar) b.UzerindeMi = false;

            bool el = false;
            foreach (Buton b in AktifButonlar())
            {
                b.UzerindeMi = b.Icerir(nokta);
                el |= b.UzerindeMi;
            }
            if (oyun.Durum == OyunDurumu.AnaMenu)
                foreach (RectangleF kart in karakterKartlari)
                    el |= kart.Contains(nokta);
            return el;
        }

        public void TusBasildi(Keys tus)
        {
            bool zipla = tus == Keys.Space || tus == Keys.Up || tus == Keys.W;
            bool onay = tus == Keys.Enter || tus == Keys.Space;

            switch (oyun.Durum)
            {
                case OyunDurumu.AnaMenu:
                    if (tus == Keys.Left || tus == Keys.A) oyun.SonrakiKarakter(-1);
                    else if (tus == Keys.Right || tus == Keys.D) oyun.SonrakiKarakter(1);
                    else if (onay) PlayaBasildi();
                    break;

                case OyunDurumu.Hazir:
                case OyunDurumu.Oynuyor:
                    if (zipla) oyun.Zipla();
                    else if (tus == Keys.Escape || tus == Keys.P) MenuyeBasildi();
                    break;

                case OyunDurumu.Duraklatildi:
                    if (tus == Keys.Escape || tus == Keys.P || tus == Keys.Enter) oyun.DevamEt();
                    else if (tus == Keys.M) oyun.SesiAcKapat();
                    else if (tus == Keys.Q) oyun.AnaMenuyeDon();
                    break;

                case OyunDurumu.OyunBitti:
                    if (!BitisButonlariHazir) break;
                    if (onay || tus == Keys.R) PlayaBasildi();
                    else if (tus == Keys.Escape) AnaMenuyeBasildi();
                    break;
            }
        }

        // Buton sesi sadece PLAY, MENU ve karakter seçiminde çalar.
        private void PlayaBasildi()
        {
            oyun.ButonSesi();
            oyun.YeniOyun();
        }

        private void MenuyeBasildi()
        {
            oyun.ButonSesi();
            oyun.Duraklat();
        }

        private void AnaMenuyeBasildi()
        {
            oyun.ButonSesi();
            oyun.AnaMenuyeDon();
        }

        // ---------------- Çizim ----------------

        public void Ciz(Graphics g)
        {
            btnSes.Metin = oyun.SesAcik ? "Ses: Açık" : "Ses: Kapalı";
            btnSes.Ikon = oyun.SesAcik ? Varliklar.PatiPembe : Varliklar.PatiMor;

            switch (oyun.Durum)
            {
                case OyunDurumu.AnaMenu:
                    AnaMenuCiz(g);
                    break;
                case OyunDurumu.Hazir:
                    GostergeCiz(g, true);
                    HazirCiz(g);
                    break;
                case OyunDurumu.Oynuyor:
                    GostergeCiz(g, true);
                    break;
                case OyunDurumu.Duraklatildi:
                    GostergeCiz(g, false);
                    MenuCiz(g);
                    break;
                case OyunDurumu.OyunBitti:
                    OyunBittiCiz(g);
                    break;
            }
        }

        private void AnaMenuCiz(Graphics g)
        {
            float orta = Ayarlar.EkranGenislik / 2f;
            float sallanma = (float)Math.Sin(oyun.Zaman * 0.05f) * 4f;

            Cizim.GokkusagiYazi(g, "FLAPPY CAT", 54f, new PointF(orta, 108 + sallanma), 9f);
            Cizim.Yazi(g, "Karakterini seç!", 24f, new PointF(orta, 182), Renkler.Krem, 6f);
            g.DrawImage(Varliklar.PatiPembe, orta - 150, 168, 30, 26);
            g.DrawImage(Varliklar.PatiPembe, orta + 120, 168, 30, 26);

            for (int i = 0; i < Karakterler.Length; i++)
            {
                KarakterTuru tur = Karakterler[i];
                bool secili = tur == oyun.SeciliKarakter;
                RectangleF kart = karakterKartlari[i];
                if (secili) kart.Inflate(5, 5);

                Cizim.Kart(g, kart, secili ? Renkler.SariAcik : Renkler.Krem, 18f, secili ? 5f : 4f);

                Bitmap gorsel = Varliklar.SecimGorselleri[tur];
                float zipla = secili ? (float)Math.Abs(Math.Sin(oyun.Zaman * 0.12f)) * -8f : 0f;
                g.DrawImage(gorsel, kart.X + (kart.Width - gorsel.Width) / 2f, kart.Y + 18 + zipla, gorsel.Width, gorsel.Height);

                Cizim.Yazi(g, KarakterBilgisi.Ad(tur), 20f, new PointF(kart.X + kart.Width / 2f, kart.Bottom - 26),
                    secili ? Renkler.Pembe : Renkler.Krem, 5f);

                if (secili)
                {
                    float p = (float)(0.7 + 0.3 * Math.Sin(oyun.Zaman * 0.15));
                    Cizim.MerkezdenCiz(g, Varliklar.Parilti, new PointF(kart.Right - 8, kart.Y + 8), 1.1f * p);
                    Cizim.MerkezdenCiz(g, Varliklar.Parilti, new PointF(kart.X + 10, kart.Bottom - 40), 0.8f * (1.6f - p));
                }
            }

            btnPlay.Ciz(g);

            Cizim.Yazi(g, "Yön tuşları : Karakter seç    ENTER : Başla", 15f, new PointF(orta, 560), Renkler.Krem, 4f);
            Cizim.Yazi(g, "SPACE / YUKARI : Zıpla    ESC : Menü", 15f, new PointF(orta, 588), Renkler.Krem, 4f);
        }

        /// <summary>Oyun içi göstergeler: skor paneli ve MENU butonu.</summary>
        private void GostergeCiz(Graphics g, bool menuButonu)
        {
            Bitmap panel = Varliklar.Panel;
            float px = (Ayarlar.EkranGenislik - panel.Width) / 2f;
            g.DrawImage(panel, px, 16, panel.Width, panel.Height);
            Cizim.Yazi(g, oyun.Skor.ToString(), 34f, new PointF(px + panel.Width * 0.38f, 16 + panel.Height / 2f), Renkler.Beyaz, 7f);

            if (menuButonu) btnMenu.Ciz(g);
        }

        private void HazirCiz(Graphics g)
        {
            float orta = Ayarlar.EkranGenislik / 2f;
            float sallanma = (float)Math.Sin(oyun.Zaman * 0.1f) * 3f;
            Cizim.Yazi(g, "HAZIR MISIN?", 40f, new PointF(orta, 205 + sallanma), Renkler.Sari, 8f);

            float p = (float)Math.Abs(Math.Sin(oyun.Zaman * 0.1f)) * -6f;
            g.DrawImage(Varliklar.PatiPembe, orta - 21, 410 + p, 42, 37);
            Cizim.Yazi(g, "SPACE / YUKARI / Tıkla : Zıpla", 20f, new PointF(orta, 478), Renkler.Krem, 5f);
        }

        private void MenuCiz(Graphics g)
        {
            KaraltmaCiz(g, 110);
            float orta = Ayarlar.EkranGenislik / 2f;

            Cizim.Kart(g, new RectangleF(orta - 155, 190, 310, 330), Renkler.Krem, 26f, 5f, false);
            Cizim.Yazi(g, "MENÜ", 38f, new PointF(orta, 232), Renkler.Pembe, 7f);

            btnDevam.Ciz(g);
            btnSes.Ciz(g);
            btnCik.Ciz(g);

            Cizim.Yazi(g, "ESC: Devam   M: Ses   Q: Çık", 15f, new PointF(orta, 548), Renkler.Krem, 4f);
        }

        private void OyunBittiCiz(Graphics g)
        {
            if (oyun.BitisSuresi < BitisBekleme * 0.6f) return;

            float giris = Math.Min(1f, (oyun.BitisSuresi - BitisBekleme * 0.6f) / 15f);   // 0 → 1 yumuşak geliş
            KaraltmaCiz(g, (int)(110 * giris));
            float orta = Ayarlar.EkranGenislik / 2f;
            float kayma = (1f - giris) * 40f;

            Cizim.Yazi(g, "OYUN BİTTİ", 46f, new PointF(orta, 170 - kayma), Renkler.Pembe, 9f, 0.5f, (int)(255 * giris));

            if (giris < 1f) return;

            var kart = new RectangleF(orta - 140, 218, 280, 206);
            Cizim.Kart(g, kart, Renkler.Krem, 24f, 5f, false);
            Cizim.Yazi(g, "SKOR", 20f, new PointF(orta, 260), Renkler.KremKoyu, 5f);
            Cizim.Yazi(g, oyun.Skor.ToString(), 60f, new PointF(orta, 325), Renkler.Beyaz, 10f);

            btnTekrar.Ciz(g);
            btnAnaMenu.Ciz(g);
        }

        private static void KaraltmaCiz(Graphics g, int saydamlik)
        {
            using (var firca = new SolidBrush(Color.FromArgb(saydamlik, 60, 30, 80)))
                g.FillRectangle(firca, 0, 0, Ayarlar.EkranGenislik, Ayarlar.EkranYukseklik);
        }
    }
}
