namespace FlappyCat
{
    /// <summary>Oyunun içinde bulunabileceği durumlar.</summary>
    public enum OyunDurumu
    {
        AnaMenu,        // karakter seçim ekranı
        Hazir,          // oyun başladı, ilk zıplama bekleniyor
        Oynuyor,
        Duraklatildi,   // menü açık
        OyunBitti
    }
}
