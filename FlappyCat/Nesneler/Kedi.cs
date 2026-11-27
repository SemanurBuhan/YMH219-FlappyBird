using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FlappyCat
{
    public class Kedi : OyunNesnesi
    {
        private const float AnimasyonAraligi = 5f;

        private Bitmap[] kareler;
        private int kareIndeksi;
        private float animasyonSayaci;

        public KarakterTuru Tur { get; private set; }
        public float DikeyHiz { get; private set; }
        public float Aci { get; private set; }

        public Kedi(KarakterTuru tur)
        {
            X = Ayarlar.KediX;
            KarakterAyarla(tur);
        }

        public void KarakterAyarla(KarakterTuru tur)
        {
            Tur = tur;
            kareler = Varliklar.KediKareleri[tur];
            Genislik = kareler[0].Width;
            Yukseklik = kareler[0].Height;
        }

        public void Sifirla(float y)
        {
            Y = y;
            DikeyHiz = 0;
            Aci = 0;
            kareIndeksi = 0;
            animasyonSayaci = 0;
        }

        public void Zipla()
        {
            DikeyHiz = Ayarlar.ZiplamaHizi;
            kareIndeksi = 1;
            animasyonSayaci = 0;
        }

        public void Carp()
        {
            DikeyHiz = -4f;
        }

        public override void Guncelle(float dt)
        {
            DikeyHiz = Math.Min(DikeyHiz + Ayarlar.YerCekimi * dt, Ayarlar.MaksDusmeHizi);
            Y += DikeyHiz * dt;

            float hedefAci = DikeyHiz < 0 ? -22f : Math.Min(-22f + DikeyHiz * 9f, 70f);
            Aci += (hedefAci - Aci) * Math.Min(1f, 0.25f * dt);

            Animasyon(dt);
        }

        public void Suzul(float zaman, float dt)
        {
            Y = Ayarlar.HazirY + (float)Math.Sin(zaman * 0.08f) * 7f;
            Aci = (float)Math.Sin(zaman * 0.08f + 1f) * 4f;
            Animasyon(dt);
        }

        public void Dus(float dt)
        {
            float zemin = Ayarlar.ZeminY - Yukseklik + 8;
            if (Y >= zemin) { Y = zemin; return; }

            DikeyHiz = Math.Min(DikeyHiz + Ayarlar.YerCekimi * dt, Ayarlar.MaksDusmeHizi);
            Y = Math.Min(Y + DikeyHiz * dt, zemin);
            Aci += (90f - Aci) * Math.Min(1f, 0.15f * dt);
        }

        private void Animasyon(float dt)
        {
            animasyonSayaci += dt;
            if (animasyonSayaci >= AnimasyonAraligi)
            {
                animasyonSayaci = 0;
                kareIndeksi = (kareIndeksi + 1) % kareler.Length;
            }
        }

        public override RectangleF Sinirlar
        {
            get { return new RectangleF(X + 20, Y + 8, Genislik - 26, Yukseklik - 15); }
        }

        public PointF KuyrukNoktasi
        {
            get
            {
                double radyan = Aci * Math.PI / 180.0;
                float ox = -Genislik / 2f + 12f, oy = 4f;
                float cx = X + Genislik / 2f, cy = Y + Yukseklik / 2f;
                return new PointF(
                    cx + (float)(ox * Math.Cos(radyan) - oy * Math.Sin(radyan)),
                    cy + (float)(ox * Math.Sin(radyan) + oy * Math.Cos(radyan)));
            }
        }

        public override void Ciz(Graphics g)
        {
            GraphicsState durum = g.Save();
            g.TranslateTransform(X + Genislik / 2f, Y + Yukseklik / 2f);
            g.RotateTransform(Aci);
            g.DrawImage(kareler[kareIndeksi], -Genislik / 2f, -Yukseklik / 2f, Genislik, Yukseklik);
            g.Restore(durum);
        }
    }
}
