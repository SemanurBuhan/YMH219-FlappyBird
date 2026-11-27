using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FlappyCat
{
    public class GokkusagiIzi
    {
        private readonly List<PointF> noktalar = new List<PointF>();
        private readonly List<Parcacik> parcaciklar = new List<Parcacik>();
        private readonly Random rastgele;
        private float faz;

        public float Yogunluk { get; private set; }
        public int Seviye { get; set; }

        public GokkusagiIzi(Random rastgele)
        {
            this.rastgele = rastgele;
        }

        public void Temizle()
        {
            noktalar.Clear();
            parcaciklar.Clear();
            Yogunluk = 0;
            Seviye = 0;
        }

        public void Guncelle(float dt, PointF kuyruk, float kaymaHizi, bool aktif)
        {
            faz += 0.25f * dt;

            for (int i = 0; i < noktalar.Count; i++)
                noktalar[i] = new PointF(noktalar[i].X - kaymaHizi * dt, noktalar[i].Y);
            noktalar.Add(kuyruk);

            int maksUzunluk = Math.Min(14 + Seviye * 6, 60);
            while (noktalar.Count > maksUzunluk) noktalar.RemoveAt(0);

            float hedef = aktif && Seviye > 0 ? 1f : 0f;
            Yogunluk += (hedef - Yogunluk) * Math.Min(1f, 0.08f * dt);

            if (Yogunluk > 0.3f && rastgele.NextDouble() < 0.12 * dt * Math.Min(Seviye, 4))
            {
                parcaciklar.Add(new Parcacik(
                    kuyruk.X - rastgele.Next(0, 30), kuyruk.Y + rastgele.Next(-14, 15),
                    0.4f + (float)rastgele.NextDouble() * 0.5f, 30 + rastgele.Next(0, 25)));
            }
            for (int i = parcaciklar.Count - 1; i >= 0; i--)
            {
                parcaciklar[i].Guncelle(dt, kaymaHizi * 0.7f);
                if (parcaciklar[i].Bitti) parcaciklar.RemoveAt(i);
            }
        }

        public void Ciz(Graphics g)
        {
            if (Yogunluk < 0.02f || noktalar.Count < 3) return;

            int n = noktalar.Count;
            float xMin = noktalar[0].X, xMax = noktalar[n - 1].X;
            if (xMax - xMin < 3f) return;

            float bant = Math.Min(3.6f + Seviye * 0.3f, 5.4f);
            float toplam = bant * Renkler.Gokkusagi.Length;

            for (int b = 0; b < Renkler.Gokkusagi.Length; b++)
            {
                var cokgen = new PointF[n * 2];
                for (int i = 0; i < n; i++)
                {
                    PointF p = noktalar[i];
                    float dalga = (float)Math.Sin(i * 0.5f - faz) * 1.6f * (1f - i / (float)n);
                    float ust = p.Y - toplam / 2f + b * bant + dalga;
                    cokgen[i] = new PointF(p.X, ust);
                    cokgen[n * 2 - 1 - i] = new PointF(p.X, ust + bant + 0.7f);
                }

                Color renk = Renkler.Gokkusagi[b];
                using (var firca = new LinearGradientBrush(
                           new PointF(xMin - 1, 0), new PointF(xMax + 1, 0),
                           Color.FromArgb(0, renk), Color.FromArgb((int)(245 * Yogunluk), renk)))
                {
                    firca.Blend = new Blend(3) { Factors = new[] { 0f, 0.85f, 1f }, Positions = new[] { 0f, 0.3f, 1f } };
                    g.FillPolygon(firca, cokgen);
                }
            }

            foreach (Parcacik p in parcaciklar)
                p.Ciz(g, Yogunluk);
        }
    }
}
