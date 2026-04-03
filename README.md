# Deneme
# SZ
-
-
-
<img width="788" height="927" alt="Ekran görüntüsü 2026-03-11 115535" src="https://github.com/user-attachments/assets/50f0e656-6b18-4105-80a4-a7179641795b" style="border:2px solid red; border-radius:10px;"  />

### 🌡️ BUG-001 — Sensör verisi None döndüğünde sistem çöküyor

**📝 Açıklama**

DHT22 sensörü bağlantı kopukluğunda `None` değeri döndürmektedir. Veri işleme modülü bu değer üzerinde karşılaştırma yapmaya çalışırken `TypeError` fırlatıyor ve tüm otomasyon döngüsü duruyordu.

**❌ Hatalı Kod**

```python
def process_sensor_data(reading):
    # BUG: None kontrolü yapılmadan karşılaştırma
    if reading['soil_moisture'] < MOISTURE_THRESHOLD:
        trigger_irrigation()
    if reading['temperature'] > TEMP_ALERT_LIMIT:
        send_emergency_alert()
# TypeError: '<' not supported between NoneType and int
```
