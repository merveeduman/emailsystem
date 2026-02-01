# Rapor Taslağı

1. **Giriş**
   - Proje amacı, teknoloji yığını (React, .NET 9, SQL Server), tehdit modeli.
2. **Kriptografik Teknikler**
   - Parola saklama: ASP.NET `PasswordHasher`, kullanılan algoritma ve salt işlemesi.
   - Mesaj gizliliği: AES-GCM ile gövde şifreleme, 256-bit anahtar, IV üretimi.
   - Anahtar sarma: RSA 2048 OAEP-SHA256 ile simetrik anahtarın alıcı açık anahtarıyla şifrelenmesi.
   - Bütünlük ve kimlik doğrulama: SHA-256 özet + RSA-PSS imzası; alıcı tarafı doğrulama adımları.
3. **Sistem Mimarisi**
   - API uç noktaları, JWT ile kimlik doğrulama akışı.
   - Veritabanı şeması (Users, Messages tabloları; saklanan alanlar).
4. **İş Akışları (Ekran görüntüleri ekleyin)**
   - Kayıt ve giriş ekranı.
   - Mesaj gönderme formu.
   - Gelen kutusu, doğrulama rozetleri.
5. **Hata Yönetimi**
   - Yanlış parola/oturum açma.
   - Eksik alıcı, imza doğrulama başarısızlık senaryosu (API 401/404/400 yanıtları).
6. **Güvenlik Değerlendirmesi**
   - Güçlü parola gereksinimi, TLS zorunluluğu, anahtar saklama tercihi (üretimde HSM/istemci).
   - Sınırlamalar: Demo için özel anahtar sunucuda tutuluyor, rate-limit eklenmedi.
7. **Kurulum ve Çalıştırma**
   - `appsettings.json` ve `.env` yapılandırmaları.
   - `dotnet run` ve `npm run dev/build` komutları.
8. **Sonuç**
   - Öğrenilenler, gelecekte yapılacak iyileştirmeler (anahtar rotasyonu, denetim logları). 

