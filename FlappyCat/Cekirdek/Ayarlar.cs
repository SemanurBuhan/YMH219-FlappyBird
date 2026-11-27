namespace FlappyCat
{
    /// <summary>
    /// Oyunun tüm sabit değerleri tek bir yerde tutulur.
    /// Hız birimleri "piksel / kare" (60 FPS) cinsindendir.
    /// </summary>
    public static class Ayarlar
    {
        public const int EkranGenislik = 480;
        public const int EkranYukseklik = 720;
        public const int ZeminY = 650;              // bulut zeminin üst sınırı

        public const int KediX = 110;               // kedinin sabit yatay konumu
        public const int HazirY = 300;              // oyun başlamadan önceki yükseklik
        public const float YerCekimi = 0.42f;       // her karede düşey hıza eklenir
        public const float ZiplamaHizi = -7.4f;
        public const float MaksDusmeHizi = 10.5f;

        public const float BaslangicHizi = 3.0f;    // engellerin başlangıç hızı
        public const float HizArtisi = 0.4f;        // her hızlanmada eklenen hız
        public const int MaksHizSeviyesi = 8;
        public const int HizlanmaAraligi = 3;       // kaç engel üst üste geçilince hızlanır

        public const int EngelSayisi = 3;
        public const int EngelAraligi = 235;        // engel çiftleri arası yatay mesafe
        public const int BoslukYuksekligi = 185;    // üst ve alt engel arasındaki boşluk
        public const int BoslukKenarPayi = 70;      // boşluğun ekran kenarlarına en yakın mesafesi
    }
}
