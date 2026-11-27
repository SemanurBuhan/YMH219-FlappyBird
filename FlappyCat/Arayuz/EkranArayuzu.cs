using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlappyCat
{
    /// <summary>
    /// Ekranların (oyun içi göstergeler, oyun bitti) çizimini ve
    /// fare/klavye girdilerini yönetir.
    /// </summary>
    public class EkranArayuzu
    {
        private const float BitisBekleme = 30f;   // oyun bittikten sonra butonların gelmesi için beklenen kare

        private readonly OyunYoneticisi oyun;
        private readonly Buton btnTekrar;

        public EkranArayuzu(OyunYoneticisi oyun)
        {
            this.oyun = oyun;
            float orta = Ayarlar.EkranGenislik / 2f;
            Size play = Varliklar.ButonPlay.Size;
            btnTekrar = new Buton(new RectangleF(orta - play.Width / 2f, 452, play.Width, play.Height), Varliklar.ButonPlay);
        }

        private bool BitisButonlariHazir
        {
            get { return oyun.Durum == OyunDurumu.OyunBitti && oyun.BitisSuresi >= BitisBekleme; }
        }

        // ---------------- Girdiler ----------------

        public void FareTiklandi(Point nokta)
        {
            if (oyun.Durum == OyunDurumu.OyunBitti)
            {
                if (BitisButonlariHazir && btnTekrar.Icerir(nokta)) oyun.YeniOyun();
            }
            else
            {
                oyun.Zipla();   // ekrana tıklamak da zıplatır
            }
        }

        /// <summary>Fare bir butonun üstündeyse true döner.</summary>
        public bool FareHareket(Point nokta)
        {
            btnTekrar.UzerindeMi = BitisButonlariHazir && btnTekrar.Icerir(nokta);
            return btnTekrar.UzerindeMi;
        }

        public void TusBasildi(Keys tus)
        {
            bool zipla = tus == Keys.Space || tus == Keys.Up || tus == Keys.W;

            if (oyun.Durum == OyunDurumu.OyunBitti)
            {
                if (BitisButonlariHazir && (tus == Keys.Enter || tus == Keys.Space || tus == Keys.R))
                    oyun.YeniOyun();
            }
            else if (zipla)
            {
                oyun.Zipla();
            }
        }

        // ---------------- Çizim ----------------

        public void Ciz(Graphics g)
        {
            switch (oyun.Durum)
            {
                case OyunDurumu.Hazir:
                    GostergeCiz(g);
                    HazirCiz(g);
                    break;
                case OyunDurumu.Oynuyor:
                    GostergeCiz(g);
                    break;
                case OyunDurumu.OyunBitti:
                    OyunBittiCiz(g);
                    break;
            }
        }

        /// <summary>Oyun içi skor paneli.</summary>
        private void GostergeCiz(Graphics g)
        {
            Bitmap panel = Varliklar.Panel;
            float px = (Ayarlar.EkranGenislik - panel.Width) / 2f;
            g.DrawImage(panel, px, 16, panel.Width, panel.Height);
            Cizim.Yazi(g, oyun.Skor.ToString(), 34f, new PointF(px + panel.Width * 0.38f, 16 + panel.Height / 2f), Renkler.Beyaz, 7f);
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
            Cizim.Yazi(g, "ENTER / R : Yeniden Başlat", 16f, new PointF(orta, 560), Renkler.Krem, 4f);
        }

        private static void KaraltmaCiz(Graphics g, int saydamlik)
        {
            using (var firca = new SolidBrush(Color.FromArgb(saydamlik, 60, 30, 80)))
                g.FillRectangle(firca, 0, 0, Ayarlar.EkranGenislik, Ayarlar.EkranYukseklik);
        }
    }
}
