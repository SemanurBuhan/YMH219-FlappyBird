using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FlappyCat
{
    /// <summary>
    /// Oyunun kurallarını ve durumunu yönetir: skor, hız, engeller,
    /// çarpışma kontrolü, menü ve oyun durumları arasındaki geçişler.
    /// </summary>
    public class OyunYoneticisi
    {
        private readonly Random rastgele = new Random();
        private readonly List<EngelCifti> engeller = new List<EngelCifti>();
        private readonly List<Bulut> bulutlar = new List<Bulut>();
        private readonly Zemin zemin = new Zemin();
        private readonly SesYoneticisi ses;

        private OyunDurumu duraklatmaOncesiDurum;
        private PointF carpmaNoktasi;
        private float sonBoslukUst;
        private float sarsinti;
        private float flas;

        public event EventHandler SkorDegisti;
        public event EventHandler OyunBitti;

        public OyunDurumu Durum { get; private set; }
        public KarakterTuru SeciliKarakter { get; private set; }
        public Kedi Kedi { get; private set; }
        public int Skor { get; private set; }
        public float Hiz { get; private set; }
        public float Zaman { get; private set; }
        public float BitisSuresi { get; private set; }
        public bool SesAcik { get { return ses.SesAcik; } }
        public IReadOnlyList<EngelCifti> Engeller { get { return engeller; } }

        public OyunYoneticisi(SesYoneticisi ses)
        {
            this.ses = ses;
            SeciliKarakter = KarakterTuru.Gri;
            Kedi = new Kedi(SeciliKarakter);
            Hiz = Ayarlar.BaslangicHizi;

            for (int i = 0; i < 5; i++)
            {
                Bitmap b = Varliklar.Bulutlar[rastgele.Next(Varliklar.Bulutlar.Length)];
                bulutlar.Add(new Bulut(b, 0.55f + (float)rastgele.NextDouble() * 0.35f, 0.75f,
                    0.25f + (float)rastgele.NextDouble() * 0.35f,
                    rastgele.Next(-40, Ayarlar.EkranGenislik), rastgele.Next(40, 430), rastgele));
            }

            Durum = OyunDurumu.AnaMenu;
        }

        // ---------------- Kullanıcı işlemleri ----------------

        public void KarakterSec(KarakterTuru tur)
        {
            SeciliKarakter = tur;
            Kedi.KarakterAyarla(tur);
            ButonSesi();
        }

        public void SonrakiKarakter(int yon)
        {
            int adet = Enum.GetValues(typeof(KarakterTuru)).Length;
            KarakterSec((KarakterTuru)(((int)SeciliKarakter + yon + adet) % adet));
        }

        /// <summary>Yeni oyun (veya yeniden başlat): her şeyi sıfırlar ve ilk zıplamayı bekler.</summary>
        public void YeniOyun()
        {
            Kedi.KarakterAyarla(SeciliKarakter);
            Kedi.Sifirla(Ayarlar.HazirY);

            Skor = 0;
            Hiz = Ayarlar.BaslangicHizi;
            BitisSuresi = 0;
            sarsinti = 0;
            flas = 0;

            engeller.Clear();
            sonBoslukUst = (Ayarlar.ZeminY - Ayarlar.BoslukYuksekligi) / 2f;
            for (int i = 0; i < Ayarlar.EngelSayisi; i++)
                engeller.Add(new EngelCifti(Ayarlar.EkranGenislik + 140 + i * Ayarlar.EngelAraligi, RastgeleBosluk()));

            Durum = OyunDurumu.Hazir;
            OnSkorDegisti();
        }

        public void Zipla()
        {
            if (Durum == OyunDurumu.Hazir) Durum = OyunDurumu.Oynuyor;
            if (Durum != OyunDurumu.Oynuyor) return;

            Kedi.Zipla();
        }

        public void Duraklat()
        {
            if (Durum != OyunDurumu.Oynuyor && Durum != OyunDurumu.Hazir) return;
            duraklatmaOncesiDurum = Durum;
            Durum = OyunDurumu.Duraklatildi;
        }

        public void DevamEt()
        {
            if (Durum != OyunDurumu.Duraklatildi) return;
            Durum = duraklatmaOncesiDurum;
        }

        public void SesiAcKapat()
        {
            ses.SesAcik = !ses.SesAcik;
        }

        /// <summary>PLAY, MENU ve karakter seçiminde çalan yumuşak buton sesi.</summary>
        public void ButonSesi()
        {
            ses.Cal("tik");
        }

        public void AnaMenuyeDon()
        {
            Durum = OyunDurumu.AnaMenu;
            sarsinti = 0;
            flas = 0;
        }

        // ---------------- Oyun döngüsü ----------------

        public void Guncelle(float dt)
        {
            Zaman += dt;
            if (Durum == OyunDurumu.Duraklatildi) return;

            foreach (Bulut b in bulutlar) b.Guncelle(dt);

            switch (Durum)
            {
                case OyunDurumu.AnaMenu:
                    zemin.Hiz = 1.5f;
                    zemin.Guncelle(dt);
                    break;

                case OyunDurumu.Hazir:
                    zemin.Hiz = Hiz;
                    zemin.Guncelle(dt);
                    Kedi.Suzul(Zaman, dt);
                    break;

                case OyunDurumu.Oynuyor:
                    OynuyorGuncelle(dt);
                    break;

                case OyunDurumu.OyunBitti:
                    BitisSuresi += dt;
                    Kedi.Dus(dt);
                    sarsinti *= (float)Math.Pow(0.85, dt);
                    flas = Math.Max(0f, flas - 0.07f * dt);
                    break;
            }
        }

        private void OynuyorGuncelle(float dt)
        {
            zemin.Hiz = Hiz;
            zemin.Guncelle(dt);

            Kedi.Guncelle(dt);
            if (Kedi.Y < -Kedi.Yukseklik) Kedi.Y = -Kedi.Yukseklik;   // ekranın çok üstüne çıkamaz

            // Engelleri kaydır, ekrandan çıkanları en sağdakinin arkasına taşı
            foreach (EngelCifti e in engeller)
            {
                e.Hiz = Hiz;
                e.Guncelle(dt);
            }
            foreach (EngelCifti e in engeller)
            {
                if (!e.EkrandanCiktiMi) continue;
                float enSag = float.MinValue;
                foreach (EngelCifti diger in engeller) enSag = Math.Max(enSag, diger.X);
                e.YenidenKonumlandir(enSag + Ayarlar.EngelAraligi, RastgeleBosluk());
            }

            // Skor: engelin ortası kedinin ortasını geçince
            float kediMerkez = Kedi.X + Kedi.Genislik / 2f;
            foreach (EngelCifti e in engeller)
            {
                if (!e.Gecildi && e.X + e.Genislik / 2f < kediMerkez)
                {
                    e.Gecildi = true;
                    PuanKazan();
                }
            }

            // Çarpışma kontrolü
            RectangleF kutu = Kedi.Sinirlar;
            foreach (EngelCifti e in engeller)
            {
                if (e.CarpiyorMu(kutu))
                {
                    OyunuBitir(new PointF(kutu.Right - 6, kutu.Y + kutu.Height / 2f));
                    return;
                }
            }
            if (kutu.Bottom >= Ayarlar.ZeminY + 6)
                OyunuBitir(new PointF(kutu.X + kutu.Width / 2f, Ayarlar.ZeminY));
        }

        private void PuanKazan()
        {
            Skor++;
            OnSkorDegisti();
        }

        private void OyunuBitir(PointF nokta)
        {
            Durum = OyunDurumu.OyunBitti;
            carpmaNoktasi = nokta;
            BitisSuresi = 0;
            sarsinti = 12f;
            flas = 1f;
            Kedi.Carp();
            ses.Cal("miyav");   // canı yanan minik kedi

            if (OyunBitti != null) OyunBitti(this, EventArgs.Empty);
        }

        private void OnSkorDegisti()
        {
            if (SkorDegisti != null) SkorDegisti(this, EventArgs.Empty);
        }

        /// <summary>Bir önceki boşluğa göre çok uzak olmayan rastgele bir boşluk konumu üretir.</summary>
        private float RastgeleBosluk()
        {
            float min = Ayarlar.BoslukKenarPayi;
            float max = Ayarlar.ZeminY - Ayarlar.BoslukYuksekligi - Ayarlar.BoslukKenarPayi;
            float yeni = sonBoslukUst + rastgele.Next(-170, 171);
            yeni = Math.Max(min, Math.Min(max, yeni));
            sonBoslukUst = yeni;
            return yeni;
        }

        // ---------------- Çizim ----------------

        /// <summary>Arka plan, bulutlar, engeller, zemin ve kediyi çizer.</summary>
        public void DunyayiCiz(Graphics g)
        {
            g.DrawImage(Varliklar.Arkaplan, 0, 0, Ayarlar.EkranGenislik, Ayarlar.EkranYukseklik);
            foreach (Bulut b in bulutlar) b.Ciz(g);

            GraphicsState durum = g.Save();
            if (sarsinti > 0.5f)
                g.TranslateTransform((float)(rastgele.NextDouble() - 0.5) * sarsinti,
                                     (float)(rastgele.NextDouble() - 0.5) * sarsinti);

            bool oyunEkrani = Durum != OyunDurumu.AnaMenu;
            if (oyunEkrani)
                foreach (EngelCifti e in engeller) e.Ciz(g);

            zemin.Ciz(g);

            if (oyunEkrani)
            {
                Kedi.Ciz(g);
                if (Durum == OyunDurumu.OyunBitti) PatCiz(g);
            }
            g.Restore(durum);

            if (flas > 0.01f)
            {
                using (var firca = new SolidBrush(Color.FromArgb((int)(170 * flas), Color.White)))
                    g.FillRectangle(firca, 0, 0, Ayarlar.EkranGenislik, Ayarlar.EkranYukseklik);
            }
        }

        /// <summary>Çarpışma anında "PAT!" efekti.</summary>
        private void PatCiz(Graphics g)
        {
            if (BitisSuresi > 50) return;
            float olcek = BitisSuresi < 6 ? 0.4f + BitisSuresi / 6f * 0.6f : 1f - Math.Min(0.15f, (BitisSuresi - 6) * 0.01f);
            float saydamlik = BitisSuresi < 35 ? 1f : 1f - (BitisSuresi - 35) / 15f;
            float x = Math.Max(60, Math.Min(Ayarlar.EkranGenislik - 60, carpmaNoktasi.X));
            float y = Math.Max(60, Math.Min(Ayarlar.ZeminY - 40, carpmaNoktasi.Y));
            Cizim.MerkezdenCiz(g, Varliklar.Pat, new PointF(x, y), olcek * 1.1f, saydamlik);
        }
    }
}
