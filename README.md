CaseStudy TD – 2D Sprite’larla 3D Dünyada Tower-Defense Prototipi

Bu repo, 2D sprite’ların billboard tekniğiyle 3D bir sahnede kullanıldığı küçük bir Tower-Defense prototipidir. Hedef, temel oyun döngüsünü; dalga tabanlı düşman akışı, oyuncu hareketi/otomatik saldırı, basit UI, ses ve VFX ile oynanabilir şekilde göstermekti.

Motor & Sürüm:

Unity 6 — 6000.0.34f1

Render Pipeline: URP (varsayılan ayarlar)

Platform: Windows (Editor)


Oynanış – Kontroller
Tuş	        İşlev

W A S D	        Oyuncu hareketi (top-down)

Otomatik	En yakındaki hedefe doğru otomatik atış (öne doğru koni + fallback)

1 / 2 / 3	Silah/renk/pool değiştirme (Blue / Orange / Purple)

Tab		Yakın dövüş (melee) pulse aç/kapa


N		Bir sonraki dalgayı erken çağır

C		Chaser düşmanlarını burst olarak spawnla(zaten otomatik geliyor c ile de gelebilir)

UI		Ekrandaki 0.6x / 1.0x / 1.8x hız butonları yalnızca düşmanları etkiler (oyuncuyu değil)

Ana Özellikler
Oyuncu:

WASD hareket.

AutoShooter3D: En yakındaki düşmanı ön koni içinde bulup ateş eder; hedef yoksa isteğe bağlı geniş koni fallback (aim assist).

Melee modu (Tab): Yarıçap içinde pulse hasar.

I-frame: PlayerHealth3D hasar alınca kısa süre dokunulmazlık + görsel blink. Respawn sonrası extra i-frame.

Silah Değişimi: 1/2/3 ile farklı pool ve SFX.

Düşmanlar:

TD.Enemy: Path üzerindeki waypoint’leri izler.

Spawner: Dalga sistemi; 5/10/15. dalgalarda boss benzeri güçlendirme; her dalgada renk/hız/HP profil değişimi.

Early Wave: N ile sıradaki dalgayı erken çağırma.

Chaser (Survivors tarzı): ChaserSpawner ile her 3 saniyede küçük bir burst; ayrıca her 3. dalgada ekstra sürü. Dış çerçeveden (minPos/maxPos) spawn olur; oyuncuya en az X mesafe ile doğar.

VFX / SFX

VFX_Hit_Common (Animator + AnimatedObjectDestroy) → isabetlerde kısa partikül.

AudioController: SFX/Music için tekil yönetici (mixer route’lu).

Atış, isabet, melee, düşman ölümü, dalga başı/çağırma, oyuncu hasar/ölüm/respawn gibi temel SFX’ler.


Nasıl Konfigüre Ederim?

Spawner (TD/Spawner):

Waves: WaveData dizisi; dalga başına spawn adedi/sıklığı.

Boss dalgaları sabit: 5/10/15.

EnemyData: HP / hız / base hasar / kaynak ödülü. Prefab üzerinde TD.Enemy.Data alanına ilgili asset’i ata.

ChaserSpawner:

minPos / maxPos: spawn dikdörtgeni köşeleri (Transform).

everyNWave: her kaç dalgada bir burst.

usePeriodic & periodSeconds: periyodik burst.

testKey (C): debug spawn.

Audio:

AudioController tek sahnede (MainMenu) bulunur, DontDestroyOnLoad.

Mixer: MasterMixer (Groups: Music/SFX/UI).


Hız butonları: yalnız Enemy global speed’i değiştirir.


Varsayımlar, Açıklamalar ve Notlar:

Bu prototip öğrenme ve hız odaklı hazırlanmıştır.

Oyun sektöründe yeniyim; daha önce yaptığım iki küçük projeden bazı utility kodları (pooling, temel UI akışları) referans aldım ve sadeleştirerek yeniden yazdım.

Asset’lerin çoğu placeholder (ücretsiz/paket içi). Amaç, teknik yapı ve oynanabilir döngüyü göstermektir.
