using System.Drawing;
using System.Drawing.Drawing2D;

namespace FlappyCat
{
    /// <summary>
    /// Üstte ve altta birer kedi kulaklı sütundan oluşan engel çifti.
    /// Sağdan sola kayar, ekrandan çıkınca yeniden sağa yerleştirilir.
    /// </summary>
    public class EngelCifti : OyunNesnesi
    {
        // Görsel ölçüleri (engel_kapak.png: üstte kulaklar, altta kenar)
        private const int KulakYuksekligi = 24;
        private const int KenarYuksekligi = 23;
        private const int KenarX = 5, KenarGenislik = 77;
        private const int GovdeX = 10, GovdeGenislik = 67;

        public float BoslukUst { get; private set; }
        public float BoslukAlt { get { return BoslukUst + Ayarlar.BoslukYuksekligi; } }
        public bool Gecildi { get; set; }
        public float Hiz { get; set; }

        public EngelCifti(float x, float boslukUst)
        {
            Genislik = Varliklar.EngelKapak.Width;
            Yukseklik = Ayarlar.EkranYukseklik;
            YenidenKonumlandir(x, boslukUst);
        }

        public void YenidenKonumlandir(float x, float boslukUst)
        {
            X = x;
            Y = 0;
            BoslukUst = boslukUst;
            Gecildi = false;
        }

        public bool EkrandanCiktiMi
        {
            get { return X + Genislik < 0; }
        }

        public override void Guncelle(float dt)
        {
            X -= Hiz * dt;
        }

        public bool CarpiyorMu(RectangleF kutu)
        {
            var ustKenar = new RectangleF(X + KenarX, BoslukUst - KenarYuksekligi, KenarGenislik, KenarYuksekligi);
            var ustGovde = new RectangleF(X + GovdeX, -1000, GovdeGenislik, BoslukUst - KenarYuksekligi + 1000);
            var altKenar = new RectangleF(X + KenarX, BoslukAlt, KenarGenislik, KenarYuksekligi);
            var altGovde = new RectangleF(X + GovdeX, BoslukAlt + KenarYuksekligi, GovdeGenislik, 1000);

            return kutu.IntersectsWith(ustKenar) || kutu.IntersectsWith(ustGovde)
                || kutu.IntersectsWith(altKenar) || kutu.IntersectsWith(altGovde);
        }

        public override void Ciz(Graphics g)
        {
            int w = Genislik;
            int kapakH = Varliklar.EngelKapak.Height;
            int govdeH = Varliklar.EngelGovde.Height;

            // Üst engel: ters çevrilmiş, kulaklar boşluğa doğru bakar
            float ustKapakY = BoslukUst - KenarYuksekligi;
            float ustGovdeY = ustKapakY - govdeH;
            if (ustGovdeY > 0) DolguCiz(g, -1, ustGovdeY + 2);
            g.DrawImage(Varliklar.EngelGovdeTers, X, ustGovdeY, w, govdeH);
            g.DrawImage(Varliklar.EngelKapakTers, X, ustKapakY, w, kapakH);

            // Alt engel: kulaklar yukarı bakar
            float altKapakY = BoslukAlt - KulakYuksekligi;
            float altGovdeY = altKapakY + kapakH;
            float dolguY = altGovdeY + govdeH;
            if (dolguY < Ayarlar.EkranYukseklik) DolguCiz(g, dolguY - 2, Ayarlar.EkranYukseklik - dolguY + 2);
            g.DrawImage(Varliklar.EngelGovde, X, altGovdeY, w, govdeH);
            g.DrawImage(Varliklar.EngelKapak, X, altKapakY, w, kapakH);
        }

        /// <summary>Sütunun uzayan düz kısmını dikey olarak tekrarlayan desenle doldurur.</summary>
        private void DolguCiz(Graphics g, float y, float yukseklik)
        {
            using (var firca = new TextureBrush(Varliklar.EngelDolgu, WrapMode.Tile))
            {
                firca.TranslateTransform(X, 0);
                g.FillRectangle(firca, X, y, Genislik, yukseklik);
            }
        }
    }
}
