# Confidential & Authentic Email System

React + .NET 9 Web API + SQL Server örnek uygulaması. Parolalar güvenli biçimde hash'lenir, iletiler simetrik olarak şifrelenir, anahtarlar alıcının açık anahtarıyla sarılır, iletilerin özeti imzalanarak bütünlük ve gönderici doğrulaması sağlanır.

## Çalıştırma
1) SQL Server bağlantı bilgisini `EmailSystem.Api/appsettings.json` altındaki `DefaultConnection` içinde güncelleyin.
2) JWT anahtarını `Jwt:Key` alanında güçlü ve uzun (en az 32 byte) bir değerle değiştirin.
3) API'yi başlatın:
```powershell
cd EmailSystem.Api
dotnet run --launch-profile https
```
4) Frontend `.env` dosyasında API adresini tanımlayın (varsayılan `https://localhost:7177`):
```
VITE_API_URL=https://localhost:7177
```
5) Frontend'i çalıştırın:
```powershell
cd frontend
npm install
npm run dev
```

## Özellikler
- Kullanıcı kayıt/giriş, parolalar `PasswordHasher` ile hash'lenir.
- Kullanıcıya özel RSA anahtar çifti üretimi; açık anahtar veritabanında tutulur, özel anahtar demo amaçlı sunucuda saklanır.
- Mesaj gönderirken:
  - Mesaj gövdesi AES-GCM (256 bit) ile şifrelenir.
  - AES anahtarı alıcının açık anahtarıyla (RSA-OAEP SHA-256) sarılır.
  - Gövde SHA-256 özeti alınır ve gönderenin özel anahtarıyla (RSA-PSS) imzalanır.
- Alıcı:
  - Anahtarı kendi özel anahtarıyla açar, gövdeyi çözer.
  - Özeti doğrular, imzayı gönderenin açık anahtarıyla doğrular.
- React arayüzü ile kayıt/giriş, mesaj gönderme ve gelen kutusu görüntüleme.

## Endpoints (özet)
- `POST /api/auth/register` — kullanıcı oluşturur.
- `POST /api/auth/login` — JWT döner.
- `POST /api/messages` — güvenli mesaj gönderir (Bearer token gerekli).
- `GET /api/messages/inbox` — çözümlenmiş içerik ve doğrulama durumlarıyla gelen kutusu.

## Notlar
- Üretimde özel anahtarlar sunucu tarafında tutulmamalı; HSM veya istemci yanlı saklama tercih edin.
- TLS zorunlu tutulmalıdır (`appsettings` yerine barındırma ortamında gizli yönetimi yapın).
- Node 20.19+ veya 22.12+ Vite için önerilir.

