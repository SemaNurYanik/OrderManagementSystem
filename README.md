# Order Management System

Bu proje, .NET 8.0 kullanarak geliştirilmiş bir Sipariş Yönetim Sistemi'dir. 

Sistem, siparişlerin API üzerinden oluşturulmasını, RabbitMQ ile kuyruğa gönderilmesini ve arka planda çalışan bir Worker Service ile siparişlerin işlenmesini sağlar.


## Özellikler

- RESTful API ile sipariş ekleme, listeleme ve sorgulama

- RabbitMQ kullanarak siparişlerin kuyruğa eklenmesi

- Background Service (Worker Service) ile siparişlerin işlenmesi

- SQLite Database kullanarak veritabanı yönetimi

- xUnit & Moq ile Unit Testler


## Gereksinimler

Bu projeyi çalıştırmak için aşağıdaki araçların sisteminizde kurulu olması gerekmektedir:

- .NET 8.0 SDK → [İndir](https://dotnet.microsoft.com/en-us/download)
- Docker (Opsiyonel, RabbitMQ için) → [İndir](https://www.docker.com/get-started)


## Kurulum Adımları

**Bağımlılıkları Yükle
.NET bağımlılıklarını yüklemek için:

terminale: dotnet restore

**RabbitMQ’yu Başlat
Docker Kullanıyorsanız, RabbitMQ’yu aşağıdaki komutla başlatabilirsiniz:

terminale: docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management

**RabbitMQ Yönetim Paneline şu adresten erişebilirsiniz: http://localhost:15672

Varsayılan giriş bilgileri:
Kullanıcı Adı: guest
Şifre: guest

Eğer Docker kullanmıyorsanız, RabbitMQ’yu manuel olarak indirip çalıştırmalısınız.

**SqLite veritabanının api ve worker service için ortak kullanılması için data katmanı oluşturuldu ve database'e ulaşabilmek için api projesinde ve background services projesinde
appsettings.json dosyasında aşağıdaki satır kendi local path'iniz olarak değiştirilmesi gerekmektedir.
 "ConnectionStrings": {
    "DefaultConnection": "Data Source=C:\\Users\\Semanur\\Desktop\\OrderManagementSystem\\OrderManagementData\\orders.db"  }


**Uygulamayı Çalıştır

API’yi başlatmak için:
terminale: dotnet run --project OrderManagementSystem


Eğer Swagger UI üzerinden API’yi test etmek istiyorsanız, aşağıdaki URL’yi açabilirsiniz:
 http://localhost:5015/swagger

**Background Worker Service’i Çalıştır

Arka planda siparişleri işleyen Worker Service’i çalıştırmak için:
terminale: dotnet run --project BackgroundServices


## API Kullanımı:

1)Yeni Sipariş Oluştur / Yeni bir sipariş eklemek için:
POST /api/orders

Request Body (JSON):
{
  "productName": "Laptop",
  "price": 15000
}

CURL Komutu ile Örnek Kullanım:

curl -X POST "http://localhost:5015/api/orders" -H "Content-Type: application/json" -d '{"productName": "Laptop", "price": 15000}'


2)Belirli Bir Siparişin Bilgilerini Al:
GET /api/orders/{id}
Örnek: GET http://localhost:5015/api/orders/orderId


3️) Tüm Siparişleri Listele:
GET /api/orders


## Testlerin Çalıştırılması:

Projede xUnit ve Moq kullanılarak yazılmış testler bulunmaktadır.
**Testleri çalıştırmak için:
terminale: dotnet test

**Eğer sadece belirli bir test sınıfını çalıştırmak isterseniz:
terminale: dotnet test --filter OrderControllerTests



## Proje Yapısı

OrderManagementSystem/

│── OrderManagementSystem/            # API Projesi
│── BackgroundServices/               # Worker Service (Arka plan servisi)
│── OrderManagementData/              # Data Katmanı
│── OrderManagementSystem.Tests/      # xUnit Test Projesi
│── README.md                         # Kurulum ve Kullanım Rehberi
│── OrderManagementSystem.sln         # Solution dosyası

